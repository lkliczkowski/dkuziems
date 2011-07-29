using System;
using ZScore;

namespace LearningBPandLM
{
    partial class Program
    {
        #region parametry programu

        private static string inputFile, outputFile;
        private static ZScoreData dataset;
        private static EnumDataTypes dataType;
        private static TrainerLMImproved networkTrainerLM;
        private static TrainerBP networkTrainerBP;

        //flagi menu
        private static bool endFlag, BPendFlag, LMendFlag, AMenuFlag,
            readyToZScore, readyToCreateNN, readyToTrain, configured, 
            LMuseGenSetToo, runAutomated;

        //parametry konfiguracyjne
        private static int hiddenNodeRatioPar, sampleSizePar;
        private static double learningRatePar, desiredMSEPar, 
            coefficientMIpar, adjustmentFactorVpar;
        private static ulong maxEpochsPar;
        private static EnumDatasetStructures datasetStructurePar;
        private static SingularMatrixProceeding proceedingWithSingular;

        const string VER = "0.79";

        #endregion

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
                MainMenu.Add("Automatyczne testy", automatedRun);
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
