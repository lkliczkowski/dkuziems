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
                Console.WriteLine("Część programu do automatycznego testowania nauczania.\n" + 
                    "W celu konfiguracji należy dokonać zmian w kodzie");
                Menu AMenu = new Menu();
                AMenu.Add("Algorytm wstecznej propagacji błędów", automatedRunBP);
                AMenu.Add("Algorytm Levenberga-Marquardta", automatedRunLM);
                AMenu.Add("BP dla wszystkich zbiorów danych", fullAutomatedRunBP);
                AMenu.Add("LM dla wszystkich zbiorów danych", fullAutomatedRunLM);
                AMenu.Add("Wróć do Menu głównego", AEnd);

                AMenu.Show();
            }
        }



        private static void setTestW80S20()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 80;
                sampleSizePar = 3;
            }
            else
            {
                holdoutPercentagePar = 80;
                sampleSizePar = 20;
            }
            datasetStructurePar = EnumDatasetStructures.Windowed;
        }

        private static void setTestW50S20()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 95;
                sampleSizePar = 3;
            }
            else
            {
                holdoutPercentagePar = 50;
                sampleSizePar = 20;
            }

            datasetStructurePar = EnumDatasetStructures.Windowed;
        }

        private static void setTestV80S20()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                sampleSizePar = 3;
            }
            else
            {
                sampleSizePar = 20;
            }
            holdoutPercentagePar = 80;
            datasetStructurePar = EnumDatasetStructures.WindowedNoRandom;
        }

        private static void setTestV50S20()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 95;
                sampleSizePar = 3;
            }
            else
            {
                holdoutPercentagePar = 50;
                sampleSizePar = 20;
            }

            datasetStructurePar = EnumDatasetStructures.WindowedNoRandom;
        }

        private static void setTestV85S20()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 90;
                sampleSizePar = 3;
            }
            else
            {
                holdoutPercentagePar = 85;
                sampleSizePar = 20;
            }

            datasetStructurePar = EnumDatasetStructures.WindowedNoRandom;
        }

        private static void setTestS80()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 80;
            }
            else
            {
                holdoutPercentagePar = 80;
            }
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestS50()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 95;
            }
            else
            {
                holdoutPercentagePar = 50;
            }

            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestS60()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 94;
            }
            else
            {
                holdoutPercentagePar = 60;
            }

            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestS40PINV()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 94;
            }
            else
            {
                holdoutPercentagePar = 40;
            }

            proceedingWithSingular = SingularMatrixProceeding.PINV;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestS40SVD()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 94;
            }
            else
            {
                holdoutPercentagePar = 40;
            }

            proceedingWithSingular = SingularMatrixProceeding.SVD;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestS85()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 90;
            }
            else
            {
                holdoutPercentagePar = 85;
            }

            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestW95S3()
        {
            holdoutPercentagePar = 95;
            sampleSizePar = 3;
            datasetStructurePar = EnumDatasetStructures.Windowed;
        }


        private static void setTestMIoSVDtoV80()
        {
            holdoutPercentagePar = 80;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.WindowedNoRandom;
            proceedingWithSingular = SingularMatrixProceeding.SVD;
        }

        private static void setTestMIoSVDtoV50()
        {
            holdoutPercentagePar = 50;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.WindowedNoRandom;
            proceedingWithSingular = SingularMatrixProceeding.SVD;
        }

        private static void setTestMIoPINVtoV80()
        {
            holdoutPercentagePar = 80;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.WindowedNoRandom;
            proceedingWithSingular = SingularMatrixProceeding.PINV;
        }

        private static void setTestMIoPINVtoV50()
        {
            holdoutPercentagePar = 50;
            sampleSizePar = 20;
            datasetStructurePar = EnumDatasetStructures.WindowedNoRandom;
            proceedingWithSingular = SingularMatrixProceeding.PINV;
        }

        private static void setTestD80()
        {
            holdoutPercentagePar = 80;
            LMuseDoubleSet = true;
            maxEpochsPar = 50;
        }

        private static void setTestD50()
        {
            holdoutPercentagePar = 50;
            LMuseDoubleSet = true;
            maxEpochsPar = 50;
        }

        private static void setTestD85()
        {
            if (dataType == EnumDataTypes.LetterRecognitionA)
            {
                holdoutPercentagePar = 90;
            }
            else
            {
                holdoutPercentagePar = 85;
            }
            LMuseDoubleSet = true;
            maxEpochsPar = 50;
        }

        #region normal auto
        private static void automatedRunBP()
        {
            int[] hiddenRatioTest = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            int[] hiddenRatioTest2 = new int[] { 0, 2, 4, 6, 8, 10, 12 };

            double[] learningRateTest =
                new double[] { 0.001, 0.0025, 0.005,
                    0.01, 0.025, 0.05,
                    0.1, 0.25, 0.5};

            setOptionsToDefault();
            setTestV80S20();
            selectDataForAutomated();
            prepareData();
            configured = runAutomated = true;

            foreach (int n in hiddenRatioTest2)
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
            int[] hiddenRatioTest = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            int[] hiddenRatioTest2 = new int[] { 4, 6, 8, 10, 12 };
            int[] hiddenRatioTest3 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };

            double[] coefficientMITest = new double[] { 0.01, 0.1, 10 };
            double[] coefficientMITest2 = new double[] { 0.02 };

            int[] adjustmentFactorVTableTest = new int[] { 5, 10, 20 };
            int[] adjustmentFactorVTableTest2 = new int[] { 10 };

            setOptionsToDefault();
            selectDataForAutomated();
            prepareData();
            configured = runAutomated = true;
            setTestV50S20();


            foreach (int n in hiddenRatioTest3)
            {
                hiddenRatioPar = n;
                foreach (double m in coefficientMITest2)
                {
                    coefficientMIpar = m;

                    foreach (int v in adjustmentFactorVTableTest2)
                    {
                        adjustmentFactorVpar = v;
                        LMCreateNN();
                        LMStart();
                    }
                }
            }
        }
        #endregion

        #region full auto
        private static void fullAutomatedRunBP()
        {
            int[] hiddenNumTest1 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            int[] hiddenNumTest2 = new int[] { 0, 2, 4, 6, 8, 10, 12 };
            int[] hiddenNumTest3 = new int[] { 6, 7, 8, 9, 10 };

            int[] hiddenNumTest4 = new int[] { 0, 1, 2, 3, 4, 5 };

            double[] learningRateTest1 =
                new double[] { 0.001, 0.0025, 0.005,
                    0.01, 0.025, 0.05,
                    0.1, 0.25, 0.5};

            double[] learningRateTest4 =
                new double[] { 0.001, 0.0015, 0.0025, 0.003, 0.0035, 0.005,
                    0.01, 0.015, 0.025, 0.03, 0.035, 0.05,
                    0.1, 0.15, 0.25, 0.3, 0.35, 0.5};

            int[] hiddenUsed = hiddenNumTest1;
            double[] lrUsed = learningRateTest1;

            for (int i = 1; i <= 4; i++)
            {
                if (i == 2)
                    continue;

                setOptionsToDefault();

                switch (i)
                {
                    case 1:
                        SetToHeartDisease();
                        break;
                    case 2:
                        SetToGermanCreditData();
                        break;
                    case 3:
                        SetToCreditRisk();
                        break;
                    case 4:
                        SetToLetterRecognition();
                        break;
                    default:
                        return;
                }


                //=======================================================================
                //=======================================================================
                setTestV85S20();
                //=======================================================================
                //=======================================================================
                prepareData();
                configured = runAutomated = true;

                foreach (int n in hiddenUsed)
                {
                    hiddenRatioPar = n;

                    if (dataType == EnumDataTypes.GermanCreditData && n > 12)
                        continue;

                    //foreach (double l in lrUsed)
                    for (double l = 0.02; l < 0.999; l += 0.02)
                    {
                        learningRatePar = l;
                        BPCreateNN();
                        BPstart();
                    }
                }
            }

            Console.WriteLine("\a");
        }

        private static void fullAutomatedRunLM()
        {
            int[] hiddenRatioTest1 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int[] hiddenRatioTest2 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            int[] hiddenRatioTest3 = new int[] { 6, 7, 8, 9, 10 };

            int[] hiddenRatioTest4 = new int[] { 0, 1, 2, 3, 4, 5 };

            double[] coefficientMITest1 = new double[] { 0.01, 0.1, 10 };
            double[] coefficientMITest2 = new double[] { 10, 100 };
            double[] coefficientMITest3 = new double[] { 0.002, 0.0002, 0, 00002 };
            double[] coefficientMITest4 = new double[] { 0.01, 0.02, 0.05,
                                                         0.1, 0.2, 0.5};

            double[] tolerances = new double[] { 0.1, 0.15, 0.2, 0.25 };
            double[] tolerances2 = new double[] { 0.1 };

            int[] adjustmentFactorVTableTest1 = new int[] { 5, 10, 20 };
            int[] adjustmentFactorVTableTest2 = new int[] { 2, 5 };
            int[] adjustmentFactorVTableTest3 = new int[] { 10 };
            int[] adjustmentFactorVTableTest4 = new int[] { 2, 5, 10 };

            int[] ratioUsed = hiddenRatioTest4;
            double[] miUsedInTest = coefficientMITest1;
            int[] vUsedInTest = adjustmentFactorVTableTest1;
            double[] tolUsed = tolerances2;

            for (int i = 1; i <= 4; i++)
            {
                setOptionsToDefault();

                switch (i)
                {
                    case 1:
                        SetToHeartDisease();
                        break;
                    case 2:
                        SetToGermanCreditData();
                        break;
                    case 3:
                        SetToCreditRisk();
                        break;
                    case 4:
                        SetToLetterRecognition();
                        break;
                    default:
                        continue;
                }

                prepareData();
                configured = runAutomated = true;

                //=======================================================================
                //=======================================================================
                setTestD85();
                //=======================================================================
                //=======================================================================
                foreach (int n in ratioUsed)
                //for (int n = 0; n < 25; n++ )
                {
                    hiddenRatioPar = n;

                    //if (dataType == EnumDataTypes.GermanCreditData && n > 12)
                    //    continue;

                    //foreach (double m in miUsedInTest)
                    //for (double m = 0.005; m < 1.01; m += 0.05)
                    //for (double m = 100; m > 0.0001; m *= 0.9 )
                    for (double m = 100; m > 0.0001; m *= 0.75)
                    {
                        coefficientMIpar = m;

                        //foreach (int v in vUsedInTest)
                        for (double v = 5; v < 16; v += 5)
                        {
                            adjustmentFactorVpar = v;
                            foreach (double t in tolUsed)
                            {
                                tolerancePar = t;
                                LMCreateNN();
                                LMStart();
                            }
                        }
                    }
                }
            }

            Console.WriteLine("\a");
        }
        #endregion

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
