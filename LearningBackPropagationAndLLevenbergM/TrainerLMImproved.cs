using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections;
using MatrixLibrary;

namespace LearningBPandLM
{
    class TrainerLMImproved
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
        /// Główny parametr algorytmu, 
        /// wspolczynnik tlumienia, wykorzystywany w trakcie aktualizacji wag,
        /// najczesciej oznaczany jako \mi lub \lambda
        /// </summary>
        private double coefficientMI;

        /// <summary>
        /// wspolczynnik przystosowania V - jezeli wspolczynnik tlumienia \mi ma zostac
        /// zwiekszony to mnozymy go przez V, jezeli zmniejszony to dzielimy go przez V
        /// </summary>
        private double adjustmentFactorV;

        /// <summary>
        /// Wektor bledu, wyliczanyw dla kazdej probki w kazdej probce blad
        /// e = actual_output - desired
        /// </summary>
        private double error;

        /// <summary>
        /// Wektor Nx1 wyznaczajacy przyrost/zmiany wag
        /// </summary>
        private double[] deltaWeights;

        /// <summary>
        /// Maksymalna wielkość współczynnika \mi
        /// </summary>
        private const double MaxMiCoefficient = 100000;
        /// <summary>
        /// Minimalna wielkość współczynnika \mi
        /// </summary>
        private const double MinMiCoefficient = 0.000000001;
        /// <summary>
        /// domyślna wartość startowa współczynnika "tłumienia"
        /// </summary>
        private double defaultMi;
        /// <summary>
        /// wspołczynnik przystosowania, wartosc domyślna
        /// </summary>
        private const double VDefault = 10;
        #endregion

        #region parametry cyklu nauczania sieci

        /// <summary>
        /// sieć MLP (multilayer perceptron networks)
        /// </summary>
        private neuralNetwork NN;

        /// <summary>
        /// Licznik epok, informuje nas o aktualnej epoce
        /// </summary>
        private ulong epochCounter;
        /// <summary>
        /// Maksymalna ilość epok
        /// </summary>
        private ulong maxEpochs;

        /// <summary>
        /// Zbior danych, klasa ZScore zawiera zstandaryzowane dane:
        /// Dataset.sample(i) - i-ta probka danych (WE) 
        /// Dataset.target(i) - i-ta wartosc docelowa (WY) dla i-tej probki
        /// </summary>
        private ZScore.ZScoreData Dataset;

        /// <summary>
        /// Zbior indeksow wydzielajacych zbiory danych
        /// DatasetIndexes.TrainingSet - int[] dane trenujace (uczace)
        /// DatasetIndexes.GeneralizationSet - int[] dane testujace
        /// </summary>
        private DatasetStructure DatasetIndexes;

        /// <summary>
        /// celnosc modelu dla kazdej z epok dla zbioru trenujacego
        /// </summary>
        private double trainingSetAccuracy;
        /// <summary>
        /// celnosc modelu dla kazdej z epok dla zbioru testujacego
        /// </summary>
        private double generalizationSetAccuracy;

        /// <summary>
        /// suma kwadratow bledu dla zbioru trenujacego
        /// </summary>
        private double trainingSetMSE;
        /// <summary>
        /// suma kwadratow bledu dla zbioru testujacego
        /// </summary>
        private double generalizationSetMSE;
        /// <summary>
        /// Wartosci MSE uzyskane w poprzednich epokach
        /// </summary>
        private double previousTrainingSetMSE, previousGeneralizationSetMSE;

        /// <summary>
        /// docelowa dokladnosc modelu
        /// </summary>
        private double desiredMSE;

        /// <summary>
        /// Okresla sposob postepowania z odwracaniem macierzy osobliwych
        /// </summary>
        private readonly SingularMatrixProceeding ProceedingWithSingularMatrix;

        /// <summary>
        /// Domyslne zachowanie podczas proby odwrocenia macierzy osobliwej
        /// </summary>
        private const SingularMatrixProceeding DefaultProceedingWithSingularMatrix = SingularMatrixProceeding.Reg;

        /// <summary>
        /// Czy w trakcie nauczania ma zwracać uwagę na polepszenie wyniku MSE dla zbioru DoubleSet 
        /// i nie odrzucać zmian, które mogą być potencjalnie korzystne
        /// </summary>
        private bool useDouble;

        /// <summary>
        /// MSE dla zbioru dublującego
        /// </summary>
        private double addSetMSE, previousAddSetMSE;

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

        //TODO - przenieść do zmiennych ogólnych

        int counterOfNoChangeTMSE = 0,
            counterOfNoChangeGMSE = 0;
        int MaxNoImprovementTimes = 10;

        double DifferenceBetweenMSEs = 0.0001;

        double tolerance = 0.3;

        public void SetTolerance(double d)
        {
            tolerance = d;
        }

        #endregion

        #region konstruktory

        public TrainerLMImproved(ZScore.ZScoreData dataset)
            : this(dataset, EnumDatasetStructures.Growing, DatasetStructure.DefaultGeneralizationSetSize,
            1, 1500, 99, 0.001, VDefault, 25, DefaultProceedingWithSingularMatrix, false, 0.0, false)
        { }

