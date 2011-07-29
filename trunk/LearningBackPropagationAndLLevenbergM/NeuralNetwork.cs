﻿using System;
using System.IO;


namespace LearningBPandLM
{
    public enum ActivationFuncType
    {
        Linear,
        Sigmoid,
        Tanh
    }

    class neuralNetwork
    {
        public const int BIAS = 1;

        public int numInput { get; set; }
        public int numHidden { get; set; }
        public int numOutput { get; set; }

        public double[] Inputs { get; set; }
        public double[] HiddenNeurons { get; set; }
        public double[] OutputNeurons { get; set; }

        /// <summary>
        /// net_ij - przechowuje wartosci sum wazonych,
        /// odwoluje sie do nich algorytm LM
        /// </summary>
        private double[] hiddenNets;
        public double[] HiddenNets { get { return hiddenNets; } }

        /// <summary>
        /// net_ij - przechowuje wartosc sumy wazonej dla WY,
        /// odwoluje sie do niej algorytm LM
        /// </summary>
        private double[] outputNets;
        public double[] OutputNets { get { return outputNets; } }

        /// <summary>
        /// Wagi pomiedzy We a warstwa ukryta
        /// </summary>
        public double[][] wInputHidden { get; set; }

        /// <summary>
        /// Wagi pomiedzy warstwa ukryta a WY
        /// </summary>
        public double[][] wHiddenOutput { get; set; }

        /// <summary>
        /// Wagi input-hidden dla najlepszego generalizationSetMSE
        /// </summary>
        public double[][] BestWeightInputHidden { get; set; }
        /// <summary>
        /// Wagi hidden-output dla najlepszego generalizationSetMSE
        /// </summary>
        public double[][] BestWeightHiddenOutput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private double[][] previousWeightsInputHidden { get; set; }
        private double[][] previousWeightsHiddenOutput { get; set; }

        private double decideOutputRangeA;
        private double decideOutputRangeB;

        private ActivationFuncType firstActivationIsSigmoid, secondActivationIsSigmoid;

        public neuralNetwork(int nInput, int nHidden, int nOutput, ZScore.EnumDataTypes dataType,
            ActivationFuncType firstActivationFunc, ActivationFuncType secondActivationFunc)
        {
            numInput = nInput;
            numHidden = nHidden;
            numOutput = nOutput;

            Inputs = new double[numInput + 1];
            Inputs[numInput] = BIAS;

            HiddenNeurons = new double[numHidden + 1];
            HiddenNeurons[numHidden] = BIAS;
            OutputNeurons = new double[numOutput];

            //net_ij
            hiddenNets = new double[numHidden + 1];
            hiddenNets[numHidden] = BIAS;

            outputNets = new double[numOutput];

	        wInputHidden = new double[numInput + 1][];
	        for ( int i=0; i <= numInput; i++ ) 
	        {
		        wInputHidden[i] = new double[numHidden];
	        }

            wHiddenOutput = new double[numHidden + 1][];
	        for ( int i=0; i <= numHidden; i++ ) 
	        {
		        wHiddenOutput[i] = new double[numOutput];			
	        }

            //zakresy klasyfikacji, sluza tylko do okreslania Accuracy, nie wplywaja na MSE
            switch (dataType)
            {
                case ZScore.EnumDataTypes.HeartDisease:
                case ZScore.EnumDataTypes.GermanCreditData:
                case ZScore.EnumDataTypes.CreditRisk:
                default:
                    decideOutputRangeA = 0.5;
                    decideOutputRangeB = 0.5;
                    break;
                case ZScore.EnumDataTypes.LetterRecognitionA:
                    decideOutputRangeA = 0.2;//0.1;
                    decideOutputRangeB = 0.8;//0.9;
                    break;
            }

            firstActivationIsSigmoid = firstActivationFunc;
            secondActivationIsSigmoid = secondActivationFunc;
        }

        /// <summary>
        /// Oblicza celnosc predykcji modelu dla wskazanego podzbioru
        /// </summary>
        /// <param name="Dataset">caly zbior danych</param>
        /// <param name="setIndex">indeksy wyznaczajace podzbior</param>
        /// <returns>x% poprawnie sklasyfikowanych przypadkow</returns>
        public double GetAccuracy(ZScore.ZScoreData Dataset, int[] setIndex)
        {
            double incorrectResults = 0;

            //wykonujemy dla wszystkich elementow podzbioru
            for (int i = 0; i < setIndex.Length; i++)
            {
                //wyliczamy WY dla kolejnych probek danych
                FeedForward(Dataset.sample(setIndex[i]));

                //flaga informujaca czy wskazany przypadek zostal dobrze wyliczony przez siec
                bool correctResult = true;

                foreach(double actualVal in OutputNeurons)
                {
                    if (decideOutput(actualVal) != Dataset.target(setIndex[i])) 
                        correctResult = false;
                    //Console.WriteLine("{0:N2} vs {1:N2}", actualVal, Dataset.target(setIndex[i])); 
                }
                //jezeli niepoprawnie sklasyfikowany zwieksz blad
                if (!correctResult) incorrectResults++;
            }

            //calculate error and return as percentage
            return 100 - (incorrectResults / setIndex.Length * 100);
        }

