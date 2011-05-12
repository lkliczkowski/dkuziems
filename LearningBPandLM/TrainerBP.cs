using System;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace LearningBPandLM
{
    class TrainerBP
    {
        #region parametry i zmienne algorytmu

        /// <summary>
        /// wspolczynnik uczenia wyznacza szybkosc z jaka zmieniaja sie wagi
        /// </summary>
        double learningRate;

        /// <summary>
        /// wspolczynniki zmian wag dla reguly delta dla wag pomiedzy warstwa WE a ukryta
        /// </summary>
        double[][] deltaInputHidden;
        /// <summary>
        /// wspolczynniki zmian wag dla reguly delta dla wag pomiedzy warstwa ukryta a WY
        /// </summary>
        double[][] deltaHiddenOutput;

        /// <summary>
        /// Wektory z bledami dla warstwy ukrytej i WY
        /// </summary>
        double[] hiddenErrorGradients, outputErrorGradients;

        #endregion

        #region parametry cyklu nauczania sieci

        /// <summary>
        /// liczniki epok
        /// </summary>
        ulong epochCounter;
        ulong maxEpochs;

        /// <summary>
        /// docelowa dokladnosc modelu
        /// </summary>
        double desiredAccuracy;

        /// <summary>
        /// celnosc modelu dla kazdej z epok dla zbioru trenujacego i testujacego
        /// </summary>
        double trainingSetAccuracy, generalizationSetAccuracy;

        /// <summary>
        /// suma kwadratow bledu dla zbioru trenujacego i testujacego
        /// </summary>
        double trainingSetMSE, generalizationSetMSE;

        /// <summary>
        /// domyślna docelowa wartosc MSE
        /// </summary>
        protected const double DEFAUT_DESIRED_MSE = 0.001;

        /// <summary>
        /// struktura sieci neuronowej (sieć MLP (multilayer perceptron networks))
        /// </summary>
        neuralNetwork NN;

        /// <summary>
        /// Zbior danych, klasa ZScore zawiera zstandaryzowane dane:
        /// Dataset.sample(i) - i-ta probka danych (WE) 
        /// Dataset.target(i) - i-ta wartosc docelowa (WY) dla i-tej probki
        /// </summary>
        ZScore.ZScore Dataset;

        /// <summary>
        /// Zbior indeksow wydzielajacych zbiory danych
        /// DatasetIndexes.TrainingSet - int[] dane trenujace (uczace)
        /// DatasetIndexes.GeneralizationSet - int[] dane testujace
        /// </summary>
        DatasetOperateWindowed DatasetIndexes;

        #endregion

        #region dodatkowe parametry

        /// <summary>
        /// nazwa pliku z wynikami pomiarów
        /// </summary>
        string RESULTS;

        /// <summary>
        /// nazwa pliku do ktorego zostana zapisane najlepsze wagi oraz wybrane (customWeightsOutputFile)
        /// </summary>
        string weightsOutputFile, customWeightsOutputFile;

        /// <summary>
        /// flaga czy kontynuowac nauczanie
        /// </summary>
        bool trainingComplete;

        /// <summary>
        /// domyslna wartosc wspolczynnika uczenia
        /// </summary>
        const double LEARNINGRATEDEFAULT = 0.1;

        Hashtable durationOfEachEpoch;
        Stopwatch timer;

        #endregion

        #region konstruktory

        public TrainerBP(ZScore.ZScore dataset)
            : this(dataset, dataset.sample(0).Length - 1)
        { }

        public TrainerBP(ZScore.ZScore dataset, int hiddenNodeRatio)
            :this (dataset, hiddenNodeRatio, LEARNINGRATEDEFAULT, 1500, 99)
        { }

        public TrainerBP(ZScore.ZScore dataset, int hiddenNodeRatio, double lr, ulong mE, double desiredAcc)
        {
            this.Dataset = dataset;

            ///ostatnie 2 argumenty - false, true dla funkcji aktywacji ktore maja byc tanh(x), true dla sigmoid
            NN = new neuralNetwork(Dataset.sample(0).Length, hiddenNodeRatio, 
                1, dataset.DataType, ActivationFuncType.Tanh, ActivationFuncType.Sigmoid);
            DatasetIndexes = new DatasetOperateWindowed(Dataset.NormalizedData[0].GetNum());

            createFileNames();
            durationOfEachEpoch = new Hashtable();
            timer = new Stopwatch();

            maxEpochs = mE;
            learningRate = lr;

            desiredAccuracy = desiredAcc;
            trainingSetAccuracy = 0;
            generalizationSetAccuracy = 0;

            deltaInputHidden = createDeltaList(deltaInputHidden, NN.numInput, NN.numHidden);
            deltaHiddenOutput = createDeltaList(deltaHiddenOutput, NN.numHidden, NN.numOutput);

            hiddenErrorGradients = createErrorGradientStorage(hiddenErrorGradients, NN.numHidden);
            outputErrorGradients = createErrorGradientStorage(outputErrorGradients, NN.numOutput);

            NN.initializeWeights();
            trainingComplete = false;

            Program.PrintInfo("Utworzono sieć neuronową");
            networkStats();
        }
        #endregion

        /// <summary>
        /// utworz nazwy plikow (zwiazane z typem danych)
        /// </summary>
        private void createFileNames()
        {
            RESULTS = String.Format("wynik_{0}_BP.txt", 
                Enum.GetName(typeof(ZScore.EnumDataTypes), (int)Dataset.DataType));
            weightsOutputFile = String.Format("weights_{0}_BP.txt", 
                Enum.GetName(typeof(ZScore.EnumDataTypes), (int)Dataset.DataType));
            customWeightsOutputFile = String.Format("customWeights_{0}_BP.txt", 
                Enum.GetName(typeof(ZScore.EnumDataTypes), (int)Dataset.DataType));
        }

        /// <summary>
        /// Funkcja nadzorujaca procesem uczenia sieci
        /// </summary>
        public void TrainNetwork()
        {
            //uchwyt do pliku w ktorym zapisujemy wyniki
            TextWriter saveResult = new StreamWriter(RESULTS);

            //inicjalizacja licznika epok oraz wag
            epochCounter = 0;
            
            trainingComplete = false;

            Console.Write("\nNaciśnij dowolny przycisk by rozpocząć nauczanie sieci...\n");
            Console.ReadKey();

            ShowOptions();

            //naglowki w pliku z wynikami
            saveResult.WriteLine("#LMtraining: learningRate {0}, NN: {1}(+1):{2}(+1):{3}",
                learningRate, NN.numInput, NN.numHidden, NN.numOutput);
            saveResult.WriteLine("#{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                "epoch", "tMSE", "tAcc", "gMSE", "gAcc", "ms");
            saveResult.Flush();

            saveResultsToFile(saveResult);

            PrintStatus();

            double bestGeneralizationSetMSE = generalizationSetMSE;

            while ((trainingSetAccuracy < desiredAccuracy || generalizationSetAccuracy < desiredAccuracy) 
                && epochCounter < maxEpochs && !trainingComplete)
            {

                //kolejna epoka, zwiekszamy licznik
                epochCounter++;


                timer.Restart();

                //pojedyncza epoka
                runTrainingEpoch(DatasetIndexes.TrainingSet);

                timer.Stop();
                durationOfEachEpoch.Add(epochCounter, timer.ElapsedMilliseconds);

                /* zapisujemy stan generalizationSetAccuracy by sprawdzic czy MSE sie zmniejszyl
                 * jezeli tak zapisujemy wagi do NN.bestWeights- */

                //zapisujemy wyniki do pliku
                saveResultsToFile(saveResult);

                //zachowujemy wagi dla najnizszego MSE dla zbioru testujacego
                if (bestGeneralizationSetMSE > generalizationSetMSE)
                {
                    bestGeneralizationSetMSE = generalizationSetMSE;
                    NN.bestWeightInputHidden = NN.wInputHidden;
                    NN.bestWeightHiddenOutput = NN.wHiddenOutput;
                    //zapis wag do pliku pod warunkiem, ze minelo wiecej niz n-epok
                    if (epochCounter > 10)
                    {
                        NN.SaveWeights(weightsOutputFile);
                    }
                }

                //zmieniamy zakres indeksu podzbioru dla zbioru trenujacego
                DatasetIndexes.IncreaseRange();

                PrintStatus();

                if (epochCounter % 10 == 0)
                {
                    //Console.WriteLine("średni czas: {0}", calcMeanDuration(durationOfEachEpoch));
                    //ShowOptions();
                }
            }
            saveResult.WriteLine("#ms.average(): {0}", calcMeanDuration(durationOfEachEpoch));
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
                NN.feedForward(Dataset.sample(trainingSet[i]));
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
        private void backpropagate(double desiredOutputs)
        {
            //dla k-tej jednostki wyjsciowej
            for (int k = 0; k < NN.numOutput; k++)
            {
                //gradient (kierunke spadku) dla wyjscia y_k*(1-y_k)*(d_k - y_k)
                outputErrorGradients[k] = getOutputErrorGradient(desiredOutputs, NN.OutputNeurons[k]);

                //z j-tym neuronem warstwy ukrytej
                for (int j = 0; j <= NN.numHidden; j++)
                {
                    //zmiana(przysrost) wagi z reguly delta
                    deltaHiddenOutput[j][k] += learningRate * NN.HiddenNeurons[j] * outputErrorGradients[k];
                }
            }

            //dla j-tego neuronu warstwy ukrytej
            for (int j = 0; j < NN.numHidden; j++)
            {
                //gradient dla warstwy ukrytej:
                //y_j*(1-y_j)*Suma(w_jk*delta(j)) //gdzie delta(j) jest gradientem warstwy WY
                hiddenErrorGradients[j] = getHiddenErrorGradient(j);

                //z i-tym neuronem warstwy WE
                for (int i = 0; i <= NN.numInput; i++)
                {
                    deltaInputHidden[i][j] += learningRate * NN.Inputs[i] * hiddenErrorGradients[j];
                }
            }
        }

        /// <summary>
        /// Aktualizacja wag
        /// </summary>
        private void updateWeights()
        {
            NN.wInputHidden = updateWeights(NN.wInputHidden, ref deltaInputHidden, NN.numInput, NN.numHidden);
            NN.wHiddenOutput = updateWeights(NN.wHiddenOutput, ref deltaHiddenOutput, NN.numHidden, NN.numOutput);
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
            generalizationSetAccuracy = NN.getAccuracy(Dataset, DatasetIndexes.GeneralizationSet);
            generalizationSetMSE = NN.calcMSE(Dataset, DatasetIndexes.GeneralizationSet);

            trainingSetAccuracy = NN.getAccuracy(Dataset, DatasetIndexes.TrainingSet);
            trainingSetMSE = NN.calcMSE(Dataset, DatasetIndexes.TrainingSet);

            try
            {
                saveResult.WriteLine("{0}\t{1:N4}\t{2:N2}\t{3:N4}\t{4:N2}\t{5}", 
                    epochCounter, trainingSetMSE, trainingSetAccuracy, generalizationSetMSE, 
                    generalizationSetAccuracy, durationOfEachEpoch[epochCounter]);

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
            if (!trainingComplete)
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
            Console.WriteLine("Docelowa dokładność modelu:\t{0}\n", desiredAccuracy);
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

            return (double)(total / durationOfEachEpoch.Count);
        }
    }
}
