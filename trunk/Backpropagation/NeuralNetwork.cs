using System;
using System.IO;


namespace Backpropagation
{
    class neuralNetwork
    {
        public const int BIAS = 1;

        public int numInput { get; set; }
        public int numHidden { get; set; }
        public int numOutput { get; set; }

        public double[] inputNeurons { get; set; }
        public double[] hiddenNeurons { get; set; }
        public double[] outputNeurons { get; set; }

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

        public double[][] wInputHidden { get; set; }
        public double[][] wHiddenOutput { get; set; }

        public double[][] bestWeightInputHidden { get; set; }
        public double[][] bestWeightHiddenOutput { get; set; }

        public double[][] previousWeightsInputHidden { get; set; }
        public double[][] previousWeightsHiddenOutput { get; set; }

        private double decideOutputRangeA;
        private double decideOutputRangeB;

        bool inputHiddenActivationSigmoid, hiddenOutputActivationSigmoid;

        public neuralNetwork(int nInput, int nHidden, int nOutput, ZScore.EnumDataTypes dataType, 
            bool firstActivationFunc, bool secondActivationFunc)
        {
            numInput = nInput;
            numHidden = nHidden;
            numOutput = nOutput;

            inputNeurons = new double[numInput + 1];
            for ( int i=0; i < numInput; i++ ) inputNeurons[i] = 0;
            inputNeurons[numInput] = BIAS;

            hiddenNeurons = new double[numHidden + 1];
            for ( int i=0; i < numHidden; i++ ) hiddenNeurons[i] = 0;
            hiddenNeurons[numHidden] = BIAS;
//TODO spr. czy trzeba zerowac tu czy kompilator sam zeruje
            outputNeurons = new double[numOutput];
            for ( int i=0; i < numOutput; i++ ) outputNeurons[i] = 0;

            //net_ij
            hiddenNets = new double[numHidden + 1];
            for (int i = 0; i < numHidden; i++) hiddenNets[i] = 0;
            hiddenNets[numHidden] = BIAS;

            outputNets = new double[numOutput];
            for (int i = 0; i < numOutput; i++) outputNets[i] = 0;

	        wInputHidden = new double[numInput + 1][];
	        for ( int i=0; i <= numInput; i++ ) 
	        {
		        wInputHidden[i] = new double[numHidden];
		        for ( int j=0; j < numHidden; j++ ) wInputHidden[i][j] = 0;		
	        }

            wHiddenOutput = new double[numHidden + 1][];
	        for ( int i=0; i <= numHidden; i++ ) 
	        {
		        wHiddenOutput[i] = new double[numOutput];			
		        for ( int j=0; j < numOutput; j++ ) wHiddenOutput[i][j] = 0;		
	        }

            //zakresy klasyfikacji
            switch (dataType)
            {
                case ZScore.EnumDataTypes.unknown:
                    decideOutputRangeA = 0;
                    decideOutputRangeB = 0;
                    break;
                case ZScore.EnumDataTypes.HeartDisease:
                    decideOutputRangeA = 0.5;
                    decideOutputRangeB = 0.5;
                    break;
                case ZScore.EnumDataTypes.LetterRecognitionA:
                    decideOutputRangeA = 0.1;
                    decideOutputRangeB = 0.9;
                    break;
            }

            inputHiddenActivationSigmoid = firstActivationFunc;
            hiddenOutputActivationSigmoid = secondActivationFunc;
        }

        /// <summary>
        /// Oblicza celnosc predykcji modelu dla wskazanego podzbioru
        /// </summary>
        /// <param name="Dataset">caly zbior danych</param>
        /// <param name="setIndex">indeksy wyznaczajace podzbior</param>
        /// <returns>x% poprawnie sklasyfikowanych przypadkow</returns>
        public double getAccuracy(ZScore.ZScore Dataset, int[] setIndex)
        {
            double incorrectResults = 0;

            //wykonujemy dla wszystkich elementow podzbioru
            for (int i = 0; i < setIndex.Length; i++)
            {
                //wyliczamy WY dla kolejnych probek danych
                feedForward(Dataset.sample(setIndex[i]));

                //flaga informujaca czy wskazany przypadek zostal dobrze wyliczony przez siec
                bool correctResult = true;

                foreach(double actualVal in outputNeurons)
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
        public double calcMSE(ZScore.ZScore Dataset, int[] setIndex)
        {
            double mse = 0;

            //wykonujemy dla wszystkich elementow podzbioru
            for (int i = 0; i < setIndex.Length; i++)
            {
                //wyliczamy WY
                feedForward(Dataset.sample(setIndex[i]));

                //wyliczamy Set Mean Squared Error
                foreach(double actualVal in outputNeurons)
                    mse += Math.Pow((actualVal - Dataset.target(setIndex[i])), 2);
            }

            //procentowa wartosc bledu
            return mse / (numOutput * setIndex.Length);
        }

        /// <summary>
        /// Inicjalizacja wag
        /// </summary>
        public void initializeWeights()
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
                        //weightList[i][j] = 0.1;
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
        public void feedForward(double[] sample)
        {
            //przypisuje wartosci na WE (wejsciu)
            for (int i = 0; i < numInput; i++) inputNeurons[i] = sample[i];

            //obliczamy wartosci w hiddenLayer
            //hiddenNeurons = calculateOfOutputs(numInput, numHidden, inputNeurons, hiddenNeurons, wInputHidden, ref hiddenNets, true);

            //dla sigmoid 
            hiddenNeurons = calculateOfOutputs(numInput, numHidden, inputNeurons, hiddenNeurons, wInputHidden, 
                ref hiddenNets, inputHiddenActivationSigmoid);

            //wyliczamy wartosci na WY
            outputNeurons = calculateOfOutputs(numHidden, numOutput, hiddenNeurons, outputNeurons, wHiddenOutput, 
                ref outputNets, hiddenOutputActivationSigmoid);

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
            double[][] wWithinLayers, ref double[] currNets, bool useSigmoid)
        {
            for (int j = 0; j < nCurrentLayer; j++)
            {
                //clear value
                neuronsCurrentLayer[j] = 0;

                //get weighted sum of pattern and bias neuron
                for (int i = 0; i <= nLayerFrom; i++) 
                    neuronsCurrentLayer[j] += neuronsLayerFrom[i] * wWithinLayers[i][j];

                currNets[j] = neuronsCurrentLayer[j];

                //set to result of sigmoid
                if(useSigmoid)
                    neuronsCurrentLayer[j] = activationFunctionSigmoid(neuronsCurrentLayer[j]);
                else
                    neuronsCurrentLayer[j] = activationFunctionTanh(neuronsCurrentLayer[j]);
            }
            return neuronsCurrentLayer;
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