        /// <summary>
        /// Suma kwadratow bledow uczenia
        /// </summary>
        /// <param name="Dataset">zbior</param>
        /// <param name="setIndex">zbior ideksow wyznaczajacy podzbior</param>
        /// <returns>(Sum(desired-actual)^2)</returns>
        public double CalcMSE(ZScore.ZScoreData Dataset, int[] setIndex)
        {
            double mse = 0;

            //wykonujemy dla wszystkich elementow podzbioru
            for (int i = 0; i < setIndex.Length; i++)
            {
                //wyliczamy WY
                FeedForward(Dataset.sample(setIndex[i]));

                //wyliczamy Set Mean Squared Error
                foreach(double actualVal in OutputNeurons)
                    mse += Math.Pow((actualVal - Dataset.target(setIndex[i])), 2);
            }

            //procentowa wartosc bledu
            return mse / (numOutput * setIndex.Length);
        }

        /// <summary>
        /// Inicjalizacja wag
        /// </summary>
        public void InitializeWeights()
        {
            wInputHidden = initRandomWeights(numInput, numHidden, wInputHidden);
            wHiddenOutput = initRandomWeights(numHidden, numOutput, wHiddenOutput);
        }

        /// <summary>
        /// Inicjuje tablice wag miedzy podanymi warstwami,
        /// ostatnia waga w liscie to BIAS
        /// </summary>
        /// <param name="weightList">tablica wag do wpisania</param>
        /// <param name="numLayerFrom">ilosc neuronow w wartwie pierwszej</param>
        /// <param name="numLayerTo">ilosc neuronow w warstwie drugiej</param>
        /// <returns>zainicjowana lista wag pomiedzy warstwami</returns>
        private static double[][] initRandomWeights(int numLayerFrom, int numLayerTo, double[][] weightList)
        {
            Random r = new Random();

            for (int i = 0; i < numLayerFrom + 1; i++)
            {
                for (int j = 0; j < numLayerTo; j++)
                {
                    if (i != numLayerFrom)
                    {
                        //losowe wagi
                        weightList[i][j] = (r.NextDouble() * 2 - 1); // (-1.1)
                        //weightList[i][j] = (r.NextDouble() * 1 - 0.5); // (-0.5,0.5)
                    }
                    else
                    {
                        //waga dla neuronu BIAS
                        weightList[i][j] = BIAS;
                    }
                }
            }
            return weightList;
        }

        /// <summary>
        /// Funkcja aktywacji sigmoidalna unipolarna
        /// </summary>
        /// <param name="x">argument</param>
        /// <returns>1/(1+e^(-x)), wartosci w przedziale (0,1)</returns>
        public static double activationFunctionSigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        /// <summary>
        /// Funkcja aktywacji sigmoidalna bipolarna
        /// </summary>
        /// <param name="x">argument</param>
        /// <returns>Tanh(x), wartosci w przedziale (-1,1)</returns>
        public static double activationFunctionTanh(double x)
        {
            return Math.Tanh(x);
        }

        /// <summary>
        /// Decyduje jakiemu wynikowi przypisac wartosc
        /// </summary>
        /// <param name="x">wyliczona wartosc</param>
        /// <returns>wartosc wynikowa (0/1 lub -1 gdy poza zakresem)</returns>
        public int decideOutput(double x)
        {
            if (x < decideOutputRangeA) return 0;
            else if (x >= decideOutputRangeB) return 1;
            else return -1;
        }

        /// <summary>
        /// Faza W PRZÓD
        /// </summary>
        /// <param name="sample">próbka danych</param>
        public void FeedForward(double[] sample)
        {
            //przypisuje wartosci na WE (wejsciu)
            for (int i = 0; i < numInput; i++) Inputs[i] = sample[i];

            //obliczamy wartosci w hiddenLayer
            //hiddenNeurons = calculateOfOutputs(numInput, numHidden, inputNeurons, hiddenNeurons, wInputHidden, ref hiddenNets, true);

            //dla ukrytej
            HiddenNeurons = calculateOfOutputs(numInput, numHidden, Inputs, HiddenNeurons, wInputHidden, 
                ref hiddenNets, firstActivationIsSigmoid);

            //wyliczamy wartosci na WY
            OutputNeurons = calculateOfOutputs(numHidden, numOutput, HiddenNeurons, OutputNeurons, wHiddenOutput, 
                ref outputNets, secondActivationIsSigmoid);

        }
        
