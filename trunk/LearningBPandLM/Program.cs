﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LearningBPandLM
{
    class Program
    {

        private static string inputFile, outputFile;
        private static ZScore.ZScore dataset;
        private static ZScore.EnumDataTypes dataType;
        private static TrainerLMImproved networkTrainerLM;
        private static BPlearning nT;

        //flagi menu
        private static bool endFlag, BPendFlag, LMendFlag, readyToZScore, readyToCreateNN, 
            readyToBackpropagate, configured;

        //parametry konfiguracyjne
        private static int hiddenNodeRatioPar;
        private static double learningRatePar = 0.001, desiredAccuracyPar;
        private static ulong maxEpochsPar;

        static void Main(string[] args)
        {
            setOptionsToDefault();
            SetToCreditRisk();
            prepareData();
            /*
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
             */
            Console.ReadKey();
        }

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

            //parametry konfiguracyjne
            hiddenNodeRatioPar = 4;
            desiredAccuracyPar = 99;
            maxEpochsPar = 1500;
        }

        private static void BPAlgorithm()
        {
            while (!BPendFlag)
            {
                PrintInfo("Menu główne");
                Menu menu = new Menu();
                menu.Add("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", quickStart);
                menu.Add("[wybor danych] Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                menu.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                menu.Add("Wyświetl konfigurację", ShowConfig);
                menu.Add("Zakończ", BPEnd);

                if (readyToZScore)
                {
                    menu.Remove("Zakończ", BPEnd);
                    menu.Add(">> Standaryzacja danych", prepareData);
                    menu.Add("Zakończ", BPEnd);
                }

                if (readyToCreateNN)
                {
                    menu.Remove("Zakończ", BPEnd);
                    menu.Add(String.Format(">> Konfiguruj sieć:\n\t\t" +
                        "- liczba neuronów w warstwie ukrytej,\n\t\t" +
                        "- współczynnik uczenia,\n\t\t" +
                        "- maksymalną liczbę epok,\n\t\t" +
                        "- docelowa dokładność modelu."), BPSetConfig);
                    menu.Add(">> Utworz nową sieć neuronową oraz zbiory trenujący i walidacyjny", BPCreateNN);

                    menu.Remove("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", quickStart);
                    menu.Remove("Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                    menu.Remove("Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                    menu.Add("Zakończ", BPEnd);
                }

                if (readyToBackpropagate)
                {
                    menu.Remove("Zakończ", BPEnd);
                    menu.Add(">> Rozpocznij/kontynuuj działanie sieci", BPstart);
                    menu.Add("Zakończ", BPEnd);
                }

                menu.Show();
            }

            Console.ReadKey();
        }

        private static void LMAlgorithm()
        {
            while (!LMendFlag)
            {
                PrintInfo("Menu główne algorytm Levenberga-Marquardta");
                Menu LMmenu = new Menu();
                LMmenu.Add("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", LMquickStart);
                LMmenu.Add("[wybor danych] Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                LMmenu.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                LMmenu.Add("Wyświetl konfigurację", ShowConfig);
                LMmenu.Add("Zakończ", LMEnd);

                if (readyToZScore)
                {
                    LMmenu.Remove("Zakończ", LMEnd);
                    LMmenu.Add(">> Standaryzacja danych", prepareData);
                    LMmenu.Add("Zakończ", LMEnd);
                }

                if (readyToCreateNN)
                {
                    LMmenu.Remove("Zakończ", LMEnd);
                    LMmenu.Add(String.Format(">> Konfiguruj sieć:\n\t\t" +
                        "- liczba neuronów w warstwie ukrytej,\n\t\t" +
                        "- maksymalną liczbę epok,\n\t\t" +
                        "- docelowa dokładność modelu."), LMSetConfig);
                    LMmenu.Add(">> Utworz nową sieć neuronową oraz zbiory trenujący i walidacyjny", LMCreateNN);

                    LMmenu.Remove("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", LMquickStart);
                    LMmenu.Remove("Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                    LMmenu.Remove("Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                    LMmenu.Add("Zakończ", LMEnd);
                }

                if (readyToBackpropagate)
                {
                    LMmenu.Remove("Zakończ", LMEnd);
                    LMmenu.Add(">> Rozpocznij/kontynuuj działanie sieci", LMStart);
                    LMmenu.Add("Zakończ", LMEnd);
                }

                LMmenu.Show();
            }

            Console.ReadKey();
        }

        #region funkcje konfiguracji, menu

        private static void quickStart()
        {
            SetToLetterRecognition();
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

        private static void LMquickStart()
        {
            SetToLetterRecognition();
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

        private static void SetToHeartDisease()
        {
            inputFile = "HeartDisease.csv";
            dataType = ZScore.EnumDataTypes.HeartDisease;
            readyToZScore = true;
        }

        private static void SetToLetterRecognition()
        {
            inputFile = "letter-a-recognition.csv";
            dataType = ZScore.EnumDataTypes.LetterRecognitionA;
            readyToZScore = true;
        }

        private static void SetToCreditRisk()
        {
            inputFile = "CreditRisk.csv";
            outputFile = "normalizedCreditRisk.csv";
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

        private static void BPSetConfig()
        {
            double learningRateDefault = 0.001,
                desiredAccuracyDefault = 99;
            ulong maxEpochsDefault = 1500;

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


            Console.WriteLine("Podaj współczynnik uczenia, obecny: {0}", learningRatePar);
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
                Console.WriteLine("Niepoprawna wartość, ustawiona na domyślną ({0})", maxEpochsDefault);
                maxEpochsPar = maxEpochsDefault;
            }
            Console.WriteLine("Obecna maksymalna ilość epok: {0}\n", maxEpochsPar);

            Console.WriteLine("Podaj docelową dokładność (w %) modelu, obecna: {0}%", desiredAccuracyPar);
            try
            {
                desiredAccuracyPar = Double.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona automatycznie ({0}%)", desiredAccuracyDefault);
                desiredAccuracyPar = desiredAccuracyDefault;
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

        private static void LMSetConfig()
        {
            double desiredAccuracyDefault = 99;
            ulong maxEpochsDefault = 1500;

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

            Console.WriteLine("Podaj maksymalną ilość epok, obecna: {0}", maxEpochsPar);
            try
            {
                maxEpochsPar = ulong.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona na domyślną ({0})", maxEpochsDefault);
                maxEpochsPar = maxEpochsDefault;
            }
            Console.WriteLine("Obecna maksymalna ilość epok: {0}\n", maxEpochsPar);

            Console.WriteLine("Podaj docelową dokładność (w %) modelu, obecna: {0}%", desiredAccuracyPar);
            try
            {
                desiredAccuracyPar = Double.Parse(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Niepoprawna wartość, ustawiona automatycznie ({0}%)", desiredAccuracyDefault);
                desiredAccuracyPar = desiredAccuracyDefault;
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
                nT = new BPlearning(dataset, hiddenNodeRatioPar, learningRatePar, maxEpochsPar, desiredAccuracyPar);
            else
                nT = new BPlearning(dataset);

            readyToBackpropagate = true;
        }

        private static void LMCreateNN()
        {
            if (configured)
                networkTrainerLM = new TrainerLMImproved(dataset, hiddenNodeRatioPar, maxEpochsPar, desiredAccuracyPar);
            else
                networkTrainerLM = new TrainerLMImproved(dataset);

            readyToBackpropagate = true;
        }

        private static void BPstart()
        {
            nT.TrainNetwork();
            nT.ShowOptions();
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

        private static void MainEnd()
        {
            Console.WriteLine("Program zakończy działanie, naciśnij dowolny przycisk...");
            endFlag = true;
        }

        private static void BPEnd()
        {
            Console.WriteLine("Moduł zakończy działanie, naciśnij dowolny przycisk...");
            BPendFlag = true;
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