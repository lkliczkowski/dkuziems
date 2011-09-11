using System;
using ZScore;

namespace LearningBPandLM
{
    partial class Program
    {
        #region opcje ogolne konfiguracji i danych

        private static void programInfo()
        {
            Console.WriteLine("W programie zaimplementowane są 2 metody nauczania sieci neuronowych, "
                + "na danych wykonywana jest także standaryzacja, wybierz jedną z poniższych opcji.\n"
                + VER);
        }

        private static void setOptionsToDefault()
        {
            inputFile = outputFile = "";
            dataType = EnumDataTypes.unknown;
            //flagi menu
            endFlag = BPendFlag = LMendFlag = AMenuFlag = readyToZScore = readyToCreateNN
                = readyToTrain = configured = LMuseDoubleSet = runAutomated = false;

            //parametry konfiguracji            
            hiddenRatioPar = 4;
            desiredMSEPar = 0.01;
            maxEpochsPar = 1500;
            learningRatePar = 0.01;
            coefficientMIpar = 0.01;
            adjustmentFactorVpar = 10;
            holdoutPercentagePar = 20;
            datasetStructurePar = EnumDatasetStructures.Growing;
            sampleSizePar = 2;
            proceedingWithSingular = SingularMatrixProceeding.Reg;
            tolerancePar = 0.0;
        }

        private static void SetToHeartDisease()
        {
            inputFile = "HeartDisease.csv";
            //outputFile = "normalizedHeartDisease.csv";
            dataType = EnumDataTypes.HeartDisease;
            readyToZScore = true;
        }

        private static void SetToGermanCreditData()
        {
            inputFile = "GermanCreditData.csv";
            //outputFile = "normalizedGermanCreditData.csv";
            dataType = EnumDataTypes.GermanCreditData;
            readyToZScore = true;
        }

        private static void SetToLetterRecognition()
        {
            inputFile = "letter-a-recognition.csv";
            //outputFile = "normalizedLetter-a-recognition.csv";
            dataType = EnumDataTypes.LetterRecognitionA;
            readyToZScore = true;
        }

        private static void SetToCreditRisk()
        {
            inputFile = "CreditRisk.csv";
            //outputFile = "normalizedCreditRisk.csv";
            dataType = EnumDataTypes.CreditRisk;
            readyToZScore = true;
        }

        private static void prepareData()
        {
            PrintInfo("Przygotowywanie danych");
            if (outputFile.Equals(""))
                dataset = new ZScoreData(inputFile, dataType);
            else
                dataset = new ZScoreData(inputFile, outputFile, dataType);

            if (dataset.NormalizeRun())
            {
                Console.WriteLine("Standaryzacja Z-Score zakończona z powodzeniem!");
                readyToCreateNN = true;
            }
            else
            {
                Console.WriteLine("StandaryzacjaZ (ZScore) nie powiodła się!");
            }
        }

        private static void ShowConfig()
        {
            PrintInfo("Konfiguracja");
            Console.WriteLine("Opcje ustawione dla {0} typu: {1}",
                (inputFile == "") ? "brak" : inputFile, Enum.GetName(typeof(EnumDataTypes), dataType));

            Console.WriteLine("\nKonfiguracja sieci:{0}", (configured == false) ? "\n[nie konfigurowano - wartości domyślne]" : "");
            Console.WriteLine("Liczba neuronów w war.ukrytej:\t{0}", hiddenRatioPar);
            Console.WriteLine("Maksymalna liczba epok:\t\t{0}", maxEpochsPar);
            Console.WriteLine("Docelowy błąd MSE:\t{0}\n", desiredMSEPar);
        }

        private static void setHiddenRatio()
        {
            Console.WriteLine("Podaj liczbę neuronów w warstwie ukrytej, obecna: {0}", hiddenRatioPar);
            //Console.WriteLine("Liczba neuronów zostanie odliczona wg: \n[wspolczynnik]*sqrt([liczba wejść]*[liczba wyjść])");
            try
            {
                hiddenRatioPar = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona wartość poprzednia.");
            }
            Console.WriteLine("Obecna liczba neuronów wynosi: {0}.", hiddenRatioPar);
            //Console.WriteLine("Utworzona sieć będzie mieć [{0}] neuronów w warstwie ukrytej.\n\n",
            //    (int)(hiddenRatioPar * Math.Sqrt(dataset.sample(0).Length * dataset.target(0).Length)));
        }

