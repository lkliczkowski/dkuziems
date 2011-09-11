using System;
using ZScore;

namespace LearningBPandLM
{
    partial class Program
    {
        #region parametry programu

        private static string inputFile, outputFile = "data.txt";
        private static ZScoreData dataset;
        private static EnumDataTypes dataType;
        private static TrainerLMImproved networkTrainerLM;
        private static TrainerBP networkTrainerBP;

        //flagi menu
        private static bool endFlag, BPendFlag, LMendFlag, AMenuFlag,
            readyToZScore, readyToCreateNN, readyToTrain, configured, 
            LMuseDoubleSet, runAutomated;

        //parametry konfiguracyjne
        private static int holdoutPercentagePar, sampleSizePar, hiddenRatioPar;
        private static double learningRatePar, desiredMSEPar, coefficientMIpar, 
            adjustmentFactorVpar, tolerancePar;
        private static ulong maxEpochsPar;
        private static EnumDatasetStructures datasetStructurePar;
        private static SingularMatrixProceeding proceedingWithSingular;

        const string VER = "0.94";

        #endregion

        #region menu glowne

        static void Main(string[] args)
        {
            PrintInfo("Menu główne programu");
            programInfo();
            while (!endFlag)
            {
                setOptionsToDefault();
                Menu MainMenu = new Menu();
                MainMenu.Add("Algorytm wstecznej propagacji błędów", BPAlgorithm);
                MainMenu.Add("Algorytm Levenberga-Marquardta", LMAlgorithm);
                MainMenu.Add("Automatyczne testy", automatedRun);
                MainMenu.Add("Tworzenie statystyk plików wynikowych i skryptów gnuplot", StatisticsConfig.StatisticsMenu);

                MainMenu.Add("Zakończ działanie programu", MainEnd);

                PrintLongLine();
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
