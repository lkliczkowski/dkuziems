using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZScore;

namespace LearningBPandLM
{
    partial class Program
    {
        private static void automatedRun()
        {
            while (!AMenuFlag)
            {
                PrintInfo("Automatyczne działanie programu");
                Menu AMenu = new Menu();
                AMenu.Add("Algorytm wstecznej propagacji błędów", automatedRunBP);
                AMenu.Add("Algorytm Levenberga-Marquardta", automatedRunLM);
                AMenu.Add("Wróć do Menu głównego", AEnd);

                AMenu.Show();
            }
        }

        private static void setTestS90()
        {
            holdoutPercentagePar = 90;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        /*private static void setTestW95()
        {
            holdoutPercentagePar = 95;
            sampleSizePar = 5;
            datasetStructurePar = EnumDatasetStructures.Windowed;
        }
        */
        private static void setTestW80S3()
        {
            holdoutPercentagePar = 80;
            sampleSizePar = 3;
            datasetStructurePar = EnumDatasetStructures.Windowed;
        }

        private static void setTestW80S20()
        {
            holdoutPercentagePar = 80;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.Windowed;
        }

        private static void setTestW50S20()
        {
            holdoutPercentagePar = 50;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.Windowed;
        }

        private static void setTestW95S3()
        {
            holdoutPercentagePar = 95;
            sampleSizePar = 3;
            datasetStructurePar = EnumDatasetStructures.Windowed;
        }

        private static void setTestS99()
        {
            holdoutPercentagePar = 99;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestS95()
        {
            holdoutPercentagePar = 95;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestS70()
        {
            holdoutPercentagePar = 70;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestS97()
        {
            holdoutPercentagePar = 97;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestMIoSVDW80()
        {
            holdoutPercentagePar = 80;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.Windowed;
            proceedingWithSingular = SingularMatrixProceeding.SVD;
        }

        private static void setTestMIoSVDW50()
        {
            holdoutPercentagePar = 50;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.Windowed;
            proceedingWithSingular = SingularMatrixProceeding.SVD;
        }

        private static void setTestMIoPINVW80()
        {
            holdoutPercentagePar = 80;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.Windowed;
            proceedingWithSingular = SingularMatrixProceeding.PINV;
        }

        private static void setTestMIoPINVW50()
        {
            holdoutPercentagePar = 50;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.Windowed;
            proceedingWithSingular = SingularMatrixProceeding.PINV;
        }

        private static void automatedRunBP()
        {
            int hiddenRatioFrom = 0,//2,
                hiddenRatioTo = 6;//33;
            double[] learningRateTest =
                new double[] { 0.001, 0.0025, 0.005, 0.0075,
                    0.01, 0.025, 0.05, 0.075,
                    0.1, 0.25, 0.5, 0.75};

            setOptionsToDefault();
            setTestW80S20();
            selectDataForAutomated();
            prepareData();
            configured = runAutomated = true;

            for (int n = hiddenRatioFrom; n < hiddenRatioTo; n++)
            {
                hiddenRatioPar = n;

                foreach (double l in learningRateTest)
                {
                    learningRatePar = l;
                    BPCreateNN();
                    BPstart();
                }
            }
        }

        private static void automatedRunLM()
        {
            int hiddenRatioFrom = 0,//2,
                hiddenRatioTo = 4;//21;
            double[] coefficientMITest = new double[] { 0.001, 0.01, 0.1, 10, 100 };

            int[] adjustmentFactorVTableTest = new int[] { 2, 5, 8, 10, 20 };

            setOptionsToDefault();
            selectDataForAutomated();
            prepareData();
            configured = runAutomated = true;
            setTestW80S20();

            for (int n = hiddenRatioFrom; n < hiddenRatioTo; n++)
            {
                hiddenRatioPar = n;
                foreach (double m in coefficientMITest)
                {
                    coefficientMIpar = m;

                    foreach (int v in adjustmentFactorVTableTest)
                    {
                        adjustmentFactorVpar = v;
                        LMCreateNN();
                        LMStart();
                    }
                }
            }
        }

        private static void selectDataForAutomated()
        {
            while (!readyToZScore)
            {
                PrintLongLine();
                Console.WriteLine("Wybierz zestaw danych");
                Menu selectData = new Menu();
                selectData.Add("[wybor danych] Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                selectData.Add("[wybor danych] Ustaw Opcje dla GermanCreditData", SetToGermanCreditData);
                selectData.Add("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);
                selectData.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                selectData.Add("Pokaż ogólne opcje", AShowActualConfig);

                selectData.Show();
            }
        }

        private static void AEnd()
        {
            AMenuFlag = true;
        }

        private static void AShowActualConfig()
        {
            PrintLongLine();
            Console.WriteLine("holdout: {0}", holdoutPercentagePar);
            Console.WriteLine("datasetStructures: {0}",
                Enum.GetName(typeof(EnumDatasetStructures), datasetStructurePar));

            Console.WriteLine("\nLM Only: SingularMatrixProceeding: {0}",
                Enum.GetName(typeof(SingularMatrixProceeding), proceedingWithSingular));
            PrintLongLine();
        }
    }
}
