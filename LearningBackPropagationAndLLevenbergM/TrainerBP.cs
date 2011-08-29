using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using ZScore;

namespace LearningBPandLM
{
    class TrainerBP
    {
        #region parametry i zmienne algorytmu

        /// <summary>
        /// wspolczynnik uczenia wyznacza szybkosc z jaka zmieniaja sie wagi
        /// </summary>
        private double learningRate;

        /// <summary>
        /// wspolczynniki zmian wag dla reguly delta dla wag pomiedzy warstwa WE a ukryta
        /// </summary>
        private double[][] deltaInputHidden;
        /// <summary>
        /// wspolczynniki zmian wag dla reguly delta dla wag pomiedzy warstwa ukryta a WY
        /// </summary>
        private double[][] deltaHiddenOutput;

        /// <summary>
        /// Wektory z bledami dla warstwy ukrytej i WY
        /// </summary>
        private double[] hiddenErrorGradients, outputErrorGradients;

        #endregion

        #region parametry cyklu nauczania sieci

        /// <summary>
        /// liczniki epok
        /// </summary>
        private ulong epochCounter, maxEpochs;

        /// <summary>
        /// docelowa dokladnosc modelu
        /// </summary>
        private double desiredMSE;

        /// <summary>
        /// celnosc modelu dla kazdej z epok dla zbioru trenujacego i testujacego
        /// </summary>
        private double trainingSetAccuracy, generalizationSetAccuracy;

        /// <summary>
        /// suma kwadratow bledu dla zbioru trenujacego i testujacego
        /// </summary>
        private double trainingSetMSE, generalizationSetMSE;

        /// <summary>
        /// struktura sieci neuronowej (sieć MLP (multilayer perceptron networks))
        /// </summary>
        private neuralNetwork NN;

        /// <summary>
        /// Zbior danych, klasa ZScore zawiera zstandaryzowane dane:
        /// Dataset.sample(i) - i-ta probka danych (WE) 
        /// Dataset.target(i) - i-ta wartosc docelowa (WY) dla i-tej probki
        /// </summary>
        private ZScoreData Dataset;

        /// <summary>
        /// Zbior indeksow wydzielajacych zbiory danych
        /// DatasetIndexes.TrainingSet - int[] dane trenujace (uczace)
        /// DatasetIndexes.GeneralizationSet - int[] dane testujace
        /// </summary>
        private DatasetStructure DatasetIndexes;

        /// <summary>
        /// training-set-MSE w poprzedniej epoce
        /// </summary>
        private double previousTMSE;

        /// <summary>
        /// Zlicza ile razy z rzedu nie nastapily zmiany w modelu
        /// </summary>
        private int noImprovementInModelTimes;

        /// <summary>
        /// Maksymalna ilosc razy z rzedu gdy model nie zmienil sie 
        /// (opierajac sie o dokladnosc DifferenceBetweenMSEs)
        /// </summary>
        private readonly int MaxNoImprovementTimes = 10;

        /// <summary>
        /// dopuszczalna roznica miedzy poprzednim a aktualnym MSE 
        /// </summary>
        private readonly double DifferenceBetweenMSEs = 0.0001;

        #endregion

        #region dodatkowe parametry

        /// <summary>
        /// nazwa pliku z wynikami pomiarów
        /// </summary>
        private string resultsFileName;

        /// <summary>
        /// nazwa pliku do ktorego zostana zapisane najlepsze wagi oraz wybrane (customWeightsOutputFile)
        /// </summary>
        private string weightsOutputFile, customWeightsOutputFile;

        /// <summary>
        /// flaga czy kontynuowac nauczanie
        /// </summary>
        private bool trainingComplete;

        /// <summary>
        /// domyslna wartosc wspolczynnika uczenia
        /// </summary>
        private const double DefaultLearningRate = 0.1;

        /// <summary>
        /// Zawiera wyliczenia dlugosci czasowych poszczegolnych epok, wyliczenia cykli zegara na epokę
        /// </summary>
        private Hashtable durationOfEachEpoch, durationInElapsedTicks;
        /// <summary>
        /// Uzywany do mierzenia probek czasowych
        /// </summary>
        private Stopwatch timerE, timerA;