        private static void setMaxEpoch()
        {
            Console.WriteLine("Podaj maksymalną ilość epok, obecna: {0}", maxEpochsPar);
            try
            {
                maxEpochsPar = ulong.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią...");
            }
            Console.WriteLine("Obecna maksymalna ilość epok: {0}\n", maxEpochsPar);
        }

        private static void setMSE()
        {
            double desiredMSE = 0.01;
            Console.WriteLine("Podaj docelowy MSE modelu, obecny: {0}", desiredMSEPar);
            try
            {
                desiredMSEPar = Double.Parse((Console.ReadLine().Replace(".", ",")));
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią");
            }
            if (desiredMSEPar >= 1 || desiredMSEPar <= 0)
            {
                Console.WriteLine("MSE powinno wynosić wartość pomiędzy <0, 1>");
                Console.WriteLine("ustawiona na domyślną ({0})", desiredMSE);
                desiredMSEPar = desiredMSE;
            }
            Console.WriteLine("Obecny docelowy MSE: {0}\n", desiredMSEPar);
        }

        private static void setHoldout()
        {
            int previousHoldout = holdoutPercentagePar;
            Console.WriteLine("Ile (w %) danych ma być zbiorem walidacyjnym, obecnie: {0}%", holdoutPercentagePar);
            try
            {
                holdoutPercentagePar = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią");
            }
            if (holdoutPercentagePar > 101 || holdoutPercentagePar < 0)
            {
                Console.WriteLine("Niepoprawna wartość, powinna wynosić pomiędzy (0-100)%");
                Console.WriteLine("ustawiona na wartość poprzednią ({0}%)", previousHoldout);
                holdoutPercentagePar = previousHoldout;
            }
            Console.WriteLine("Obecny podział danych to:\n {0}% - walidacyjny\n {1}% - treningowy\n",
                holdoutPercentagePar, 100 - holdoutPercentagePar);
        }


        private static void setDatasetStructure()
        {
            Console.WriteLine("W jaki sposób algorytm ma pobierać dane z próbki treningowej? (w jaki sposób dane mają być podzielone)");
            Console.WriteLine("[1]{0} Rosnący zbiór danych (Growing Dataset)",
                (datasetStructurePar == EnumDatasetStructures.Growing) ? "[Opcja obecna]" : "");
            Console.WriteLine("[2]{0} Okienkowy zbiór danych (Windowed Dataset) ",
                (datasetStructurePar == EnumDatasetStructures.Windowed) ? "[Opcja obecna]" : "");
            Console.WriteLine("[3]{0} Okienkowy zbiór danych (Windowed Dataset) ",
                (datasetStructurePar == EnumDatasetStructures.WindowedNoRandom) ? "[Opcja obecna]" : "");
            Console.WriteLine("[4]{0} Brak podziału (w każdej epoce analizuj całość danych treningowych) ",
                (datasetStructurePar == EnumDatasetStructures.Simple) ? "[Opcja obecna]" : "");

            try
            {
                int opt = Int32.Parse(Console.ReadLine());

                if (Enum.IsDefined(typeof(EnumDatasetStructures), --opt))
                {
                    datasetStructurePar = (EnumDatasetStructures)opt;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartość");
                }
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią");
            }
            Console.WriteLine("Obecnie ustawiono [{0}] {1}", (int)datasetStructurePar + 1,
                Enum.GetName(typeof(EnumDatasetStructures), datasetStructurePar));
        }

        private static void setSampleSize()
        {
            int sampleSizeDefault;
            switch (datasetStructurePar)
            {
                case EnumDatasetStructures.Growing:
                    if (!configured)
                        sampleSizePar = 3;
                    sampleSizeDefault = 3;
                    Console.WriteLine("Ile ma wynosic przyrost próbki danych w każdej kolejnej epoce? obecnie: {0}%",
                        sampleSizePar);
                    getSampleSize(0, 101, sampleSizeDefault);
                    break;
                case EnumDatasetStructures.Windowed:
                case EnumDatasetStructures.WindowedNoRandom:
                    if (!configured)
                        sampleSizePar = 15;
                    sampleSizeDefault = 15;
                    Console.WriteLine("Podaj wielkosc jednorazowej próbki danych (w %), obecna: {0}%", sampleSizePar);
                    Console.WriteLine("(Jednorazowa próbka danych ze zbioru treningowego analizowana w każdej epoce)");
                    getSampleSize(0, 51, sampleSizeDefault);
                    break;
                case EnumDatasetStructures.Simple:
                default:
                    return;
            }

            Console.WriteLine("Obecna wielkosc próbki wynosi: {0}%\n", sampleSizePar);
        }

