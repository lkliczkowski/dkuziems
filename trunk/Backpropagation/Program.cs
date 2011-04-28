using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backpropagation
{
    class Program
    {
        static string inputFile = "";
        static ZScore.ZScore dataset;
        static ZScore.EnumDataTypes dataType = ZScore.EnumDataTypes.unknown;
        static neuralNetworkTrainer nT;

        //flagi menu
        static bool end = false, 
            readyToZScore = false, 
            readyToCreateNN = false, 
            readyToBackpropagate = false,
            configured = false;

        //parametry konfiguracyjne
        static int hiddenNodeRatioPar = 4;
        static double learningRatePar = 0.001, desiredAccuracyPar = 99;
        static ulong maxEpochsPar = 1500;

        static void Main(string[] args)
        {
            while (!end)
            {
                PrintInfo("Menu główne");
                Menu menu = new Menu();
                menu.Add("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", quickStart);
                menu.Add("[wybor danych] Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                menu.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                menu.Add("Wyświetl konfigurację", ShowConfig);
                menu.Add("Zakończ", End);

                if (readyToZScore)
                {
                    menu.Remove("Zakończ", End);
                    menu.Add(">> Standaryzacja danych", prepareData);
                    menu.Add("Zakończ", End);
                }

                if (readyToCreateNN)
                {
                    menu.Remove("Zakończ", End);
                    menu.Add(String.Format(">> Konfiguruj sieć:\n\t\t" +
                        "- liczba neuronów w warstwie ukrytej,\n\t\t" +
                        "- współczynnik uczenia,\n\t\t" +
                        "- maksymalną liczbę epok,\n\t\t" +
                        "- docelowa dokładność modelu."), SetConfig);
                    menu.Add(">> Utworz sieć neuronową oraz zbiory trenujący i walidacyjny", CreateNN);

                    menu.Remove("Wystartuj sieć dla domyślnego zestawu danych i konfiguracji", quickStart);
                    menu.Remove("Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                    menu.Remove("Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                    menu.Add("Zakończ", End);
                }

                if (readyToBackpropagate)
                {
                    menu.Remove("Zakończ", End);
                    menu.Add(">> Rozpocznij działanie sieci", Backpropagate);
                    menu.Add("Zakończ", End);
                }

                menu.Show();
            }

            Console.ReadKey();
        }

        #region funkcje konfiguracji, menu

        public static void quickStart()
        {
            SetToLetterRecognition();
            prepareData();
            CreateNN();
            Backpropagate();
        }

        public static void SetToHeartDisease()
        {
            inputFile = "HeartDisease.csv";
            dataType = ZScore.EnumDataTypes.HeartDisease;
            readyToZScore = true;
        }

        public static void SetToLetterRecognition()
        {
            inputFile = "letter-a-recognition.csv";
            dataType = ZScore.EnumDataTypes.LetterRecognitionA;
            readyToZScore = true;
        }

        public static void prepareData()
        {
            PrintInfo("Przygotowywanie danych");
            dataset = new ZScore.ZScore(inputFile, dataType);
            if(dataset.NormalizeRun())
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

        public static void SetConfig()
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

        public static void CreateNN()
        {
            if (configured)
                nT = new neuralNetworkTrainer(dataset, hiddenNodeRatioPar, learningRatePar, maxEpochsPar, desiredAccuracyPar);
            else
                nT = new neuralNetworkTrainer(dataset);

            readyToBackpropagate = true;
        }

        public static void Backpropagate()
        {
            nT.trainNetwork();
        }

        public static void ShowConfig()
        {
            PrintInfo("Konfiguracja");
            Console.WriteLine("Opcje ustawione dla {0} typu: {1}",
                (inputFile=="")?"brak":inputFile, Enum.GetName(typeof(ZScore.EnumDataTypes), dataType));

            Console.WriteLine("\nKonfiguracja sieci:{0}", (configured==false)?"\n[nie konfigurowano - wartości domyślne]":"");
            Console.WriteLine("Liczba neuronów w war.ukrytej:\t{0}", hiddenNodeRatioPar);
            Console.WriteLine("Współczynnik uczenia:\t\t{0}", learningRatePar);
            Console.WriteLine("Maksymalna liczba epok:\t\t{0}", maxEpochsPar);
            Console.WriteLine("Docelowa dokładność modelu:\t{0}\n", desiredAccuracyPar);
        }

        public static void End()
        {
            Console.WriteLine("Program zakończy działanie, naciśnij dowolny przycisk...");
            end = true;
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