        /// <summary>
        /// Flaga czy uruchomienie programu ma przebiec automatycznie czy manualnie
        /// </summary>
        private bool automatedRun = false;

        #endregion

        #region konstruktory

        public TrainerBP(ZScoreData dataset)
            : this(dataset, EnumDatasetStructures.Growing, DatasetStructure.DefaultGeneralizationSetSize,
            2, DefaultLearningRate, 1500, 0.001, 2, false)
        { }


        public TrainerBP(ZScoreData dataset, EnumDatasetStructures ds, int holdout, int hiddenNodeRatio,
            double lr, ulong mE, double dMSE, int sz, bool runAutomated)
        {
            this.Dataset = dataset;

            ///ostatnie 2 argumenty - false, true dla funkcji aktywacji ktore maja byc tanh(x), true dla sigmoid
            NN = new neuralNetwork(Dataset.sample(0).Length, 
                (int)(hiddenNodeRatio * Math.Sqrt(dataset.sample(0).Length * dataset.target(0).Length)), 
                Dataset.target(0).Length, dataset.DataType, ActivationFuncType.Tanh, ActivationFuncType.Sigmoid);

            string setSplitTypeNameForFile;
            switch (ds)
            {
                case EnumDatasetStructures.Growing:
                    DatasetIndexes = new DatasetOperateGrowing(Dataset.NormalizedData[0].GetNum(), holdout, sz);
                    setSplitTypeNameForFile = "G";
                    break;
                case EnumDatasetStructures.Windowed:
                    DatasetIndexes = new DatasetOperateWindowed(Dataset.NormalizedData[0].GetNum(), holdout, sz);
                    setSplitTypeNameForFile = "W";
                    break;
                case EnumDatasetStructures.Simple:
                default:
                    DatasetIndexes = new DatasetOperateSimple(Dataset.NormalizedData[0].GetNum(), holdout, sz);
                    setSplitTypeNameForFile = "S";
                    break;
            }


            maxEpochs = mE;
            learningRate = lr;
            automatedRun = runAutomated;

            createFileNames(holdout, setSplitTypeNameForFile);
            durationOfEachEpoch = new Hashtable();
            durationInElapsedTicks = new Hashtable();
            timerE = new Stopwatch();
            timerA = new Stopwatch();

            desiredMSE = dMSE;
            trainingSetAccuracy = 0;
            generalizationSetAccuracy = 0;
            noImprovementInModelTimes = 0;



            if (!NN.IsLogistic)
            {
                deltaInputHidden = createDeltaList(deltaInputHidden, NN.numInput, NN.numHidden);
                deltaHiddenOutput = createDeltaList(deltaHiddenOutput, NN.numHidden, NN.numOutput);

                hiddenErrorGradients = createErrorGradientStorage(hiddenErrorGradients, NN.numHidden);
                outputErrorGradients = createErrorGradientStorage(outputErrorGradients, NN.numOutput);
            }
            else
            {
                deltaInputHidden = createDeltaList(deltaInputHidden, NN.numInput, NN.numOutput);

                outputErrorGradients = createErrorGradientStorage(outputErrorGradients, NN.numOutput);
            }

            NN.InitializeWeights();
            trainingComplete = false;

            Program.PrintInfo("Utworzono sieć neuronową");
            networkStats();
        }
        #endregion