        public TrainerLMImproved(ZScore.ZScoreData dataset, EnumDatasetStructures ds, int holdout,
            int hiddenNum, ulong mE, double dMSE, double cMi, double aFV, int sz,
            SingularMatrixProceeding smp, bool uDouble, double tol, bool runAutomated)
        {

            Dataset = dataset;
            string setSplitTypeNameForFile;
            if (!uDouble)
            {
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
                    case EnumDatasetStructures.WindowedNoRandom:
                        DatasetIndexes = new DatasetOperateWindowedNoRandom(Dataset.NormalizedData[0].GetNum(), holdout, sz);
                        setSplitTypeNameForFile = "V";
                        break;
                    case EnumDatasetStructures.Simple:
                    default:
                        DatasetIndexes = new DatasetOperateSimple(Dataset.NormalizedData[0].GetNum(), holdout, sz);
                        setSplitTypeNameForFile = "S";
                        break;
                }
            }
            else
            {
                DatasetIndexes = new DatasetOperateAdditional(Dataset.NormalizedData[0].GetNum(), holdout, sz);
                setSplitTypeNameForFile = "D";
                addSetMSE = double.MaxValue;
            }

            NN = new neuralNetwork(Dataset.sample(0).Length, hiddenNum, Dataset.target(0).Length,
                Dataset.DataType, ActivationFuncType.Sigmoid, ActivationFuncType.Sigmoid);

            trainingSetMSE = generalizationSetMSE = double.MaxValue;
            maxEpochs = mE;
            desiredMSE = dMSE;
            coefficientMI = defaultMi = cMi;
            adjustmentFactorV = aFV;
            ProceedingWithSingularMatrix = smp;
            useDouble = uDouble;
            tolerance = tol;
            automatedRun = runAutomated;

            createFileNames(holdout, setSplitTypeNameForFile);
            durationOfEachEpoch = new Hashtable();
            durationInElapsedTicks = new Hashtable();
            timerE = new Stopwatch();
            timerA = new Stopwatch();

            if (!NN.IsLogistic)
                N = (NN.numInput + 1) * NN.numHidden
                    + (NN.numHidden + 1) * NN.numOutput; //liczba wag = N
            else
                N = (NN.numInput + 1) * NN.numOutput; //liczba wag = N

            vectorJ = new double[N];
            subvectorETA = new double[N];
            //error = new double[numTrainingPatternsInSingleEpoch];
            error = new double();
            submatrixQ = initArray(submatrixQ, N, N);
            initializeForNewEpoch();

            NN.InitializeWeights();

            NN.KeepWeightsToPrevious();

