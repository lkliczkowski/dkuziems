using System;

namespace LearningBPandLM
{
    class Program
    {

        private static string inputFile, outputFile;
        private static ZScore.ZScore dataset;
        private static ZScore.EnumDataTypes dataType;
        private static TrainerLMImproved networkTrainerLM;
        private static TrainerBP networkTrainerBP;

        //flagi menu
        private static bool endFlag, BPendFlag, LMendFlag, readyToZScore, readyToCreateNN, 
            readyToBackpropagate, configured;

        //parametry konfiguracyjne
        private static int hiddenNodeRatioPar;
        private static double learningRatePar, desiredAccuracyPar, coefficientMIpar, adjustmentFactorVpar;
        private static ulong maxEpochsPar;

        #region menu glowne
        
        static void Main(string[] args)
        {
            while (!endFlag)
            {
                setOptionsToDefault();
                PrintInfo("Menu główne programu");
                programInfo();
                Menu MainMenu = new Menu();
                MainMenu.Add("Algorytm wstecznej propagacji błędów", BPAlgorithm);
                MainMenu.Add("Algorytm Levenberga-Marquardta", LMAlgorithm);
                MainMenu.Add("Zakończ działanie programu", MainEnd);


                MainMenu.Show();
            }

            Console.ReadKey();
        }

        private static void MainEnd()
        {
            Console.WriteLine("Program zakończy działanie, naciśnij dowolny przycisk...");
            endFlag = true;
        }
        #endregion

        #region opcje ogolne konfiguracji i danych

        private static void programInfo()
        {
            Console.WriteLine("W programie zaimplementowane są 2 metody nauczania sieci neuronowych, " 
                + "na danych wykonywana jest także standaryzacja, wybierz jedną z poniższych opcji.");
        }

        private static void setOptionsToDefault()
        {
            inputFile = outputFile = "";
            dataType = ZScore.EnumDataTypes.unknown;
            //flagi menu
            endFlag = BPendFlag = LMendFlag = readyToZScore = readyToCreateNN = readyToBackpropagate = configured = false;
            
            //parametry konfiguracji
            hiddenNodeRatioPar = 4;
            desiredAccuracyPar = 99;
            maxEpochsPar = 1500;
            learningRatePar = 0.01;
            coefficientMIpar = 0.00001;
            adjustmentFactorVpar = 10;
        }

        private static void SetToHeartDisease()
        {
            inputFile = "HeartDisease.csv";
            //outputFile = "normalizedHeartDisease.csv";
            dataType = ZScore.EnumDataTypes.HeartDisease;
            readyToZScore = true;
        }

        private static void SetToGermanCreditData()
        {
            inputFile = "GermanCreditData.csv";
            //outputFile = "normalizedGermanCreditData.csv";
            dataType = ZScore.EnumDataTypes.GermanCreditData;
            readyToZScore = true;
        }

        private static void SetToLetterRecognition()
        {
            inputFile = "letter-a-recognition.csv";
            //outputFile = "normalizedLetter-a-recognition.csv";
            dataType = ZScore.EnumDataTypes.LetterRecognitionA;
            readyToZScore = true;
        }

        private static void SetToCreditRisk()
        {
            inputFile = "CreditRisk.csv";
            //outputFile = "normalizedCreditRisk.csv";
            dataType = ZScore.EnumDataTypes.CreditRisk;
            readyToZScore = true;
        }

