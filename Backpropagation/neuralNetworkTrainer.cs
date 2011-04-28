using System;
using System.IO;

namespace Backpropagation
{
    class neuralNetworkTrainer
    {

        protected const double DESIRED_MSE = 0.001;

        neuralNetwork NN;
        ZScore.ZScore Dataset;
        DatasetOperateGrowing DatasetIndexes;

        double learningRate;    //wspolczynnik uczenia wyznacza szybkosc z jaka zmieniaja sie wagi

        //liczniki epok
        ulong epochCounter;
        ulong maxEpochs;

        //wspolczynniki zmian wag dla reguly delta
        double[][] deltaInputHidden;
        double[][] deltaHiddenOutput;

        //error gradients
        double[] hiddenErrorGradients;
        double[] outputErrorGradients;

        //docelowa dokladnosc modelu
        double desiredAccuracy;
        //celnosc modelu dla kazdej z epok dla zbioru trenujacego i testujacego
        double trainingSetAccuracy;
        double generalizationSetAccuracy;

        //suma kwadratow bledu dla zbioru trenujacego i testujacego
        double trainingSetMSE;
        double generalizationSetMSE;

        readonly string RESULTOUTPUT;
        string weightsOutputFile; //TODO zapis wag do pliku o nazwie...

        public neuralNetworkTrainer(ZScore.ZScore dataset)
            : this(dataset, dataset.sample(0).Length - 1)
        { }

        public neuralNetworkTrainer(ZScore.ZScore dataset, int hiddenNodeRatio)
            :this (dataset, hiddenNodeRatio, 0.001, 1500, 99)
        { }

        public neuralNetworkTrainer(ZScore.ZScore dataset, int hiddenNodeRatio, double lr, ulong mE, double desiredAcc)
        {
            this.Dataset = dataset;

            NN = new neuralNetwork(Dataset.sample(0).Length, hiddenNodeRatio, 1, dataset.DataType);
            DatasetIndexes = new DatasetOperateGrowing(Dataset.Data[0].GetNum());

            RESULTOUTPUT = String.Format("wynik_{0}.txt", Enum.GetName(typeof(ZScore.EnumDataTypes), (int)dataset.DataType));

            maxEpochs = mE;
            learningRate = lr;

            desiredAccuracy = desiredAcc;
            trainingSetAccuracy = 0;
            generalizationSetAccuracy = 0;

            deltaInputHidden = createDeltaList(deltaInputHidden, NN.numInput, NN.numHidden);
            deltaHiddenOutput = createDeltaList(deltaHiddenOutput, NN.numHidden, NN.numOutput);

            hiddenErrorGradients = createErrorGradientStorage(hiddenErrorGradients, NN.numHidden);
            outputErrorGradients = createErrorGradientStorage(outputErrorGradients, NN.numOutput);

            Program.PrintInfo("Utworzona sieć neuronowa");
            neuronsNumberStats();
        }

        private static double[] createErrorGradientStorage(double[] errorGradientsList, int num)
        {
            errorGradientsList = new double[num + 1];
            for (int i = 0; i <= num; i++) 
                errorGradientsList[i] = 0;
            return errorGradientsList;
        }

        private static double[][] createDeltaList(double[][] deltaFromToLayer, int numLayerFrom, int numLayerTo)
        {
            deltaFromToLayer = new double[numLayerFrom + 1][];
            for (int i = 0; i <= numLayerFrom; i++)
            {
                deltaFromToLayer[i] = new double[numLayerTo];
                for (int j = 0; j < numLayerTo; j++) deltaFromToLayer[i][j] = 0;
            }
            return deltaFromToLayer;
        }

        private void neuronsNumberStats()
        {
            Console.WriteLine("Liczba neuronow w warstwie wejsciowej:\t{0}", NN.numInput);
            Console.WriteLine("Liczba neuronow w wartstwie ukrytej:\t{0}", NN.numHidden);
            Console.WriteLine("Liczba neuronow w warstwie wyjsciowej:\t{0}", NN.numOutput);
            Console.WriteLine("Parametry:");
            Console.WriteLine("Współczynnik uczenia:\t\t{0}", learningRate);
            Console.WriteLine("Maksymalna liczba epok:\t\t{0}", maxEpochs);
            Console.WriteLine("Docelowa dokładność modelu:\t{0}\n", desiredAccuracy);
        }

        public void trainNetwork()
        {
            //uchwyt do pliku w ktorym zapisujemy wyniki
            TextWriter saveResult = new StreamWriter(RESULTOUTPUT);

            //inicjalizacja licznika epok oraz wag
            epochCounter = 0;
            NN.initializeWeights();

            Console.Write("\nNaciśnij dowolny przycisk by rozpocząć nauczanie sieci...");
            Console.ReadKey();

            while ((trainingSetAccuracy < desiredAccuracy || generalizationSetAccuracy < desiredAccuracy) && epochCounter < maxEpochs)
            {

                //pojedyncza epoka
                runTrainingEpoch(DatasetIndexes.TrainingSet);

                //pobieramy celnosc modelu i sume kwadratow bledu uczenia
                generalizationSetAccuracy = NN.getAccuracy(Dataset, DatasetIndexes.GeneralizationSet);
                generalizationSetMSE = NN.calcMSE(Dataset, DatasetIndexes.GeneralizationSet);

                //zmieniamy zakres indeksu podzbioru dla zbioru trenujacego
                DatasetIndexes.IncreaseRange();

                //kolejna epoka, zwiekszamy licznik
                epochCounter++;

                Console.Write("Epoka: {0}\tTAcc: {1:N4}\tTMSE: {2:N4}", epochCounter, trainingSetAccuracy, trainingSetMSE);
                Console.WriteLine("\tGAcc: {0:N4}\t GMSE: {1:N4}", generalizationSetAccuracy, generalizationSetMSE);
                Program.PrintLongLine();
                if (epochCounter % 5 == 0)
                {
                    saveResult.WriteLine("E:{0}\t tTAcc: {1:N3}\tTMSE: {2:N3}\tGAcc: {3:N3}\t GMSE: {4:N3}", 
                        epochCounter, trainingSetAccuracy, trainingSetMSE, generalizationSetAccuracy, generalizationSetMSE);
                    saveResult.Flush();

                    Console.WriteLine("rozmiar tset: {0}", DatasetIndexes.TrainingSet.Length);
                    //TODO
                    Console.WriteLine("[Enter] by kontynuować, [S] by zapisac wagi, [W] by wczytac wagi, [Esc] by przerwac");
                    char option = (char)Console.Read();
                }
            }

            saveResult.Close();
        }

        void ShowOptions()
        {
            //TODO
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


        private static double getOutputErrorGradient(double desiredValue, double outputValue)
        {
            return outputValue * (1 - outputValue) * (desiredValue - outputValue);
        }

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

    }
}
