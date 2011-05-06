using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ImprovedLM
{
    class ImprovedLMTrainer
    {
        #region parametry i zmienne algorytmu
        /// <summary>
        /// Całkowita liczba wag
        /// </summary>
        private int N;

        /// <summary>
        /// Quasi-Hessian matrix (przyblizona macierz Hessiana) NxN
        /// </summary>
        private double[][] matrixQ;

        /// <summary>
        /// Quasi-Hessian submatrix (przyblizona podmacierz Hessiana) NxN,
        /// sumujemy do Q pod koniec przeliczania kazdej probki danych
        /// </summary>
        private double[][] submatrixQ;

        /// <summary>
        /// Gradient vector Nx1
        /// </summary>
        private double[] gradientVectorG;

        /// <summary>
        /// Gradient subvector (podwektor gradientu) Nx1
        /// </summary>
        private double[] subvectorETA;

        /// <summary>
        /// podmatryca podmacierzy q 1xN
        /// </summary>
        private double[] vectorJ;

        /// <summary>
        /// macierz jednostkowa (identycznosciowa)
        /// </summary>
        //private double[][] matrixI;

        /// <summary>
        /// Główny parametr algorytmu, 
        /// wspolczynnik tlumienia, wykorzystywany w trakcie aktualizacji wag,
        /// najczesciej oznaczany jako \mi lub \lambda
        /// </summary>
        double coefficientMI;

        /// <summary>
        /// wspolczynnik przystosowania V - jezeli wspolczynnik tlumienia \mi ma zostac
        /// zwiekszony to mnozymy go przez V, jezeli zmniejszony to dzielimy go przez V
        /// </summary>
        double adjustmentFactorV;

        /// <summary>
        /// Wektor bledu, wyliczanyw dla kazdej probki w kazdej probce blad
        /// e = actual_output - desired
        /// </summary>
        double error;

        /// <summary>
        /// Wektor Nx1 wyznaczajacy rzyrost/zmiany wag
        /// </summary>
        private double[] deltaWeights;

        /// <summary>
        /// domyślna wartość startowa współczynnika "tłumienia"
        /// </summary>
        const double MI_DEFAULT = 0.00001;
        /// <summary>
        /// Maksymalna wielkość współczynnika \mi
        /// </summary>
        double MAX_MI = 100000;
        /// <summary>
        /// wspołczynnik przystosowania, wartosc domyślna
        /// </summary>
        const double V_DEFAULT = 10;

        #endregion

        #region parametry cyklu nauczania sieci

        /// <summary>
        /// sieć MLP (multilayer perceptron networks)
        /// </summary>
        private neuralNetwork NN;

        /// <summary>
        /// Licznik epok, informuje nas o aktualnej epoce
        /// </summary>
        ulong epochCounter;
        /// <summary>
        /// Maksymalna ilość epok
        /// </summary>
        ulong maxEpochs;

        /// <summary>
        /// Informacja ile razy nie udało się odwrócić macierzy podczas odliczania przyrostu wag,
        /// jeżeli próba nieudana zaliczamy ją do nieudanych prób aktualizacji wag i zwiększamy 
        /// współczynnik tłumienia dzięki czemu macierz przyjmuje inne wartości i wyznacznik 
        /// przestaje być zerowy gdyż zmieniają się wartości na przekątnej macierzy (Q+μI)
        /// </summary>
        int matrixInverseFailCounter;
        /// <summary>
        /// Maksymalna ilość powtórzeń próby odwrócenia macierzy, jeżeli matrixInverseFailCounter
        /// osiągnie tę wartość zostaje zainicjowana nowa epoka z kolejną próbką danych i domyślną
        /// wartością współczynnika μ
        /// </summary>
        const int MAX_MATRIX_FAIL_COUNT = 10;

        /// <summary>
        /// Główny licznik epok dla których zmiana wag z rzędu była nieudana, jeżeli zostanie osiągnięta 
        /// wartość MAX_TOTAL_EPOCH_FAIL_IN_A_ROW uczenie sieci dobiega końca gdyż wielokrotnie nie 
        /// zostały podjęte zmiany w modelu i najprawdopodobniej dalsze próby nie przyniosłyby efektu
        /// </summary>
        int totalFailEpochInARow;
        /// <summary>
        /// Maksymalna wielkość dla totalFailEpochInARow
        /// </summary>
        const int MAX_TOTAL_EPOCH_FAIL_IN_A_ROW = 10;

        /// <summary>
        /// Dodatkowy licznik kolejno po sobie następujących epok w trakcie których zmiana wag była 
        /// nieudana, jeżeli osiągnie maksymalną wartość, dane ze zbioru uczącego zostają przetasowane
        /// ponownie metodą DatasetIndexes.MixAgainTrainingData() i zostaje zainicjowana nowa epoka
        /// </summary>
        int additionalEpochFailCounter;
        /// <summary>
        /// Maksymalna ilość powtórzeń epok dla additionalEpochFailCounter
        /// </summary>
        const int MAX_ADD_EPOCH_FAIL_COUNT = 5;

        /// <summary>
        /// Zbior danych, klasa ZScore zawiera zstandaryzowane dane:
        /// Dataset.sample(i) - i-ta probka danych (WE) 
        /// Dataset.target(i) - i-ta wartosc docelowa (WY) dla i-tej probki
        /// </summary>
        private ZScore.ZScore Dataset;
        /// <summary>
        /// Zbior indeksow wydzielajacych zbiory danych
        /// DatasetIndexes.TrainingSet - int[] dane trenujace (uczace)
        /// DatasetIndexes.GeneralizationSet - int[] dane testujace
        /// </summary>
        private Backpropagation.DatasetOperateWindowed DatasetIndexes;

        /// <summary>
        /// docelowa dokladnosc modelu
        /// </summary>
        double desiredAccuracy;
        //TODO - desiredAcc

        /// <summary>
        /// celnosc modelu dla kazdej z epok dla zbioru trenujacego
        /// </summary>
        double trainingSetAccuracy;
        /// <summary>
        /// celnosc modelu dla kazdej z epok dla zbioru testujacego
        /// </summary>
        double generalizationSetAccuracy;

        /// <summary>
        /// suma kwadratow bledu dla zbioru trenujacego
        /// </summary>
        double trainingSetMSE;
        /// <summary>
        /// suma kwadratow bledu dla zbioru testujacego
        /// </summary>
        double generalizationSetMSE;

        #endregion

        #region dodatkowe parametry

        /// <summary>
        /// plik z wynikami dla trainingSetMSE, trainingSetAcc,
        /// generalizationSetMSE, generalizationSetAcc
        /// </summary>
        readonly string RESULTS;

        /// <summary>
        /// Przyrost - dla metody skonczonych roznic
        /// </summary>
        private double h = 0.00000000001;//Double.Epsilon - gdzies gubi precyzje w trakcie obliczen!

        /// <summary>
        /// Czy wyświtlać dodatkowe informacje o tym co jest przeliczane
        /// </summary>
        bool showAdditionalOutputMsgs = true;

        #endregion

        #region konstruktory
        public ImprovedLMTrainer(ZScore.ZScore dataset, int hiddenNodeRatio, ulong mE, double dAcc)
        {


            Dataset = dataset;
            DatasetIndexes = new Backpropagation.DatasetOperateWindowed(Dataset.Data[0].GetNum());

            NN = new neuralNetwork(Dataset.sample(0).Length, hiddenNodeRatio, 1, Dataset.DataType);


            RESULTS = String.Format("wynik_{0}_LM.txt",
                Enum.GetName(typeof(ZScore.EnumDataTypes), (int)Dataset.DataType));

            trainingSetMSE = generalizationSetMSE = double.MaxValue;
            maxEpochs = mE;
            desiredAccuracy = dAcc;

            coefficientMI = MI_DEFAULT;
            adjustmentFactorV = V_DEFAULT;

            N = (NN.numInput + 1) * NN.numHidden 
                + (NN.numHidden + 1) * NN.numOutput; //liczba wag = N
            vectorJ = new double[ N ];
            subvectorETA = new double[ N ];
            //error = new double[numTrainingPatternsInSingleEpoch];
            error = new double();
            submatrixQ = initArray(submatrixQ, N, N);
            initializeForNewEpoch(false);

            //matrixI = initArray(matrixI, N, N);
            //for (int i = 0; i < N; i++)
            //    matrixI[i][i] = 1;

            NN.initializeWeights();

        }

        #endregion
        public bool trainNetwork()
        {
            
            networkStats();

            TextWriter saveResult = new StreamWriter(RESULTS);

            epochCounter = 0;
            totalFailEpochInARow = 0;

            Console.Write("\nNaciśnij dowolny przycisk by rozpocząć nauczanie sieci...\n");
            Console.ReadKey();

            saveResult.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", "epoch", "tMSE", "tAcc", "gMSE", "gAcc");
            saveResult.Flush();

            //while ((trainingSetAccuracy < desiredAccuracy || generalizationSetAccuracy < desiredAccuracy) && epochCounter < maxEpochs)
            while (epochCounter < maxEpochs || coefficientMI > 10000 || coefficientMI < 0.00000000001)
            {
                //pojedyncza epoka
                if (runTrainingEpoch(DatasetIndexes.TrainingSet))
                {
                    //kolejna epoka, zwiekszamy licznik
                    epochCounter++;
                    totalFailEpochInARow = 0;

                    double previousGeneralizationSetMSE = generalizationSetAccuracy;

                    //pobieramy celnosc modelu i sume kwadratow bledu uczenia
                    generalizationSetAccuracy = NN.getAccuracy(Dataset, DatasetIndexes.GeneralizationSet);
                    generalizationSetMSE = NN.calcMSE(Dataset, DatasetIndexes.GeneralizationSet);

                    //zachowujemy wagi dla najnizszego MSE dla zbioru testujacego
                    if (previousGeneralizationSetMSE > generalizationSetAccuracy)
                    {
                        NN.bestWeightInputHidden = NN.wInputHidden;
                        NN.bestWeightHiddenOutput = NN.wHiddenOutput;
                    }

                    //zmieniamy zakres indeksu podzbioru dla zbioru trenujacego
                    DatasetIndexes.IncreaseRange();


                    PrintStatus();

                    saveResult.WriteLine("{0}\t{1:N4}\t{2:N2}\t{3:N4}\t{4:N2}", 
                        epochCounter, trainingSetMSE, trainingSetAccuracy, generalizationSetMSE, generalizationSetAccuracy);
                    saveResult.Flush();

                }
                else
                {
                    if (++totalFailEpochInARow > MAX_TOTAL_EPOCH_FAIL_IN_A_ROW)
                    {
                        Console.WriteLine("Osiagnieto maksymalna ilość epok bez zmian w modelu ({0})", MAX_TOTAL_EPOCH_FAIL_IN_A_ROW);
                        return false;
                    }

                    additionalEpochFailCounter++;

                    

                    Console.WriteLine("\nEpoka {0} niepowodzenie [{1}x z rzędu]! Powtarzam dla nastepnej probki danych!",
                        epochCounter + 1, totalFailEpochInARow);

                    if (coefficientMI > MAX_MI)
                    {
                        coefficientMI = MI_DEFAULT;
                    }
                    if (additionalEpochFailCounter > MAX_ADD_EPOCH_FAIL_COUNT - 1)
                    {
                        DatasetIndexes.MixAgainTrainingData();
                        initializeForNewEpoch(false);
                        Console.WriteLine("Dane przemieszane!");
                    }

                    //Console.ReadKey();
                }
            }
            saveResult.Close();
            return true;
        }


        private bool runTrainingEpoch(int[] trainingSet)
        {
            initializeForNewEpoch(true);
            //jezeli kwadrat sumy bledow sie zmniejszyl
            bool epochComplete = false;

            bool matrixInverseMsgShown = false;
            
            bool miMsgShown = false;

            while (!epochComplete)
            {
                if (coefficientMI > MAX_MI)
                    epochComplete = true;

                //niepoprawnie skasyfikowane przypadki (do wyznaczenia celnosci modelu)
                double incorrectPatterns = 0;
                double previousTrainingSetMSE = NN.calcMSE(Dataset, DatasetIndexes.GeneralizationSet);
                //double previousGeneralizationSetMSE = generalizationSetMSE;

                for (int i = 0; i < trainingSet.Length; i++)
                {

                    //wyliczamy wyjscia i propagujemy wstecz
                    NN.feedForward(Dataset.sample(trainingSet[i]));

                    //faza wstecz
                    backward(Dataset.target(trainingSet[i]), i);

                    //flaga informujaca czy wskazany przypadek zostal dobrze wyliczony przez siec
                    bool patternCorrect = true;

                    foreach (double actualVal in NN.outputNeurons)
                    {
                        if (NN.decideOutput(actualVal) != Dataset.target(trainingSet[i]))
                            patternCorrect = false;
                    }

                    //jezeli niepoprawnie sklasyfikowany zwieksz blad
                    if (!patternCorrect)
                        incorrectPatterns++;
                }

                //zachowujemy stan wag
                NN.previousWeightsInputHidden = NN.wInputHidden;
                NN.previousWeightsHiddenOutput = NN.wHiddenOutput;

                try
                {
                    if (updateWeights())
                    {
                        //aktualizuj wspolczynnik celnosci oraz blad dla zbioru uczacego
                        trainingSetAccuracy = 100 - (incorrectPatterns / trainingSet.Length * 100);
                        trainingSetMSE = NN.calcMSE(Dataset, trainingSet);

                        //jezeli kwadrat sumy bledow sie zmniejszyl
                        if (previousTrainingSetMSE > trainingSetMSE)// || generalizationSetMSE < previousGeneralizationSetMSE)
                        {
                            //iteracja sie konczy
                            epochComplete = true;

                            Console.WriteLine("\nWagi zostały zmienione dla μ: {0}", coefficientMI);
                            //zmniejszamy /mi
                            decMi();
                        }
                        else
                        {
                            //powtarzamy iteracje epochComplete == false
                            //zwiekszamy /mi
                            incMi();
                            //odrzucamy nowe wagi
                            NN.wInputHidden = NN.previousWeightsInputHidden;
                            NN.wHiddenOutput = NN.previousWeightsHiddenOutput;
                            trainingSetMSE = previousTrainingSetMSE;
                            //generalizationSetMSE = previousGeneralizationSetMSE;
                            if (!miMsgShown)
                            {
                                Console.WriteLine("\nNieudana zmiana wag, aktualna wartość współczynnika μ:");
                                miMsgShown = true;
                            }
                            Console.Write("\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b{0:N6}", coefficientMI);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (MatrixLibrary.MatrixDeterminentZero e)
                {
                    if (matrixInverseFailCounter > MAX_MATRIX_FAIL_COUNT - 1)
                    {
                        matrixInverseFailCounter = 0;
                        initializeForNewEpoch(true);
                        return false;
                    }

                    /* Dodatkowe informacje o niepowodzeniu w odwracaniu macierzy
                     * dla showAdditionalOutputMsgs == true
                     */
                    if (showAdditionalOutputMsgs) { 
                        if (!matrixInverseMsgShown)
                        {
                            Console.Write("Macierz osobliwa ({0}), biorę kolejną próbkę danych! Proba:  ", e.Message);
                            matrixInverseMsgShown = true;
                        }
                        Console.Write("\b{0}", matrixInverseFailCounter);
                    }

                    matrixInverseFailCounter++;
                    incMi();

                    if (coefficientMI > MAX_MI)
                    {
                        coefficientMI = MI_DEFAULT;
                    }

                    //return false;
                }
            }

            if (coefficientMI > MAX_MI)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Faza WSTECZ
        /// </summary>
        /// <param name="desiredOutputs"></param>
        /// <param name="sampleNum"></param>
        private void backward(double desiredOutputs, int sampleNum)
        {
            //odliczamy aktualny blad sieci dla wyjscia dla aktualnej probki
            error = desiredOutputs - NN.outputNeurons[0];
            //error[sampleNum] = desiredOutputs - NN.outputNeurons[0];


            //lista pochodnych dla funkcji bledu oraz sum wazonych 
            //D(e_ij)/D(net_ij)
            double[] s = new double[NN.numHidden + NN.numOutput];
            s[s.Length - 1] =  derivativeOfNets(NN.OutputNets[0]);
            for (int i = s.Length - 2; i >= 0; --i)
            {
                s[i] = derivativeOfNets(NN.HiddenNets[i]) * NN.wHiddenOutput[i][0] * (-s.Last());
            }

            //obliczamy wektor j
            int wIndex = 0;
            for (int i = 0; i < NN.numHidden; i++)
                for (int k = 0; k < NN.numInput + 1; k++)
                {
                    vectorJ[wIndex] = s[i] * NN.inputNeurons[k];
                    wIndex++;
                }

            for (int i = 0; i < NN.numOutput; i++)
                for (int k = 0; k < NN.numHidden + 1; k++)
                {
                    vectorJ[wIndex] = s.Last() * NN.hiddenNeurons[k];
                    wIndex++;
                }

            //obliczamy podmacierz q
            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < N; m++)
                {
                    submatrixQ[n][m] = vectorJ[m] * vectorJ[n];
                }
                //TODO macierz trojkatna pozwoli oszczedzic niemal polowe obliczen
            }

            //obliczamy podwektor ETA
            for (int i = 0; i < N; i++)
            {
                subvectorETA[i] = vectorJ[i] * error;
                //subvectorETA[i] = vectorJ[i] * error[sampleNum];
            }

            //Q = Q + q
            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < N; m++)
                {
                    matrixQ[n][m] = matrixQ[n][m] + submatrixQ[n][m]; 
                }
            }

            //g = g + eta
            for (int i = 0; i < N; i++)
            {
                gradientVectorG[i] = gradientVectorG[i] + subvectorETA[i];
            }
        }

        private bool updateWeights()
        {
            //zmiana wag = (Q + \mi*I)^{-1} * g
            double[][] delta = new double[N][];

            //delta = matrixQ;

            //Q + /mi * I
            for (int n = 0; n < N; n++)
            {
                matrixQ[n][n] += coefficientMI;
            }

            //(Q + /mi * I)^(-1)
            double[][] invertedMatrix = multidimensionalArrayToJaggedArray
                (MatrixLibrary.Matrix.Inverse(jaggedArrayToMultidimensionalArray(matrixQ)));

            delta = initArray(delta, N, N);

            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < N; m++)
                {
                    deltaWeights[n] += invertedMatrix[n][m] * gradientVectorG[m];
                }
            }

            int c = 0;
            for (int i = 0; i < NN.numInput + 1; i++)
            {
                for (int j = 0; j < NN.numHidden; j++)
                {
                    NN.wInputHidden[i][j] -= deltaWeights[c];
                    c++;
                }
            }

            for (int i = 0; i < NN.numHidden + 1; i++)
            {
                for (int j = 0; j < NN.numOutput; j++)
                {
                    NN.wHiddenOutput[i][j] -= deltaWeights[c];
                    c++;
                }
            }

            return true;
        }

        private void incMi()
        {
            coefficientMI *= adjustmentFactorV;
        }

        private void decMi()
        {
            coefficientMI /= adjustmentFactorV;
        }

        private void networkStats()
        {
            Console.WriteLine("Liczba neuronow w warstwie wejsciowej:\t{0}", NN.numInput);
            Console.WriteLine("Liczba neuronow w wartstwie ukrytej:\t{0}", NN.numHidden);
            Console.WriteLine("Liczba neuronow w warstwie wyjsciowej:\t{0}", NN.numOutput);
            Console.WriteLine("Parametry:");
            Console.WriteLine("Maksymalna liczba epok:\t\t{0}", maxEpochs);
            Console.WriteLine("Docelowa dokładność modelu:\t{0}\n", desiredAccuracy);
        }

        public void PrintStatus()
        {
            Program.PrintLongLine();
            Console.Write("Epoka: {0}\ttMSE: {1:N4}\ttAcc: {2:N2}%", epochCounter, trainingSetMSE, trainingSetAccuracy);
            Console.WriteLine("\tgMSE: {0:N4}\t gAcc: {1:N2}%\t", generalizationSetMSE, generalizationSetAccuracy);
            Program.PrintLongLine();
        }

        /// <summary>
        /// Inicializacja macierzy Q i wektora gradientu g,
        /// musza byc zerowane na poczatku kazdej epoki
        /// </summary>
        private void initializeForNewEpoch(bool check)
        {
            matrixQ = initArray(matrixQ, N, N);
            gradientVectorG = new double[N];
            deltaWeights = new double[N];
            if (!check)
            {
                additionalEpochFailCounter = 0;
            }
        }

        /// <summary>
        /// Pochodna z funkcji bledu e, metoda skonczonych roznic
        /// </summary>
        /// <param name="netIJ">suma wazona wejsc i wag</param>
        /// <returns>wartosc pochodnej w punkcie netIJ</returns>
        private double derivativeOfNets(double netIJ)
        {
            return -((activationFunctionTanh(netIJ + h) - activationFunctionTanh(netIJ)) / h);
        }

        /// <summary>
        /// Funkcja aktywacji sigmoidalna bipolarna
        /// </summary>
        /// <param name="x">argument</param>
        /// <returns>Tanh(x), wartosci w przedziale (-1,1)</returns>
        static double activationFunctionTanh(double x)
        {
            return Math.Tanh(x);
        }

        /// <summary>
        /// Funkcja w celu refaktoryzacji kodu, inicjuje tablice 2d
        /// </summary>
        /// <param name="array">tablica do zainicjowania</param>
        /// <param name="length">ilosc pozycji dla pierwszego wymiaru</param>
        /// <param name="num">ilosc pozycji dla drugiego wymiaru</param>
        /// <returns>zainicjowana tablica num-wymiarowa</returns>
        protected static double[][] initArray(double[][] array, int length, int num)
        {
            array = new double[length][];
            for (int i = 0; i < array.Length; i++)
                array[i] = new double[num];

            return array;
        }

        private static double[][] multidimensionalArrayToJaggedArray(double[,] mArray)
        {
            double[][] jArray = new double[mArray.GetLength(0)][];

            for (int n = 0; n < mArray.GetLength(0); n++)
            {
                jArray[n] = new double[mArray.GetLength(1)];
                for (int m = 0; m < mArray.GetLength(1); m++)
                {
                    jArray[n][m] = mArray[n, m];
                }
            }

            return jArray;
        }

        private static double[,] jaggedArrayToMultidimensionalArray(double[][] jArray)
        {
            double[,] mArray = new double[jArray.Length, jArray[0].Length];

            for (int n = 0; n < jArray.Length; n++)
            {
                for (int m = 0; m < jArray[0].Length; m++)
                {
                    mArray[n, m] = jArray[n][m];
                }
            }

            return mArray;
        }

        private static void printMatrix(double[][] matrix)
        {
            foreach (double[] dd in matrix)
            {
                foreach (double d in dd)
                    Console.Write("{0:N2}\t", d);
                Console.WriteLine();
            }
        }

        private static void printMatrix(double[] vector)
        {
            foreach (double d in vector)
                Console.Write("{0:N2}\t", d);
            Console.WriteLine();
        }

        private static void printMatrix(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    Console.Write("{0:N2}\t", matrix[i, j]);
                Console.WriteLine();
            }
        }
    }
}