        private static void prepareData()
        {
            PrintInfo("Przygotowywanie danych");
            if (outputFile.Equals(""))
                dataset = new ZScore.ZScore(inputFile, dataType);
            else
                dataset = new ZScore.ZScore(inputFile, outputFile, dataType);

            if (dataset.NormalizeRun())
            {
                Console.WriteLine("Standaryzacja Z-Score zakończona z powodzeniem!");
                hiddenNodeRatioPar = dataset.sample(0).Length - 1;
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
                (inputFile == "") ? "brak" : inputFile, Enum.GetName(typeof(ZScore.EnumDataTypes), dataType));

            Console.WriteLine("\nKonfiguracja sieci:{0}", (configured == false) ? "\n[nie konfigurowano - wartości domyślne]" : "");
            Console.WriteLine("Liczba neuronów w war.ukrytej:\t{0}", hiddenNodeRatioPar);
            Console.WriteLine("Maksymalna liczba epok:\t\t{0}", maxEpochsPar);
            Console.WriteLine("Docelowa dokładność modelu:\t{0}\n", desiredAccuracyPar);
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
                BPmenu.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                BPmenu.Add("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);
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

                if (readyToBackpropagate)
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
            double learningRateDefault = 0.01,
                desiredAccuracyDefault = 99;

            Console.WriteLine("Podaj liczbę neuronów w warstwie ukrytej, obecna: {0}", hiddenNodeRatioPar);
            try
            {
                hiddenNodeRatioPar = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona automatycznie (liczba WE - 1)");
                hiddenNodeRatioPar = dataset.sample(0).Length - 1;
            }
            Console.WriteLine("Obecna liczba neuronów wynosi: {0}\n", hiddenNodeRatioPar);


            Console.WriteLine("Podaj współczynnik uczenia, obecny: {0}, domyślny: {1}", 
                learningRatePar, learningRateDefault);
            try
            {
                learningRatePar = Double.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na domyślną ({0})", learningRateDefault);
                learningRatePar = learningRateDefault;
            }
            Console.WriteLine("Obecny wspołczynnik uczenia wynosi: {0}\n", learningRatePar);


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

            Console.WriteLine("Podaj docelową dokładność (w %) modelu, obecna: {0}%", desiredAccuracyPar);
            try
            {
                desiredAccuracyPar = Double.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią");
            }
            if (desiredAccuracyPar > 100 || desiredAccuracyPar < 0)
            {
                Console.WriteLine("Niepoprawna wartość, powinna wynosić pomiędzy (0-100)%");
                Console.WriteLine("ustawiona na domyślną ({0}%)", desiredAccuracyDefault);
                desiredAccuracyPar = desiredAccuracyDefault;
            }
            Console.WriteLine("Obecna dokładność docelowa modelu wynosi: {0}%\n", desiredAccuracyPar);

            configured = true;

        }

        private static void BPCreateNN()
        {
            if (configured)
                networkTrainerBP = new TrainerBP(dataset, hiddenNodeRatioPar, learningRatePar, maxEpochsPar, desiredAccuracyPar);
            else
                networkTrainerBP = new TrainerBP(dataset);

            readyToBackpropagate = true;
        }

        private static void BPstart()
        {
            networkTrainerBP.TrainNetwork();
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
                LMmenu.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                LMmenu.Add("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);
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

                if (readyToBackpropagate)
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
            double desiredAccuracyDefault = 99;

            Console.WriteLine("Podaj liczbę neuronów w warstwie ukrytej, obecna: {0}", hiddenNodeRatioPar);
            try
            {
                hiddenNodeRatioPar = Int32.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona automatycznie (liczba WE - 1)");
                hiddenNodeRatioPar = dataset.sample(0).Length - 1;
            }
            Console.WriteLine("Obecna liczba neuronów wynosi: {0}\n", hiddenNodeRatioPar);

            Console.WriteLine("Podaj domyślną wartość wspolczynnik tlumienia μ, obecna {0}", 
                coefficientMIpar);
            try
            {
                coefficientMIpar = Double.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona wartość poprzednią");
            }
            Console.WriteLine("Obecna wartość μ wynosi: {0}\n", coefficientMIpar);

            Console.WriteLine("Podaj wartość wspolczynnika przystosowania V, obecna {0}", adjustmentFactorVpar);
            try
            {
                adjustmentFactorVpar = Double.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona wartość poprzednią");
            }
            Console.WriteLine("Obecna wartość V wynosi: {0}\n", adjustmentFactorVpar);

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

            Console.WriteLine("Podaj docelową dokładność (w %) modelu, obecna: {0}%", desiredAccuracyPar);
            try
            {
                desiredAccuracyPar = Double.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na poprzednią");
            }
            if (desiredAccuracyPar > 100 || desiredAccuracyPar < 0)
            {
                Console.WriteLine("Niepoprawna wartość, powinna wynosić pomiędzy (0-100)%");
                Console.WriteLine("ustawiona na domyślną ({0}%)", desiredAccuracyDefault);
                desiredAccuracyPar = desiredAccuracyDefault;
            }
            Console.WriteLine("Obecna dokładność docelowa modelu wynosi: {0}%\n", desiredAccuracyPar);

            configured = true;

        }

        private static void LMCreateNN()
        {
            if (configured)
                networkTrainerLM = new TrainerLMImproved(dataset, hiddenNodeRatioPar, 
                    maxEpochsPar, desiredAccuracyPar, coefficientMIpar, adjustmentFactorVpar);
            else
                networkTrainerLM = new TrainerLMImproved(dataset);

            readyToBackpropagate = true;
        }

        private static void LMStart()
        {
            if (networkTrainerLM.TrainNetwork())
            {
                Console.WriteLine("Zakończono trenowanie sieci neuronowej!");
            }
            else
            {
                Console.WriteLine("Zakończono trenowanie sieci neuronowej! (Niepowodzenie)");
            }
        }

        private static void LMEnd()
        {
            Console.WriteLine("Moduł zakończy działanie, naciśnij dowolny przycisk...");
            LMendFlag = true;
        }

        #endregion

        #region dodatkowe funkcje, do konsolowego menu
        public static void PrintInfo(string what)
        {
            PrintLongLine();
            Console.Write("==========    {0}  ", what);
            for (int i = 0; i < 60 - what.Length; i++) Console.Write("=");
            Console.WriteLine();
            PrintLongLine();
        }

        public static void PrintLongLine()
        {
            Console.WriteLine("============================================================================");
        }
        #endregion
    }
}
