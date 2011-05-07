using System;
using System.IO;

namespace Backpropagation
{
    class BPlearning
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

        #endregion

        #region konstruktory

        public BPlearning(ZScore.ZScore dataset)
            : this(dataset, dataset.sample(0).Length - 1)
        { }

        public BPlearning(ZScore.ZScore dataset, int hiddenNodeRatio)
            :this (dataset, hiddenNodeRatio, 0.001, 1500, 99)
        { }

        public BPlearning(ZScore.ZScore dataset, int hiddenNodeRatio, double lr, ulong mE, double desiredAcc)
        {
            this.Dataset = dataset;

            ///ostatnie 2 argumenty - false, true dla funkcji aktywacji ktore maja byc tanh(x), true dla sigmoid
            NN = new neuralNetwork(Dataset.sample(0).Length, hiddenNodeRatio, 1, dataset.DataType, false, true);
            DatasetIndexes = new DatasetOperateWindowed(Dataset.Data[0].GetNum());

            createFileNames();

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

            Program.PrintInfo("Utworzona sieć neuronową");
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

            showOptions();

            //naglowki w pliku z wynikami
            saveResult.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", "epoch", "tMSE", "tAcc", "gMSE", "gAcc");
            saveResult.Flush();

            saveResultsToFile(saveResult);

            while ((trainingSetAccuracy < desiredAccuracy || generalizationSetAccuracy < desiredAccuracy) 
                && epochCounter < maxEpochs && !trainingComplete)
            {
                //pojedyncza epoka
                runTrainingEpoch(DatasetIndexes.TrainingSet);

                /* zapisujemy stan generalizationSetAccuracy by sprawdzic czy MSE sie zmniejszyl
                 * jezeli tak zapisujemy wagi do NN.bestWeights- */
                double previousGeneralizationSetMSE = generalizationSetAccuracy;

                //zapisujemy wyniki do pliku
                saveResultsToFile(saveResult);

                //zachowujemy wagi dla najnizszego MSE dla zbioru testujacego
                if (previousGeneralizationSetMSE > generalizationSetAccuracy)
                {
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

                //kolejna epoka, zwiekszamy licznik
                epochCounter++;

                //zapisujemy wyniki do pliku
                saveResult.WriteLine("{0}\t{1:N4}\t{2:N2}\t{3:N4}\t{4:N2}", 
                    epochCounter, trainingSetMSE, trainingSetAccuracy, generalizationSetMSE, generalizationSetAccuracy);
                saveResult.Flush();

                PrintStatus();

                if (epochCounter % 10 == 0)
                {
                    showOptions();
                }
            }

            saveResult.Close();
        }

        /// <summary>
        /// Pojedyncza epoka - dla calego podzbioru danych feedforward+backpropagation,
        /// wyliczenie aktualnego bledu oraz ustawienie nowych wag
        /// </summary>
        /// <param name="trainingSet">indeksy w zbiorze danych na ktorych trenujemy siec w tej epoce</param>
        private void runTrainingEpoch(int[] trainingSet)
        {
            //niepoprawnie skasyfikowane przypadki (do wyznaczenia celnosci modelu)
            double incorrectPatterns = 0;
            //suma błędu "mean squared error"
            double mse = 0; 

            for (int i = 0; i < trainingSet.Length; i++)
            {
                //wyliczamy wyjscia i propagujemy wstecz
                NN.feedForward(Dataset.sample(trainingSet[i]));
                backpropagate(Dataset.target(trainingSet[i]));

                //flaga informujaca czy wskazany przypadek zostal dobrze wyliczony przez siec
                bool patternCorrect = true;

                foreach (double actualVal in NN.outputNeurons)
                {
                    if (NN.decideOutput(actualVal) != Dataset.target(trainingSet[i])) 
                        patternCorrect = false;

                    //wylicz sume kwadratow bledu dla wskazanych
                    mse += Math.Pow((actualVal - Dataset.target(trainingSet[i])), 2);
                }

                //jezeli niepoprawnie sklasyfikowany zwieksz blad
                if (!patternCorrect) 
                    incorrectPatterns++;

            }

            //aktualizacja wag - zostaje wykonana na podstawie obecnych wartosci 
            //deltaInputHidden/deltaHiddenOutput wyliczanych w backpropagate()
            updateWeights();

            //aktualizuj wspolczynnik celnosci oraz blad dla zbioru uczacego
            trainingSetAccuracy = 100 - (incorrectPatterns / trainingSet.Length * 100);
            trainingSetMSE = mse / (NN.numOutput * trainingSet.Length);
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
                outputErrorGradients[k] = getOutputErrorGradient(desiredOutputs, NN.outputNeurons[k]);

                //z j-tym neuronem warstwy ukrytej
                for (int j = 0; j <= NN.numHidden; j++)
                {
                    //zmiana(przysrost) wagi z reguly delta
                    deltaHiddenOutput[j][k] += learningRate * NN.hiddenNeurons[j] * outputErrorGradients[k];
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
                    deltaInputHidden[i][j] += learningRate * NN.inputNeurons[i] * hiddenErrorGradients[j];
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

            return NN.hiddenNeurons[j] * (1 - NN.hiddenNeurons[j]) * weightedSum;
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
                saveResult.WriteLine("{0}\t{1:N4}\t{2:N2}\t{3:N4}\t{4:N2}",
                    epochCounter, trainingSetMSE, trainingSetAccuracy, generalizationSetMSE, generalizationSetAccuracy);

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
        private void showOptions()
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
                        showOptions();
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
                        showOptions();
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
                                showOptions();
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
                        showOptions();
                }
                catch
                {
                    Console.WriteLine("Kontynuuje nauczanie...");
                }
            }
        }

        /// <summary>
        /// Wyswietla aktualny status nauczania sieci
        /// </summary>
        public void PrintStatus()
        {
            Program.PrintLongLine();
            Console.Write("Epoka: {0}\ttMSE: {1:N4}\ttAcc: {2:N2}%", epochCounter, trainingSetMSE, trainingSetAccuracy);
            Console.WriteLine("\tgMSE: {0:N4}\t gAcc: {1:N2}%\t", generalizationSetMSE, generalizationSetAccuracy);
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

    }
}