        /// <summary>
        /// Funkcja nadzorujaca procesem uczenia sieci
        /// </summary>
        public void TrainNetwork()
        {
            if (DatasetIndexes.TrainingSet.Length == 0)
            {
                Console.WriteLine("Za mała próbka treningowa dla obecnie ustawionej konfiguracji!\n" +
                    "Spróbuj zmienić rodzaj podziałów na zbiory (Windowed->Growing->Simple) lub"
                    + "zwiększ wielkość % pojedynczej próbki - obecnie: 0\n");
                return;
            }

            //uchwyt do pliku w ktorym zapisujemy wyniki
            TextWriter saveResult = new StreamWriter(resultsFileName);

            //inicjalizacja licznika epok oraz wag
            epochCounter = 0;

            trainingComplete = false;

            if (!automatedRun)
            {
                Console.Write("\nNaciśnij dowolny przycisk by rozpocząć nauczanie sieci...\n");
                Console.ReadKey();
            }
            ShowOptions();

            //naglowki w pliku z wynikami
            saveResult.WriteLine("#LMtraining: learningRate {0}, NN: {1}(+1):{2}(+1):{3}",
                learningRate, NN.numInput, NN.numHidden, NN.numOutput);
            saveResult.WriteLine("#{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                "epoch", "tMSE", "tAcc", "gMSE", "gAcc", "ms");
            saveResult.Flush();

            saveResultsToFile(saveResult);

            PrintStatus();
            timerA.Reset();

            double bestGeneralizationSetMSE = generalizationSetMSE;

            while ((trainingSetMSE > desiredMSE || generalizationSetMSE > desiredMSE)
                && epochCounter < maxEpochs && !trainingComplete)
            {
                //kolejna epoka, zwiekszamy licznik
                epochCounter++;

                timerE.Restart();
                timerA.Start();
                //pojedyncza epoka
                runTrainingEpoch(DatasetIndexes.TrainingSet);

                timerE.Stop();
                timerA.Stop();

                durationOfEachEpoch.Add(epochCounter, timerE.ElapsedMilliseconds);
                durationInElapsedTicks.Add(epochCounter, timerE.ElapsedTicks);

                /* zapisujemy stan generalizationSetAccuracy by sprawdzic czy MSE sie zmniejszyl
                 * jezeli tak zapisujemy wagi do NN.bestWeights- */

                //zapisz wartosc MSE w poprzedniej epoce
                previousTMSE = trainingSetMSE;

                //zapisujemy wyniki do pliku
                saveResultsToFile(saveResult);

                //zachowujemy wagi dla najnizszego MSE dla zbioru testujacego
                if (bestGeneralizationSetMSE > generalizationSetMSE)
                {
                    bestGeneralizationSetMSE = generalizationSetMSE;
                    NN.BestWeightInputHidden = NN.wInputHidden;
                    NN.BestWeightHiddenOutput = NN.wHiddenOutput;
                    //zapis wag do pliku pod warunkiem, ze minelo wiecej niz n-epok
                    if (epochCounter > 10 && !automatedRun)
                    {
                        NN.SaveWeights(weightsOutputFile);
                    }
                }

                if (Math.Abs(previousTMSE - trainingSetMSE) < DifferenceBetweenMSEs)
                {
                    if (++noImprovementInModelTimes >= MaxNoImprovementTimes)
                    {
                        trainingComplete = true;
                        Console.WriteLine("Brak zmian w modelu (x{0})", noImprovementInModelTimes);
                    }
                }
                else
                {
                    noImprovementInModelTimes = 0;
                }

                //zmieniamy zakres indeksu podzbioru dla zbioru trenujacego
                DatasetIndexes.IncreaseRange();
                if (epochCounter % 10 == 0)
                    PrintStatus();
            }
            saveResult.WriteLine("#ms.total_SpPdlaBP():\t{0}", timerA.ElapsedMilliseconds);
            saveResult.WriteLine("#ms.average():\t{0}\tlast:\t{1}",
                calcMeanDuration(durationOfEachEpoch), durationOfEachEpoch[epochCounter]);
            saveResult.WriteLine("#ticks.average():\t{0}\tlast:\t{1}",
                calcMeanDuration(durationInElapsedTicks), durationInElapsedTicks[epochCounter]);
            saveResult.Flush();
            saveResult.Close();
        }

        /// <summary>
        /// Pojedyncza epoka - dla calego podzbioru danych feedforward+backpropagation,
        /// wyliczenie aktualnego bledu oraz ustawienie nowych wag
        /// </summary>
        /// <param name="trainingSet">indeksy w zbiorze danych na ktorych trenujemy siec w tej epoce</param>
        private void runTrainingEpoch(int[] trainingSet)
        {
            for (int i = 0; i < trainingSet.Length; i++)
            {
                //wyliczamy wyjscia i propagujemy wstecz
                NN.FeedForward(Dataset.sample(trainingSet[i]));
                backpropagate(Dataset.target(trainingSet[i]));
            }

            //aktualizacja wag - zostaje wykonana na podstawie obecnych wartosci 
            //deltaInputHidden/deltaHiddenOutput wyliczanych w backpropagate()
            updateWeights();
        }

        /// <summary>
        /// Faza WSTECZ
        /// </summary>
        /// <param name="desiredOutputs">wartosc WY</param>
        private void backpropagate(double[] desiredOutputs)
        {
            if (!NN.IsLogistic)
            {
                //dla k-tej jednostki wyjsciowej
                for (int m = 0; m < NN.numOutput; m++)
                {
                    //gradient (kierunke spadku) dla wyjscia y_k*(1-y_k)*(d_k - y_k)
                    //outputErrorGradients[k] = getOutputErrorGradient(desiredOutputs[k], NN.OutputNeurons[k]);
                    outputErrorGradients[m] = (desiredOutputs[m] - NN.OutputNeurons[m]) 
                        * NN.DerivativeOfOutput(NN.OutputNets[m]);

                    //z j-tym neuronem warstwy ukrytej
                    for (int j = 0; j <= NN.numHidden; j++)
                    {
                        //zmiana(przysrost) wagi z reguly delta
                        deltaHiddenOutput[j][m] += learningRate * NN.HiddenNeurons[j] * outputErrorGradients[m];
                    }

                    //dla j-tego neuronu warstwy ukrytej
                    for (int h = 0; h < NN.numHidden; h++)
                    {
                        //gradient dla warstwy ukrytej:
                        //y_j*(1-y_j)*Suma(w_jk*delta(j)) //gdzie delta(j) jest gradientem warstwy WY
                        //hiddenErrorGradients[j] = getHiddenErrorGradient(j);
                        hiddenErrorGradients[h] = outputErrorGradients[m] * 
                            NN.wHiddenOutput[h][m] * NN.DerivativeOfHidden(NN.HiddenNets[h]);

                        //z i-tym neuronem warstwy WE
                        for (int n = 0; n <= NN.numInput; n++)
                        {
                            deltaInputHidden[n][h] += learningRate * NN.Inputs[n] * hiddenErrorGradients[h];
                        }
                    }
                }


            }
            else
            {
                //dla k-tej jednostki wyjsciowej
                for (int k = 0; k < NN.numOutput; k++)
                {
                    //gradient (kierunke spadku) dla wyjscia y_k*(1-y_k)*(d_k - y_k)
                    outputErrorGradients[k] = getOutputErrorGradient(desiredOutputs[k], NN.OutputNeurons[k]);

                    //z j-tym neuronem warstwy wejsciowej
                    for (int j = 0; j <= NN.numInput; j++)
                    {
                        //zmiana(przysrost) wagi z reguly delta
                        deltaInputHidden[j][k] += learningRate * NN.Inputs[j] * outputErrorGradients[k];
                    }
                }
            }
        }

        /// <summary>
        /// Aktualizacja wag
        /// </summary>
        private void updateWeights()
        {
            if (!NN.IsLogistic)
            {
                NN.wInputHidden = updateWeights(NN.wInputHidden, ref deltaInputHidden, NN.numInput, NN.numHidden);
                NN.wHiddenOutput = updateWeights(NN.wHiddenOutput, ref deltaHiddenOutput, NN.numHidden, NN.numOutput);
            }
            else
            {
                NN.wInputHidden = updateWeights(NN.wInputHidden, ref deltaInputHidden, NN.numInput, NN.numOutput);
            }
        }

        /// <summary>
        /// Aktualizacja wag na podstawie przeslanych argumentow,
        /// takze zerowanie wspolczynnikow delta
        /// </summary>
        /// <param name="weightsFromToLayer">wagi pomiedzy warstwami</param>
        /// <param name="deltaFromToLayer">delty wyliczane w fazie wstecznej propagacji</param>
        /// <param name="numFrom">liczba neuronow warstwy "od ktorej"</param>
        /// <param name="numTo">liczba neuronow warstwy "do ktorej" odnosimy sie ze zmianami</param>
        /// <returns>zmienione wagi</returns>
        private static double[][] updateWeights(double[][] weightsFromToLayer, ref double[][] deltaFromToLayer,
            int numFrom, int numTo)
        {
            //i-ty neuron layerFrom z j-tym layerTo
            for (int i = 0; i <= numFrom; i++)
            {
                for (int j = 0; j < numTo; j++)
                {
                    //aktualizuj wage na podstawie obliczonej delty w Backpropagate()
                    weightsFromToLayer[i][j] += deltaFromToLayer[i][j];
                    deltaFromToLayer[i][j] = new double(); //zerujemy delty
                }
            }

            return weightsFromToLayer;
        }

        /// <summary>
        /// Kalkulacja bledow dla WY
        /// </summary>
        /// <param name="desiredValue">wartosc docelowa d_k</param>
        /// <param name="outputValue">wartosc uzyskana y_k</param>
        /// <returns>Err_i = y_k(1-y_k)(d_k-y_k)</returns>
        private static double getOutputErrorGradient(double desiredValue, double outputValue)
        {
            return outputValue * (1 - outputValue) * (desiredValue - outputValue);
        }

        /// <summary>
        /// utworz nazwy plikow (zwiazane z typem danych)
        /// </summary>
        private void createFileNames(int holdout, string setSplitType)
        {
            resultsFileName = String.Format("wynik_{0}_BP-{5}g{1}-{2}-{3}-{4}.txt",
                Enum.GetName(typeof(EnumDataTypes), (int)Dataset.DataType), holdout.ToString(),
                NN.numHidden.ToString(), learningRate.ToString(), maxEpochs.ToString(), setSplitType);
            weightsOutputFile = String.Format("weights_{0}_BP-{1}.txt",
                Enum.GetName(typeof(EnumDataTypes), (int)Dataset.DataType),
                NN.numHidden.ToString());
            customWeightsOutputFile = String.Format("customWeights_{0}_BP-{1}.txt",
                Enum.GetName(typeof(EnumDataTypes), (int)Dataset.DataType),
                NN.numHidden.ToString());
        }

        /// <summary>
        /// Tworzy tablice dla wskazanego gradientu
        /// </summary>
        /// <param name="errorGradientsList">hiddenErrorGradient lub outputErrorGradient</param>
        /// <param name="num">Wskazana wielkosc</param>
        /// <returns>utworzona tablice</returns>
        private static double[] createErrorGradientStorage(double[] errorGradientsList, int num)
        {
            errorGradientsList = new double[num + 1];
            //for (int i = 0; i <= num; i++)
            //    errorGradientsList[i] = 0;
            return errorGradientsList;
        }

        /// <summary>
        /// Tworzy tablice dla wskazanych warstw potrzebnych w regule delta
        /// wyznaczajacej zmiany wag
        /// </summary>
        /// <param name="deltaFromToLayer">deltaInputHidden lub deltaHiddenOutput</param>
        /// <param name="numLayerFrom">wielkosc wartswy poprzedzajacej</param>
        /// <param name="numLayerTo">wielkosc warstwy docelowej</param>
        /// <returns>utworzona tablica</returns>
        private static double[][] createDeltaList(double[][] deltaFromToLayer, int numLayerFrom, int numLayerTo)
        {
            deltaFromToLayer = new double[numLayerFrom + 1][];
            for (int i = 0; i <= numLayerFrom; i++)
            {
                deltaFromToLayer[i] = new double[numLayerTo];
                //for (int j = 0; j < numLayerTo; j++) 
                //    deltaFromToLayer[i][j] = 0;
            }
            return deltaFromToLayer;
        }

        /// <summary>
        /// Kalkulacja bledow dla warstwy ukrytej,
        /// "The error calculation of the hidden neuron is based on the errors of the
        /// neurons in the subsequent layers and the associated weights"
        /// </summary>
        /// <param name="j">j-ty neuron warstwy ukrytej</param>
        /// <returns>Err_i = o_j(1-o_j)*Suma(Err_k*w_jk)</returns>
        private double getHiddenErrorGradient(int j)
        {
            //suma(w_ij * delta(j))
            double weightedSum = 0;
            for (int k = 0; k < NN.numOutput; k++)
            {
                weightedSum += NN.wHiddenOutput[j][k] * outputErrorGradients[k];
            }

            return NN.HiddenNeurons[j] * (1 - NN.HiddenNeurons[j]) * weightedSum;
        }

        /// <summary>
        /// Zapis pomiarow do pliku
        /// </summary>
        /// <param name="saveResult">uchwyt do pliku z wynikami</param>
        private void saveResultsToFile(TextWriter saveResult)
        {
            //pobieramy celnosc modelu i sume kwadratow bledu uczenia
            generalizationSetAccuracy = NN.GetAccuracy(Dataset, DatasetIndexes.GeneralizationSet);
            generalizationSetMSE = NN.CalcMSE(Dataset, DatasetIndexes.GeneralizationSet);

            trainingSetAccuracy = NN.GetAccuracy(Dataset, DatasetIndexes.TrainingSet);
            trainingSetMSE = NN.CalcMSE(Dataset, DatasetIndexes.TrainingSet);

            try
            {
                string line = String.Format("{0}\t{1:N4}\t{2:N2}\t{3:N4}\t{4:N2}\t{5}",
                    epochCounter, trainingSetMSE, trainingSetAccuracy, generalizationSetMSE,
                    generalizationSetAccuracy, durationOfEachEpoch[epochCounter]);
                line = line.Replace(",", ".");
                saveResult.WriteLine(line);

                saveResult.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("Nieudany zapis do pliku wyników:\n{0}", e.Message);
            }
        }

        /// <summary>
        /// pokazuje i obsluguje opcje podczas procesu nauczania
        /// </summary>
        public void ShowOptions()
        {
            if (automatedRun)
            {
                Console.WriteLine("Zautomatyzowany przebieg programu.");
            }
            else if (!trainingComplete)
            {
                Console.WriteLine("[Enter] by kontynuować, [1] Zapisac wagi, [2] Wczytac wagi, [3] Przerwac");
                int option;
                try
                {
                    option = Int32.Parse(Console.ReadLine());
                    string filename;
                    switch (option)
                    {
                        case 1:
                            Console.WriteLine("Podaj nazwę pliku, lub Enter dla domyślnej nazwy \"{0}\"",
                                customWeightsOutputFile);
                            filename = Console.ReadLine();
                            if (String.Equals(filename, ""))
                            {
                                Console.WriteLine("Nie podano nazwy, domyślna nazwa pliku (\"{0}\")",
                                    customWeightsOutputFile);
                                filename = customWeightsOutputFile;
                            }
                            NN.SaveWeights(customWeightsOutputFile);
                            ShowOptions();
                            break;
                        case 2:
                            Console.WriteLine("UWAGA! Wagi zostaną poprawnie wczytane "
                                + "jeżeli rozmiar sieci będzie taki sam jak w momencie zapisu wag.");
                            Console.WriteLine("Podaj nazwę pliku, lub Enter dla domyślnej nazwy \"{0}\"",
                                customWeightsOutputFile);
                            filename = Console.ReadLine();
                            if (String.Equals(filename, ""))
                            {
                                Console.WriteLine("Nie podano nazwy, domyślna nazwa pliku (\"{0}\")",
                                    customWeightsOutputFile);
                                filename = customWeightsOutputFile;
                            }
                            NN.LoadWeights(filename);
                            ShowOptions();
                            break;
                        case 3:
                            Console.WriteLine("Wybrano koniec nauczania sieci");
                            trainingComplete = true;
                            break;
                        default:
                            Console.WriteLine("Nie wybrano żadnej z powyższych opcji, kontynuować nauczanie sieci?"
                                + "[Enter] Kontynuuj\t [1] Pokaż menu");
                            try
                            {
                                option = Int32.Parse(Console.ReadLine());
                                if (option == 1)
                                    ShowOptions();
                            }
                            catch
                            {
                                Console.WriteLine("Kontynuuje...");
                            }
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Nie wybrano żadnej z powyższych opcji, kontynuować nauczanie sieci?"
                        + "\n[Enter] Kontynuuj\t [1] Pokaż menu");
                    try
                    {
                        option = Int32.Parse(Console.ReadLine());
                        if (option == 1)
                            ShowOptions();
                    }
                    catch
                    {
                        Console.WriteLine("Kontynuuje nauczanie...");
                    }
                }
            }
            else
            {
                Console.WriteLine("[Enter] by kontynuować, [1] Zapisac wagi");
                int option;
                try
                {
                    option = Int32.Parse(Console.ReadLine());
                    string filename;
                    switch (option)
                    {
                        case 1:
                            Console.WriteLine("Podaj nazwę pliku, lub Enter dla domyślnej nazwy \"{0}\"",
                                customWeightsOutputFile);
                            filename = Console.ReadLine();
                            if (String.Equals(filename, ""))
                            {
                                Console.WriteLine("Nie podano nazwy, domyślna nazwa pliku (\"{0}\")",
                                    customWeightsOutputFile);
                                filename = customWeightsOutputFile;
                            }
                            NN.SaveWeights(customWeightsOutputFile);
                            ShowOptions();
                            break;
                        default:
                            Console.WriteLine("Nie wybrano żadnej z powyższych opcji, kontynuować nauczanie sieci?"
                                + "[Enter] Kontynuuj\t [1] Pokaż menu");
                            try
                            {
                                option = Int32.Parse(Console.ReadLine());
                                if (option == 1)
                                    ShowOptions();
                            }
                            catch
                            {
                                Console.WriteLine("Kontynuuje nauczanie...");
                            }
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Nie wybrano żadnej z powyższych opcji, kontynuować nauczanie sieci?"
                        + "\n[Enter] Kontynuuj\t [1] Pokaż menu");
                    try
                    {
                        option = Int32.Parse(Console.ReadLine());
                        if (option == 1)
                            ShowOptions();
                    }
                    catch
                    {
                        Console.WriteLine("Kontynuuje nauczanie...");
                    }
                }
            }
        }

        /// <summary>
        /// Wyswietla aktualny status nauczania sieci
        /// </summary>
        public void PrintStatus()
        {
            Program.PrintLongLine();
            Console.Write("Epoka: {0} {3}ms  tMSE: {1:N4}\ttAcc: {2:N2}%",
                epochCounter, trainingSetMSE, trainingSetAccuracy, durationOfEachEpoch[epochCounter]);
            Console.WriteLine("\tgMSE: {0:N4}\t gAcc: {1:N2}%\t",
                generalizationSetMSE, generalizationSetAccuracy);
            Program.PrintLongLine();
        }

        /// <summary>
        /// Wyswietla podstawowe informacje o stanie sieci
        /// </summary>
        private void networkStats()
        {
            Console.WriteLine("Liczba neuronow w warstwie wejsciowej:\t{0}", NN.numInput);
            Console.WriteLine("Liczba neuronow w wartstwie ukrytej:\t{0}", NN.numHidden);
            Console.WriteLine("Liczba neuronow w warstwie wyjsciowej:\t{0}", NN.numOutput);
            Console.WriteLine("Parametry:");
            Console.WriteLine("Współczynnik uczenia:\t\t{0}", learningRate);
            Console.WriteLine("Maksymalna liczba epok:\t\t{0}", maxEpochs);
            Console.WriteLine("Docelowy MSE:\t{0}", desiredMSE);
            Console.WriteLine("Wielkość próbki treningowej: {0} \nRozmiar zbioru walidacyjnego: {1}\n",
                DatasetIndexes.TrainingSet.Length, DatasetIndexes.GeneralizationSet.Length);
        }

        /// <summary>
        /// Oblicza srednia trwania epok dla podanej tablicy
        /// </summary>
        /// <param name="durationHashtable">Hashtable durationOfEachEpoch</param>
        /// <returns>durationOfEachEpoch.Average()</returns>
        private static double calcMeanDuration(Hashtable durationOfEachEpoch)
        {
            long total = new long();

            foreach (long l in durationOfEachEpoch.Values)
            {
                total += l;
            }
            try
            {
                return (double)(total / durationOfEachEpoch.Count);
            }
            catch (DivideByZeroException e)
            {
                Debug.WriteLine("O epok? {0}", e.Message);
                return (double)total;
            }
        }
    }
}