        /// <summary>
        /// Wylicza wartosci w sieci dla wskazanej tablicy danych i wybranych warstw
        /// </summary>
        /// <param name="nLayerFrom">liczba neuronow warstwy z ktorej przychodza sygnaly</param>
        /// <param name="nCurrentLayer">liczba neuronow wartwy dla ktorej wyliczamy wartosci</param>
        /// <param name="neuronsLayerFrom">warstwa z ktorej przychodza sygnaly</param>
        /// <param name="neuronsCurrentLayer">warstwa docelowa</param>
        /// <param name="wWithinLayers">wagi pomiedzy warstwami</param>
        /// <param name="useSigmoid">okresla funkcje aktywacji (true dla sigmoid, false dla Tanh)</param>
        /// <returns>obliczone wartosci dla neuronsCurrentLayer</returns>
        private static double[] calculateOfOutputs
            (int nLayerFrom, int nCurrentLayer, double[] neuronsLayerFrom, double[] neuronsCurrentLayer,
            double[][] wWithinLayers, ref double[] currNets, ActivationFuncType useSigmoid)
        {
            for (int j = 0; j < nCurrentLayer; j++)
            {
                //clear value
                neuronsCurrentLayer[j] = 0;

                //get weighted sum of pattern and bias neuron
                for (int i = 0; i <= nLayerFrom; i++) 
                    neuronsCurrentLayer[j] += neuronsLayerFrom[i] * wWithinLayers[i][j];

                currNets[j] = neuronsCurrentLayer[j];

                switch(useSigmoid)
                {
                    case ActivationFuncType.Sigmoid:
                        neuronsCurrentLayer[j] = activationFunctionSigmoid(neuronsCurrentLayer[j]);
                        break;
                    case ActivationFuncType.Tanh:
                        neuronsCurrentLayer[j] = activationFunctionTanh(neuronsCurrentLayer[j]);
                        break;
                    case ActivationFuncType.Linear:
                    default:
                        break;
                }
            }
            return neuronsCurrentLayer;
        }

        public void KeepWeightsToPrevious()
        {
            previousWeightsInputHidden = wInputHidden;
            previousWeightsHiddenOutput = wHiddenOutput;
        }

        public void RestoreWeightsWithPrevious()
        {
            wInputHidden = previousWeightsInputHidden;
            wHiddenOutput = previousWeightsHiddenOutput;
        }

        public void PrintWeights()
        {
            
            Console.WriteLine("weights input(+bias):hidden ({0}:{1})", numInput + 1, numHidden);
            foreach (double[] dd in wInputHidden)
            {
                foreach (double d in dd)
                {
                    Console.WriteLine("{0}", d);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("weights hidden(+bias):output ({0}:{1})", numHidden + 1, numOutput);
            foreach (double[] dd in wHiddenOutput)
            {
                foreach (double d in dd)
                {
                    Console.WriteLine("{0}", d);
                }
                Console.WriteLine();

            }
        }

        /// <summary>
        /// Wczytuje wagi z pliku  nazwie filename
        /// </summary>
        /// <param name="filename">nazwa pliku z wagami</param>
        public void LoadWeights(string filename)
        {
            try
            {
                using (StreamReader readFile = new StreamReader(filename))
                {
                    string line;
                    int i = 0, j = 0;
                    bool inputHidden = false;
                    bool hiddenOutput = false;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        if (line.Contains("input") && line.Contains("hidden"))
                        {
                            inputHidden = true;
                        }

                        while (inputHidden)
                        {
                            line = readFile.ReadLine();
                            if (line.Contains("hidden") && line.Contains("output"))
                            {
                                i = j = 0;
                                hiddenOutput = true;
                                inputHidden = false;
                            }
                            else if (line.Equals(""))
                            {
                                i++;
                                j = 0;
                            }
                            else
                            {
                                double d = Double.Parse(line);
                                wInputHidden[i][j] = d;
                                j++;
                            }
                        }

                        while (hiddenOutput)
                        {
                            line = readFile.ReadLine();
                            if (line == null)
                            {
                                hiddenOutput = false;
                                inputHidden = false;
                                Console.WriteLine("Wczytywanie wag zakończone sukcesem!");
                            }
                            else if (line.Equals(""))
                            {
                                i++;
                                j = 0;
                            }
                            else
                            {
                                double d = Double.Parse(line);
                                wHiddenOutput[i][j] = d;
                                j++;
                            }
                        }
                    }
                    readFile.Close();
                }
            }
            catch
            {
                Console.WriteLine("Niepowodzenie!");
            }
            Console.WriteLine("[Enter] by kontynuować...");
            Console.ReadKey();
        }

        /// <summary>
        /// Zapisuje wagi do pliku filename
        /// </summary>
        public void SaveWeights(string filename)
        {
            TextWriter saveWeights = new StreamWriter(filename);
            saveWeights.WriteLine("weights input(+bias):hidden ({0}:{1})", numInput + 1, numHidden);
            foreach (double[] dd in wInputHidden)
            {
                foreach (double d in dd)
                {
                    saveWeights.WriteLine("{0}", d);
                }
                saveWeights.WriteLine();
                saveWeights.Flush();
            }
            saveWeights.WriteLine();
            saveWeights.WriteLine("weights hidden(+bias):output ({0}:{1})", numHidden + 1, numOutput);
            foreach (double[] dd in wHiddenOutput)
            {
                foreach (double d in dd)
                {
                    saveWeights.WriteLine("{0}", d);
                }
                saveWeights.WriteLine();
                saveWeights.Flush();
            }
            saveWeights.Close();
            Console.WriteLine("Zapisano wagi!");
        }

    }
}