        private static void getSampleSize(int from, int to, int sampleSizeDefault)
        {
            Console.WriteLine("Nie mniej niż {0}%, nie więcej niż {1}%", from + 1, to - 1);
            try
            {
                sampleSizePar = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią");
            }
            if (sampleSizePar < from || sampleSizePar > to)
            {
                Console.WriteLine("Niepoprawna wartość, powinna wynosić pomiędzy ({0}-{1})%", from, to);
                Console.WriteLine("ustawiona na domyślną ({0}%)", sampleSizeDefault);
                sampleSizePar = sampleSizeDefault;
            }
        }

        #endregion

        #region podmenu Backprop

        private static void BPAlgorithm()
        {
            while (!BPendFlag)
            {
                PrintInfo("Menu algorytmu Backpropagation");
                Menu BPmenu = new Menu();
                BPmenu.Add("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", BPquickStart);
                BPmenu.Add("[wybor danych] Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                BPmenu.Add("[wybor danych] Ustaw Opcje dla GermanCreditData", SetToGermanCreditData);
                BPmenu.Add("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);
                BPmenu.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                BPmenu.Add("Wyświetl konfigurację", ShowConfig);
                BPmenu.Add("Powrót do menu głównego", BPEnd);

                if (readyToZScore)
                {
                    BPmenu.Remove("Powrót do menu głównego", BPEnd);
                    BPmenu.Remove("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", BPquickStart);
                    BPmenu.Add(">> Standaryzacja danych", prepareData);
                    BPmenu.Add("Powrót do menu głównego", BPEnd);
                }

                if (readyToCreateNN)
                {
                    BPmenu.Remove("Powrót do menu głównego", BPEnd);
                    BPmenu.Add(String.Format(">> Konfiguruj sieć:\n\t\t" +
                        "- liczba neuronów w warstwie ukrytej,\n\t\t" +
                        "- współczynnik uczenia,\n\t\t" +
                        "- maksymalną liczbę epok,\n\t\t" +
                        "- docelowa dokładność modelu."), BPSetConfig);
                    BPmenu.Add(">> Utworz nową sieć neuronową oraz zbiory trenujący i walidacyjny", BPCreateNN);

                    BPmenu.Remove("Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                    BPmenu.Remove("[wybor danych] Ustaw Opcje dla GermanCreditData", SetToGermanCreditData);
                    BPmenu.Remove("Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                    BPmenu.Remove("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);
                    BPmenu.Add("Powrót do menu głównego", BPEnd);
                }

                if (!readyToTrain)
                {
                    BPmenu.Remove(">> Rozpocznij/kontynuuj działanie sieci", BPstart);
                }

                if (readyToTrain)
                {
                    BPmenu.Remove("Powrót do menu głównego", BPEnd);
                    BPmenu.Add(">> Rozpocznij/kontynuuj działanie sieci", BPstart);
                    BPmenu.Add("Powrót do menu głównego", BPEnd);
                }

                BPmenu.Show();
            }

            Console.ReadKey();
        }

        private static void BPquickStart()
        {
            SetToLetterRecognition();
            //SetToGermanCreditData();
            try
            {
                prepareData();
                BPCreateNN();
                BPstart();
            }
            catch
            {
                Console.WriteLine("Niepowodzenie.");
            }
        }

        private static void BPSetConfig()
        {
            setHiddenRatio();
            BPsetLearningRate();
            setMaxEpoch();
            setMSE();
            setHoldout();
            setDatasetStructure();
            setSampleSize();
            configured = true;
        }

        private static void BPsetLearningRate()
        {
            double learningRateDefault = 0.01;
            Console.WriteLine("Podaj współczynnik uczenia, obecny: {0}, domyślny: {1}", learningRatePar, learningRateDefault);
            try
            {
                learningRatePar = Double.Parse((Console.ReadLine().Replace(".", ",")));
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na domyślną ({0})", learningRateDefault);
                learningRatePar = learningRateDefault;
            }
            Console.WriteLine("Obecny wspołczynnik uczenia wynosi: {0}\n", learningRatePar);
        }

        private static void BPCreateNN()
        {
            if (configured)
                networkTrainerBP = new TrainerBP(dataset, datasetStructurePar,
                    holdoutPercentagePar, hiddenRatioPar, learningRatePar,
                    maxEpochsPar, desiredMSEPar, sampleSizePar, runAutomated);
            else
                networkTrainerBP = new TrainerBP(dataset);

            readyToTrain = true;
        }

        private static void BPstart()
        {
            networkTrainerBP.TrainNetwork();
            readyToTrain = false;
            networkTrainerBP.ShowOptions();
        }

        private static void BPEnd()
        {
            Console.WriteLine("Moduł zakończy działanie, naciśnij dowolny przycisk...");
            BPendFlag = true;
        }
        #endregion

        #region podmenu LM

        private static void LMAlgorithm()
        {
            while (!LMendFlag)
            {
                PrintInfo("Menu algorytmu Levenberga-Marquardta");
                Menu LMmenu = new Menu();
                LMmenu.Add("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", LMquickStart);
                LMmenu.Add("[wybor danych] Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                LMmenu.Add("[wybor danych] Ustaw Opcje dla GermanCreditData", SetToGermanCreditData);
                LMmenu.Add("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);
                LMmenu.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                LMmenu.Add("Wyświetl konfigurację", ShowConfig);
                LMmenu.Add("Powrót do menu głównego", LMEnd);

                if (readyToZScore)
                {
                    LMmenu.Remove("Powrót do menu głównego", LMEnd);
                    LMmenu.Remove("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", LMquickStart);
                    LMmenu.Add(">> Standaryzacja danych", prepareData);
                    LMmenu.Add("Powrót do menu głównego", LMEnd);
                }

                if (readyToCreateNN)
                {
                    LMmenu.Remove("Powrót do menu głównego", LMEnd);
                    LMmenu.Add(String.Format(">> Konfiguruj sieć:\n\t\t" +
                        "- liczba neuronów w warstwie ukrytej,\n\t\t" +
                        "- maksymalną liczbę epok,\n\t\t" +
                        "- docelowa dokładność modelu."), LMSetConfig);
                    LMmenu.Add(">> Utworz nową sieć neuronową oraz zbiory trenujący i walidacyjny", LMCreateNN);


                    LMmenu.Remove("Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                    LMmenu.Remove("[wybor danych] Ustaw Opcje dla GermanCreditData", SetToGermanCreditData);
                    LMmenu.Remove("Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                    LMmenu.Remove("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);
                    LMmenu.Add("Powrót do menu głównego", LMEnd);
                }

                if (!readyToTrain)
                {
                    LMmenu.Remove(">> Rozpocznij/kontynuuj działanie sieci", LMStart);
                }

                if (readyToTrain)
                {
                    LMmenu.Remove("Powrót do menu głównego", LMEnd);
                    LMmenu.Add(">> Rozpocznij/kontynuuj działanie sieci", LMStart);
                    LMmenu.Add("Powrót do menu głównego", LMEnd);
                }

                LMmenu.Show();
            }

            Console.ReadKey();
        }

        private static void LMquickStart()
        {
            SetToLetterRecognition();
            //SetToGermanCreditData();
            try
            {
                prepareData();
                LMCreateNN();
                LMStart();
            }
            catch
            {
                Console.WriteLine("Niepowodzenie.");
            }
        }

        private static void LMSetConfig()
        {
            setHiddenRatio();
            LMsetMi();
            LMsetV();
            setMaxEpoch();
            setMSE();
            setHoldout();
            setDatasetStructure();
            setSampleSize();
            LMsetSingularMatrixProceeding();
            LMsetUseGenSet();
            configured = true;
        }

        private static void LMsetMi()
        {
            Console.WriteLine("Podaj domyślną wartość wspolczynnik tlumienia μ, obecna {0}",
                coefficientMIpar);
            try
            {
                coefficientMIpar = Double.Parse((Console.ReadLine().Replace(".", ",")));
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona wartość poprzednią");
            }
            Console.WriteLine("Obecna wartość μ wynosi: {0}\n", coefficientMIpar);
        }

        private static void LMsetV()
        {
            Console.WriteLine("Podaj wartość wspolczynnika przystosowania V, obecna {0}", adjustmentFactorVpar);
            try
            {
                adjustmentFactorVpar = Double.Parse((Console.ReadLine().Replace(".", ",")));
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona wartość poprzednią");
            }
            Console.WriteLine("Obecna wartość V wynosi: {0}\n", adjustmentFactorVpar);
        }

        private static void LMsetSingularMatrixProceeding()
        {
            Console.WriteLine("W jaki sposób ma odbywać się postępowanie w przypadku macierzy osobliwych?");
            Console.WriteLine("[1]{0} Regularyzacja macierzy ",
                (proceedingWithSingular == SingularMatrixProceeding.Reg) ? "[Opcja obecna]" : "");
            Console.WriteLine("[2]{0} Rozkład według wartości osobliwych SVD",
                (proceedingWithSingular == SingularMatrixProceeding.SVD) ? "[Opcja obecna]" : "");
            Console.WriteLine("[3]{0} Uogólniona macierz odwrotna \"pseudoinwersja\"",
                (proceedingWithSingular == SingularMatrixProceeding.PINV) ? "[Opcja obecna]" : "");
            Console.WriteLine("[4]{0} Brak \n",
                (proceedingWithSingular == SingularMatrixProceeding.None) ? "[Opcja obecna]" : "");
            try
            {
                int opt = Int32.Parse(Console.ReadLine());

                if (Enum.IsDefined(typeof(SingularMatrixProceeding), --opt))
                {
                    proceedingWithSingular = (SingularMatrixProceeding)opt;
                }
                else
                {
                    Console.WriteLine("Niepoprawna wartość");
                }
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona wartość poprzednią");
            }
            Console.WriteLine("Obecnie ustawiono: {0}\n", Enum.GetName(typeof(SingularMatrixProceeding),
                proceedingWithSingular));
        }

        private static void LMsetUseGenSet()
        {
            char useAdditionalSetToo = 'u';
            Console.WriteLine("Czy przy ustalaniu zmian wag algorytm LM ma zwracać także uwagę "
                + "na celność zbioru dodatkowego (AdditionalSet)? (t/N) Obecnie: {0}\n", LMuseDoubleSet ? "True" : "False");
            Console.Write("Domyślnie wyłączona opcja. \n (t/N)\t");
            try
            {
                useAdditionalSetToo = Char.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość/lub [Enter], opcja ustawiona na poprzednią");
            }
            if (useAdditionalSetToo == 't' || useAdditionalSetToo == 'T')
            {
                Console.WriteLine("Opcja ustawiona na True");
                LMuseDoubleSet = true;
                setTolerance();
            }
            else if (useAdditionalSetToo == 'n' || useAdditionalSetToo == 'N')
            {
                Console.WriteLine("Opcja ustawiona na False");
                LMuseDoubleSet = false;
            }
            Console.WriteLine("UseAdditionalSet: {0}\n", LMuseDoubleSet ? "True" : "False");
        }

        private static void setTolerance()
        {
            double defaultTolerance = 0.0;
            Console.WriteLine("Podaj współczynnik tolerancji, obecny: {0}\n Wartości (-1, 1)", tolerancePar);
            try
            {
                tolerancePar = Double.Parse((Console.ReadLine().Replace(".", ",")));
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią");
            }
            if (!(tolerancePar <= 1 + Double.Epsilon && tolerancePar >= -1 - Double.Epsilon))
            {
                Console.WriteLine("t wynosić powinno pomiędzy <-1, 1>");
                Console.WriteLine("ustawiona na domyślną ({0})", defaultTolerance);
                tolerancePar = defaultTolerance;
            }
            Console.WriteLine("Współczynik tolerancji: t = {0}\n", tolerancePar);
        }

        private static void LMCreateNN()
        {
            if (configured)
                networkTrainerLM = new TrainerLMImproved(dataset, datasetStructurePar, holdoutPercentagePar,
                    hiddenRatioPar, maxEpochsPar, desiredMSEPar, coefficientMIpar, adjustmentFactorVpar,
                    sampleSizePar, proceedingWithSingular, LMuseDoubleSet, tolerancePar, runAutomated);
            else
                networkTrainerLM = new TrainerLMImproved(dataset);

            readyToTrain = true;
        }

        private static void LMStart()
        {
            if (networkTrainerLM.TrainNetwork())
            {
                Console.WriteLine("\nZakończono trenowanie sieci neuronowej!");
            }
            else
            {
                Console.WriteLine("\nZakończono trenowanie sieci neuronowej! (Niepowodzenie)");
            }
            readyToTrain = false;
        }

        private static void LMEnd()
        {
            Console.WriteLine("Moduł zakończy działanie, naciśnij dowolny przycisk...");
            LMendFlag = true;
        }

        #endregion
    }
}