            Program.PrintInfo("Utworzono sieć neuronową");
            networkStats();
        }

        #endregion

        /// <summary>
        /// Funkcja nadzorujaca procesem uczenia sieci
        /// </summary>
        public bool TrainNetwork()
        {
            //uchwyt do pliku w ktorym zapisujemy wyniki
            TextWriter saveResult = new StreamWriter(resultsFileName);

            //inicjalizacja licznika epok oraz wag oraz flagi
            epochCounter = 0;

            trainingComplete = false;

            if (!automatedRun)
            {
                Console.Write("\nNaciśnij dowolny przycisk by rozpocząć nauczanie sieci...\n");
                Console.ReadKey();
            }
            ShowOptions();

            //naglowki w pliku z wynikami
            saveResult.WriteLine("#LMtraining: default μ = {0}, V = {1}, NN: {2}(+1):{3}(+1):{4}{5}",
                defaultMi, adjustmentFactorV, NN.numInput, NN.numHidden, NN.numOutput,
                useDouble ? String.Format(", tolerance = {0}", tolerance) : null);
            saveResult.WriteLine("#{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                "epoch", "tMSE", "tAcc", "gMSE", "gAcc", "ms");
            saveResult.Flush();

            saveResultsToFile(saveResult);

            PrintStatus();

            double bestGeneralizationSetMSE = generalizationSetMSE;

            previousTrainingSetMSE = trainingSetMSE;
            previousGeneralizationSetMSE = generalizationSetMSE;
            if (useDouble)
                previousAddSetMSE = addSetMSE;

            while (epochCounter < maxEpochs && !trainingComplete)
            {
                //kolejna epoka, zwiekszamy licznik
                epochCounter++;

                timerA.Start();
                timerE.Restart();

                // pojedyncza epoka
                if (runTrainingEpoch(DatasetIndexes.TrainingSet, saveResult))
                {
                    timerE.Stop();
                    timerA.Stop();

                    durationOfEachEpoch.Add(epochCounter, timerE.ElapsedMilliseconds);
                    durationInElapsedTicks.Add(epochCounter, timerE.ElapsedTicks);

                    //zapisujemy wyniki do pliku
                    saveResultsToFile(saveResult);

                    //dodatkowo zachowujemy wagi dla najnizszego MSE dla zbioru testujacego
                    if (bestGeneralizationSetMSE > generalizationSetMSE)
                    {
                        bestGeneralizationSetMSE = generalizationSetMSE;
                        NN.BestWeightInputHidden = NN.wInputHidden;
                        NN.BestWeightHiddenOutput = NN.wHiddenOutput;
                        //zapis wag do pliku pod warunkiem, ze minelo wiecej niz n-epok
                        if (epochCounter > 0 && !automatedRun)
                        {
                            NN.SaveWeights(weightsOutputFile, false);
                        }
                    }

                    //zmieniamy zakres indeksu podzbioru dla zbioru trenujacego
                    DatasetIndexes.IncreaseRange();

                    PrintStatus();


                    previousTrainingSetMSE = trainingSetMSE;
                }
                else
                {
                    timerE.Stop();

                    durationOfEachEpoch.Add(epochCounter, timerE.ElapsedMilliseconds);
                    durationInElapsedTicks.Add(epochCounter, timerE.ElapsedTicks);
                    NN.RestoreWeightsWithPrevious();
                    Debug.WriteLine(">> Wagi przywrócone (tMSE: {0})", NN.CalcMSE(Dataset, DatasetIndexes.TrainingSet));

                    trainingComplete = true;
                }

            }
            saveResult.WriteLine();
            saveResult.WriteLine("#ms.total_SpPdlaBP():\t{0}", timerA.ElapsedMilliseconds);
            saveResult.WriteLine("#ms.average():\t{0}\tlast:\t{1}",
                calcMeanDuration(durationOfEachEpoch), durationOfEachEpoch[epochCounter]);
            saveResult.WriteLine("#ticks.average():\t{0}\tlast:\t{1}",
                calcMeanDuration(durationInElapsedTicks), durationInElapsedTicks[epochCounter]);
            saveResult.Flush();
            saveResult.Close();
            Console.WriteLine();
            ShowOptions();
            return true;
        }

        /// <summary>
        /// Pojedyncza epoka - dla calego podzbioru danych feedforward+backpropagation,
        /// wyliczenie aktualnego bledu oraz ustawienie nowych wag
        /// </summary>
        /// <param name="trainingSet">indeksy w zbiorze danych na ktorych trenujemy siec w tej epoce</param>
        private bool runTrainingEpoch(int[] trainingSet, TextWriter saveResult)
        {
            initializeForNewEpoch();
            //jezeli kwadrat sumy bledow sie zmniejszyl
            bool epochComplete = false;

            bool miMsgShown = false;

            while (!epochComplete)
            {
                if (coefficientMI > MaxMiCoefficient || coefficientMI < MinMiCoefficient)
                {
                    trainingComplete = true;
                    Console.WriteLine("Wartość progowa μ została osiągnięta - koniec nauczania.");
                }

                Debug.WriteLine(">> feedforward, backward");
                for (int i = 0; i < trainingSet.Length; i++)
                {
                    //wyliczamy wyjscia i propagujemy wstecz
                    NN.FeedForward(Dataset.sample(trainingSet[i]));

                    //faza wstecz
                    backward(Dataset.target(trainingSet[i]), i);
                }

                Debug.WriteLine(">> update weights");
                if (updateWeights())
                {
                    Debug.WriteLine(">>>> if (updateWeights()) (true)");
                    recalcMSE();

                    Debug.WriteLine(">> {0:N3}>{1:N3} ({4}) || {6} {2:N3} > {3:N3} ({5})",
                        previousTrainingSetMSE, trainingSetMSE,
                        previousAddSetMSE, addSetMSE,
                        (previousTrainingSetMSE > trainingSetMSE) ? "T" : "F",
                        (previousAddSetMSE > addSetMSE - 0.01) ? "T" : "F",
                        useDouble ? "[T]" : "[F]");

                    //jezeli kwadrat sumy bledow sie zmniejszyl
                    if (previousTrainingSetMSE > trainingSetMSE ||
                        ((previousAddSetMSE > addSetMSE * (1 - (tolerance * Math.Sqrt(N)) / 100))
                        && useDouble && 
                        trainingSetMSE * (1 - (tolerance * Math.Sqrt(N)) / 100) < addSetMSE * (1 + (tolerance * Math.Sqrt(N)) / 100)))
                    //((previousDoubleSetMSE > doubleSetMSE * (1 + tolerance/adjustmentFactorV)) && useDouble))
                    {

                        Debug.WriteLine(">> true");
                        //if (!(previousTrainingSetMSE > trainingSetMSE) && ((previousDoubleSetMSE > doubleSetMSE) && useDouble))
                        if (!(previousTrainingSetMSE > trainingSetMSE) &&
                            ((previousAddSetMSE > addSetMSE * (1 - (tolerance * Math.Sqrt(N)) / 100)) && 
                            useDouble &&
                            trainingSetMSE * (1 - (tolerance * Math.Sqrt(N)) / 100) < addSetMSE * (1 + (tolerance * Math.Sqrt(N)) / 100)))
                        {
                            //NN.RestoreWeightsWithPrevious();
                            //incMi();
                            Console.WriteLine(">>>> Zmiana wag za zgodą DoubleSet! {0:N4} -> {1:N4} ({2:N4})",
                                previousAddSetMSE, addSetMSE, addSetMSE * (1 - (tolerance * Math.Sqrt(N)) / 100));
                            //additionalWeightsUpdateOnly();
                            saveResult.Write("\tT");

                            recalcMSE();

                            if (addSetMSE * (1 + (tolerance * Math.Sqrt(N)) / 100) > trainingSetMSE)
                            {
                                NN.KeepWeightsToPrevious();
                                previousAddSetMSE = addSetMSE;
                                previousTrainingSetMSE = trainingSetMSE;

                                initializeForNewEpoch();
                                foreach(int d in DatasetIndexes.DoubleSet)
                                {
                                    NN.FeedForward(Dataset.sample(d));
                                    backward(Dataset.target(d), d);
                                }
                                additionalWeightsCalcAndUpdate();
                                //updateWeights();

                                recalcMSE();

                                if (previousAddSetMSE > addSetMSE * (1 - (tolerance * Math.Sqrt(N)) / 100))
                                {
                                    Console.WriteLine(">>>> Wagi zaktualizowane przez DoubleSet!");
                                    saveResult.Write("+");
                                    if (previousTrainingSetMSE * (1 - (tolerance * Math.Sqrt(N)) / 100) < trainingSetMSE)
                                    {
                                        incMi();
                                    }
                                }
                                else
                                {
                                    saveResult.Write("-");
                                    NN.RestoreWeightsWithPrevious();
                                    incMi();
                                }

                            }
                        }

                        Console.WriteLine("\nWagi zostały zmienione dla μ: {0}", coefficientMI);
                        //zmniejszamy /mi
                        decMi();

                        //zachowujemy stan wag
                        NN.KeepWeightsToPrevious();
                        Debug.WriteLine(">> Wagi zapisane (tMSE: {0})", NN.CalcMSE(Dataset, DatasetIndexes.TrainingSet));

                        if (Math.Abs(previousTrainingSetMSE - trainingSetMSE) < DifferenceBetweenMSEs)
                        {
                            if (++counterOfNoChangeTMSE >= MaxNoImprovementTimes)
                            {
                                trainingComplete = true;
                                Console.WriteLine("Brak zmian w modelu tmse (x{0})", counterOfNoChangeTMSE);
                            }
                        }
                        else
                        {
                            counterOfNoChangeTMSE = 0;
                        }

                        if (Math.Abs(previousGeneralizationSetMSE - generalizationSetMSE) < DifferenceBetweenMSEs)
                        {
                            if (++counterOfNoChangeGMSE >= MaxNoImprovementTimes)
                            {
                                trainingComplete = true;
                                Console.WriteLine("Brak zmian w modelu gmse (x{0})", counterOfNoChangeGMSE);
                            }
                        }
                        else
                        {
                            counterOfNoChangeGMSE = 0;
                        }

                        previousTrainingSetMSE = trainingSetMSE;
                        previousGeneralizationSetMSE = generalizationSetMSE;
                        if (useDouble)
                            previousAddSetMSE = addSetMSE;

                        //iteracja sie konczy
                        //epochComplete = true;
                        return true;
                    }
                    else
                    {
                        NN.RestoreWeightsWithPrevious();
                        recalcMSE();
                        Debug.WriteLine(">> Wagi przywrócone (tMSE: {0})", trainingSetMSE);

                        if (coefficientMI > MaxMiCoefficient || coefficientMI < MinMiCoefficient)
                            return false;

                        Debug.WriteLine(">> false");
                        //powtarzamy iteracje epochComplete == false
                        //zwiekszamy /mi
                        incMi();
                        //odrzucamy nowe wagi

                        if (!miMsgShown)
                        {
                            Console.WriteLine("\nNieudana zmiana wag"
                                + " (błąd dla zmienionych wag przyjmuje większa wartość)\n"
                                + "zmiany wag cofnięte, zmiana wartości współczynnika μ:");
                            miMsgShown = true;
                        }
                        Console.Write("\b\b\b\b\b\b\b\b\b\b\b\b\b\b          "
                            + "\b\b\b\b\b\b\b\b\b\b\b\b\b\b{0}", coefficientMI);

                        DatasetIndexes.IncreaseRange();
                        Debug.WriteLine(">> SampleChangedToNext errDecFail ({0})", DatasetIndexes.TrainingSet.Length);
                    }
                }
                else
                {
                    Debug.WriteLine(">>>> if (updateWeights()) (false)");
                    if (!miMsgShown)
                    {
                        Console.WriteLine("\nNieudana zmiana wag"
                            + " (niepowodzenia w wyliczaniu macierzy odwrotnej)\n"
                            + "zmiany wag cofnięte, zmiana wartości współczynnika μ:");
                        miMsgShown = true;
                    }
                    return false;
                }
            }


            if (coefficientMI > MaxMiCoefficient || coefficientMI < MinMiCoefficient)
            {
                NN.RestoreWeightsWithPrevious();
                recalcMSE();
                Debug.WriteLine(">> Wagi przywrócone (tMSE): {0})", NN.CalcMSE(Dataset, DatasetIndexes.TrainingSet));
                Console.WriteLine("\nMin/Max wartość η osiągnięta! ({0})", coefficientMI);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Faza WSTECZ
        /// </summary>
        /// <param name="desiredOutputs"></param>
        /// <param name="sampleNum"></param>
        private void backward(double[] desiredOutputs, int sampleNum)
        {
            double[] s = new double[NN.numHidden + NN.numOutput];
            for (int m = 0; m < NN.numOutput; m++)
            {
                //odliczamy aktualny blad sieci dla wyjscia dla aktualnej probki
                error = desiredOutputs[m] - NN.OutputNeurons[m];

                //lista pochodnych dla funkcji bledu oraz sum wazonych 
                //D(e_ij)/D(net_ij) 
                s[NN.numHidden + m] = -NN.DerivativeOfOutput(NN.OutputNets[m]);
                for (int h = NN.numHidden - 1; h > 0; h--)
                {
                    s[h] = -NN.DerivativeOfHidden(NN.HiddenNets[h]) *
                        NN.wHiddenOutput[h][m] * NN.DerivativeOfOutput(NN.OutputNets[m]);
                }

                //obliczamy wektor j
                int wIndex = 0;
                if (!NN.IsLogistic)
                {
                    for (int i = 0; i < NN.numHidden; i++)
                        for (int k = 0; k < NN.numInput + 1; k++)
                        {
                            vectorJ[wIndex] = s[i] * NN.Inputs[k];
                            wIndex++;
                        }

                    for (int k = 0; k < NN.numHidden + 1; k++)
                    {
                        vectorJ[wIndex] = s[NN.numHidden + m] * NN.HiddenNeurons[k];
                        wIndex++;
                    }
                }
                else
                {
                    for (int k = 0; k < NN.numInput + 1; k++)
                    {
                        vectorJ[wIndex] = s[m] * NN.Inputs[k];
                        wIndex++;
                    }
                }

                //obliczamy podmacierz q
                for (int n = 0; n < N; n++)
                {
                    for (int k = n; k < N; k++)
                    {
                        submatrixQ[n][k] = vectorJ[k] * vectorJ[n];
                    }
                }

                //obliczamy podwektor ETA
                for (int i = 0; i < N; i++)
                {
                    subvectorETA[i] = vectorJ[i] * error;
                }

                //Q = Q + q
                for (int n = 0; n < N; n++)
                {
                    for (int k = 0; k < N; k++)
                    {
                        matrixQ[n][k] = matrixQ[n][k] + submatrixQ[n][k];
                    }
                }

                //g = g + eta
                for (int i = 0; i < N; i++)
                {
                    gradientVectorG[i] = gradientVectorG[i] + subvectorETA[i];
                }
            }
        }

        /// <summary>
        /// Aktualizacja wag
        /// </summary>
        /// <returns>True jesli wagi zostaly zmienione, False jesli nie zostaly zmienione</returns>
        private bool updateWeights()
        {
            //zmiana wag = (Q + \mi*I)^{-1} * g
            double[][] delta = new double[N][];
            delta = initArray(delta, N, N);

            //Q + /mi * I
            for (int n = 0; n < N; n++)
            {
                matrixQ[n][n] += coefficientMI;
            }

            double[][] invertedMatrix = matrixQ;

            //proba odwrocenia macierzy "normalnie"
            try
            {
                //(Q + /mi * I)^(-1)
                invertedMatrix = multidimensionalArrayToJaggedArray
                    (MatrixLibrary.Matrix.Inverse(jaggedArrayToMultidimensionalArray(matrixQ)));
            }
            catch (MatrixLibrary.MatrixDeterminentZero)
            {
                switch (ProceedingWithSingularMatrix)
                {

                    //regularyzacja i ponowne odwrocenie
                    case SingularMatrixProceeding.Reg:

                        Debug.WriteLine(">> {0}:: Regularization", epochCounter);

                        Console.WriteLine(">>>> REG");

                        /*double[][] matrixRegQ = multidimensionalArrayToJaggedArray
                            (Matrix.Add(Matrix.Transpose(jaggedArrayToMultidimensionalArray(matrixQ)),
                            jaggedArrayToMultidimensionalArray(matrixQ)));

                        try
                        {
                            invertedMatrix = multidimensionalArrayToJaggedArray
                                (MatrixLibrary.Matrix.Inverse(jaggedArrayToMultidimensionalArray(matrixRegQ)));
                        }
                        catch (MatrixLibrary.MatrixDeterminentZero)
                        {
                            Debug.WriteLine(">> Regularization didn't work!");
                            return false;
                        }*/

                        //wspolczynnik regularyzacji
                        double factor = 0;
                        for (int i = 0; i < matrixQ.Length; i++)
                            factor += matrixQ[i][i];

                        double[][] matrixRegQ = matrixQ;
                        for (int n = 0; n < N; n++)
                            matrixRegQ[n][n] += factor;

                        try
                        {
                            invertedMatrix = multidimensionalArrayToJaggedArray
                                (MatrixLibrary.Matrix.Inverse(jaggedArrayToMultidimensionalArray(matrixRegQ)));
                        }
                        catch (MatrixLibrary.MatrixDeterminentZero)
                        {
                            Debug.WriteLine(">> Regularization didn't work!");
                            return false;
                        }


                        break;


                    //rozklad wedlug wartosci osobliwych (Singular Value Decomposition)
                    case SingularMatrixProceeding.SVD:
                        Debug.WriteLine(">> {0}:: SVD", epochCounter);

                        //TODO usunąć info
                        Console.WriteLine(">>>> SVD");

                        // Matrix = U x S x V'
                        double[,] U, S, V;
                        try
                        {
                            MatrixLibrary.Matrix.SVD(jaggedArrayToMultidimensionalArray(matrixQ), out S, out U, out V);
                            Debug.WriteLine(">> Q = U[{0}x{1}] x S[{2}x{3}] x [{4}x{5}]", U.GetLength(0), U.GetLength(1),
                                S.GetLength(0), S.GetLength(1), V.GetLength(0), V.GetLength(1));

                            // Matrix^{-1} = V x S^{-1} x U'
                            try
                            {
                                S = MatrixLibrary.Matrix.Inverse(S);
                            }
                            catch (MatrixLibrary.MatrixDeterminentZero)
                            {
                                try
                                {
                                    S = MatrixLibrary.Matrix.PINV(S);
                                    Debug.WriteLine(">> Pseudo-Inverse for S success");
                                }
                                catch (MatrixLibrary.MatrixDeterminentZero)
                                {
                                    Debug.WriteLine(">> Pseudo-Inverse for S failed");
                                    return false;
                                }
                            }

                            invertedMatrix = multidimensionalArrayToJaggedArray
                                (MatrixLibrary.Matrix.Multiply(MatrixLibrary.Matrix.Multiply(V, S),
                                    MatrixLibrary.Matrix.Transpose(U)));

                        }
                        catch (MatrixLibrary.MatrixDeterminentZero)
                        {
                            Debug.WriteLine(">> SVD fail!");
                            try
                            {
                                invertedMatrix = multidimensionalArrayToJaggedArray
                                    (MatrixLibrary.Matrix.PINV(jaggedArrayToMultidimensionalArray(matrixQ)));
                            }
                            catch (MatrixLibrary.MatrixDeterminentZero)
                            {
                                Debug.WriteLine(">> PINV niepowodzenie! return;");
                                return false;
                            }

                        }
                        break;

                    case SingularMatrixProceeding.PINV:

                        Console.WriteLine(">>>> PINV");
                        Debug.WriteLine(">> {0}:: PINV", epochCounter);
                        try
                        {
                            invertedMatrix = multidimensionalArrayToJaggedArray
                                (MatrixLibrary.Matrix.PINV(jaggedArrayToMultidimensionalArray(matrixQ)));
                        }
                        catch (MatrixLibrary.MatrixDeterminentZero)
                        {
                            return false;
                        }
                        break;

                    case SingularMatrixProceeding.None:
                    default:
                        return false;
                }
            }

            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < N; m++)
                {
                    deltaWeights[n] += invertedMatrix[n][m] * gradientVectorG[m];
                }
            }

            int c = 0;
            if (!NN.IsLogistic)
            {
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
            }
            else
            {
                for (int i = 0; i < NN.numInput + 1; i++)
                {
                    for (int j = 0; j < NN.numOutput; j++)
                    {
                        NN.wInputHidden[i][j] -= deltaWeights[c];
                        c++;
                    }
                }
            }

            return true;
        }

        private bool additionalWeightsCalcAndUpdate()
        {
            //zmiana wag = (Q + \mi*I)^{-1} * g
            double[][] delta = new double[N][];
            delta = initArray(delta, N, N);

            //Q + /mi * I
            for (int n = 0; n < N; n++)
            {
                matrixQ[n][n] += coefficientMI;
            }

            double[][] invertedMatrix;

            //proba odwrocenia macierzy "normalnie"
            try
            {
                //(Q + /mi * I)^(-1)
                invertedMatrix = multidimensionalArrayToJaggedArray
                    (MatrixLibrary.Matrix.Inverse(jaggedArrayToMultidimensionalArray(matrixQ)));
            }
            catch (MatrixLibrary.MatrixDeterminentZero)
            {
                switch (ProceedingWithSingularMatrix)
                {

                    //regularyzacja i ponowne odwrocenie
                    case SingularMatrixProceeding.Reg:

                        Debug.WriteLine(">> {0}:: Regularization", epochCounter);
                        //wspolczynnik regularyzacji
                        double factor = 0;
                        for (int i = 0; i < matrixQ.Length; i++)
                            factor += matrixQ[i][i];

                        for (int n = 0; n < N; n++)
                            matrixQ[n][n] += factor;

                        try
                        {
                            invertedMatrix = multidimensionalArrayToJaggedArray
                                (MatrixLibrary.Matrix.Inverse(jaggedArrayToMultidimensionalArray(matrixQ)));
                        }
                        catch (MatrixLibrary.MatrixDeterminentZero)
                        {
                            Debug.WriteLine(">> Regularization didn't work!");
                            return false;
                        }
                        break;


                    //rozklad wedlug wartosci osobliwych (Singular Value Decomposition)
                    case SingularMatrixProceeding.SVD:
                        Debug.WriteLine(">> {0}:: SVD", epochCounter);

                        // Matrix = U x S x V'
                        double[,] U, S, V;
                        try
                        {
                            MatrixLibrary.Matrix.SVD(jaggedArrayToMultidimensionalArray(matrixQ), out S, out U, out V);
                            Debug.WriteLine(">> Q = U[{0}x{1}] x S[{2}x{3}] x [{4}x{5}]", U.GetLength(0), U.GetLength(1),
                                S.GetLength(0), S.GetLength(1), V.GetLength(0), V.GetLength(1));

                            // Matrix^{-1} = V x S^{-1} x U'
                            try
                            {
                                S = MatrixLibrary.Matrix.Inverse(S);
                            }
                            catch (MatrixLibrary.MatrixDeterminentZero)
                            {
                                try
                                {
                                    S = MatrixLibrary.Matrix.PINV(S);
                                    Debug.WriteLine(">> Pseudo-Inverse for S success");
                                }
                                catch (MatrixLibrary.MatrixDeterminentZero)
                                {
                                    Debug.WriteLine(">> Pseudo-Inverse for S failed");
                                    return false;
                                }
                            }

                            invertedMatrix = multidimensionalArrayToJaggedArray
                                (MatrixLibrary.Matrix.Multiply(MatrixLibrary.Matrix.Multiply(V, S),
                                    MatrixLibrary.Matrix.Transpose(U)));

                        }
                        catch (MatrixLibrary.MatrixDeterminentZero)
                        {
                            Debug.WriteLine(">> SVD fail!");
                            try
                            {
                                invertedMatrix = multidimensionalArrayToJaggedArray
                                    (MatrixLibrary.Matrix.PINV(jaggedArrayToMultidimensionalArray(matrixQ)));
                            }
                            catch (MatrixLibrary.MatrixDeterminentZero)
                            {
                                Debug.WriteLine(">> PINV niepowodzenie! return;");
                                return false;
                            }

                        }
                        break;

                    case SingularMatrixProceeding.PINV:
                        Debug.WriteLine(">> {0}:: PINV", epochCounter);
                        try
                        {
                            invertedMatrix = multidimensionalArrayToJaggedArray
                                (MatrixLibrary.Matrix.PINV(jaggedArrayToMultidimensionalArray(matrixQ)));
                        }
                        catch (MatrixLibrary.MatrixDeterminentZero)
                        {
                            return false;
                        }
                        break;

                    case SingularMatrixProceeding.None:
                    default:
                        return false;
                }
            }

            for (int n = 0; n < N; n++)
            {
                for (int m = 0; m < N; m++)
                {
                    deltaWeights[n] += invertedMatrix[n][m] * gradientVectorG[m];
                }
            }

            int c = 0;
            if (!NN.IsLogistic)
            {
                for (int i = 0; i < NN.numInput + 1; i++)
                {
                    for (int j = 0; j < NN.numHidden; j++)
                    {
                        NN.wInputHidden[i][j] -= deltaWeights[c] * tolerance;
                        c++;
                    }
                }

                for (int i = 0; i < NN.numHidden + 1; i++)
                {
                    for (int j = 0; j < NN.numOutput; j++)
                    {
                        NN.wHiddenOutput[i][j] -= deltaWeights[c] * tolerance;
                        c++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < NN.numInput + 1; i++)
                {
                    for (int j = 0; j < NN.numOutput; j++)
                    {
                        NN.wInputHidden[i][j] -= deltaWeights[c] * tolerance;
                        c++;
                    }
                }
            }

            return true;
        }

        private bool additionalWeightsUpdateOnly()
        {
            int c = 0;
            if (!NN.IsLogistic)
            {
                for (int i = 0; i < NN.numInput + 1; i++)
                {
                    for (int j = 0; j < NN.numHidden; j++)
                    {
                        NN.wInputHidden[i][j] -= deltaWeights[c] * tolerance;
                        c++;
                    }
                }

                for (int i = 0; i < NN.numHidden + 1; i++)
                {
                    for (int j = 0; j < NN.numOutput; j++)
                    {
                        NN.wHiddenOutput[i][j] -= deltaWeights[c] * tolerance;
                        c++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < NN.numInput + 1; i++)
                {
                    for (int j = 0; j < NN.numOutput; j++)
                    {
                        NN.wInputHidden[i][j] -= deltaWeights[c] * tolerance;
                        c++;
                    }
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

        private void recalcMSE()
        {
            trainingSetMSE = NN.CalcMSE(Dataset, DatasetIndexes.TrainingSet);
            generalizationSetMSE = NN.CalcMSE(Dataset, DatasetIndexes.GeneralizationSet);
            if (useDouble)
                addSetMSE = NN.CalcMSE(Dataset, DatasetIndexes.DoubleSet);
        }

        private void recalcAcc()
        {
            trainingSetAccuracy = NN.GetAccuracy(Dataset, DatasetIndexes.TrainingSet);
            generalizationSetAccuracy = NN.GetAccuracy(Dataset, DatasetIndexes.GeneralizationSet);
        }

        /// <summary>
        /// Zapis pomiarow do pliku
        /// </summary>
        /// <param name="saveResult">uchwyt do pliku z wynikami</param>
        private void saveResultsToFile(TextWriter saveResult)
        {
            //pobieramy celnosc modelu i sume kwadratow bledu uczenia
            recalcMSE();
            recalcAcc();

            try
            {
                string line = String.Format("{0}\t{1:N4}\t{2:N2}\t{3:N4}\t{4:N2}\t{5}",
                    epochCounter, trainingSetMSE, trainingSetAccuracy, generalizationSetMSE,
                    generalizationSetAccuracy, durationOfEachEpoch[epochCounter]);
                line = line.Replace(",", ".");
                saveResult.WriteLine();
                saveResult.Write(line);

                saveResult.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("Nieudany zapis do pliku wyników:\n{0}", e.Message);
            }
        }

        /// <summary>
        /// utworz nazwy plikow (zwiazane z typem danych)
        /// </summary>
        private void createFileNames(int holdout, string setSplitType)
        {
            resultsFileName = String.Format("wynik_{0}_LM-{7}g{1}-{2}-{3}-{4}-{5}-{6}.txt",
                Enum.GetName(typeof(ZScore.EnumDataTypes), (int)Dataset.DataType), holdout.ToString(),
                NN.numHidden.ToString(), coefficientMI.ToString(), adjustmentFactorV.ToString(),
                Enum.GetName(typeof(SingularMatrixProceeding), ProceedingWithSingularMatrix),
                useDouble ? String.Format("T{0:N2}", tolerance)
                : "F", setSplitType);
            weightsOutputFile = String.Format("weights_{0}_LM-{1}.txt",
                Enum.GetName(typeof(ZScore.EnumDataTypes), (int)Dataset.DataType),
                NN.numHidden.ToString());
            customWeightsOutputFile = String.Format("customWeights_{0}_LM-{1}.txt",
                Enum.GetName(typeof(ZScore.EnumDataTypes), (int)Dataset.DataType),
                NN.numHidden.ToString());
        }

        private void networkStats()
        {
            Console.WriteLine("Liczba neuronow w warstwie wejsciowej:\t{0}", NN.numInput);
            Console.WriteLine("Liczba neuronow w wartstwie ukrytej:\t{0}", NN.numHidden);
            Console.WriteLine("Liczba neuronow w warstwie wyjsciowej:\t{0}", NN.numOutput);
            Console.WriteLine("Parametry:");
            Console.WriteLine("Domyślny współczynnik tłumienia μ:\t{0}", coefficientMI);
            Console.WriteLine("Współczynnik przystosowania V:\t\t{0}", adjustmentFactorV);
            Console.WriteLine("Wielkość Macierzy Hessego: {0}x{0}", N);
            Console.WriteLine("Maksymalna liczba epok:\t\t{0}", maxEpochs);
            Console.WriteLine("Docelowy MSE:\t{0}\n", desiredMSE);
            Console.WriteLine("Wielkość próbki treningowej: {0}, \nRozmiar zbioru walidacyjnego: {1}",
                DatasetIndexes.TrainingSet.Length, DatasetIndexes.GeneralizationSet.Length);
            Console.WriteLine("Postępowanie w przypadku macierzy osobliwych: {0}",
                Enum.GetName(typeof(SingularMatrixProceeding), ProceedingWithSingularMatrix));
            Console.WriteLine("Korzysta ze zbioru dodatkowego? {0}\n",
                useDouble ? "Prawda" : "Fałsz");
        }

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
        /// Inicializacja macierzy Q i wektora gradientu g,
        /// musza byc zerowane na poczatku kazdej epoki
        /// </summary>
        private void initializeForNewEpoch()
        {
            matrixQ = initArray(matrixQ, N, N);
            gradientVectorG = new double[N];
            deltaWeights = new double[N];
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
                            NN.SaveWeights(customWeightsOutputFile, false);
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
                            NN.SaveWeights(customWeightsOutputFile, false);
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

    /// <summary>
    /// Rodzaj postepowania w przypadku odwracania macierzy osobliwej
    /// </summary>
    public enum SingularMatrixProceeding
    {
        /// <summary>
        /// Regularyzuj macierz (Tikhonov-Miller regularization)
        /// </summary>
        Reg,
        /// <summary>
        /// Singular value decomposition
        /// </summary>
        SVD,
        /// <summary>
        /// Moore–Penrose pseudoinverse
        /// </summary>
        PINV,
        /// <summary>
        /// Nie podejmuj prob odwracania gdy wyznacznik jest rowny zero
        /// </summary>
        None,
    }
}
