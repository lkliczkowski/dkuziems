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

        private static void setTest90()
        {
            holdoutPercentagePar = 90;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTest99()
        {
            holdoutPercentagePar = 99;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTest95()
        {
            holdoutPercentagePar = 95;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTest70()
        {
            holdoutPercentagePar = 70;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTest97()
        {
            holdoutPercentagePar = 97;
            datasetStructurePar = EnumDatasetStructures.Simple;
        }

        private static void setTestMIoSVD()
        {
            holdoutPercentagePar = 70;
            proceedingWithSingular = SingularMatrixProceeding.SVD;
        }

        private static void setTestMIoPINV()
        {
            holdoutPercentagePar = 70;
            proceedingWithSingular = SingularMatrixProceeding.PINV;
        }
        private static void automatedRunBP()
        {
            int hiddenNumberFrom = 2,
                hiddenNumberTo = 33;
            double learningRateFrom = 0.005,
                learningRateTo = 0.9;
            int learningRateMultiplicity = 3;

            setOptionsToDefault();
            setTest95();
            selectDataForAutomated();
            prepareData();
            configured = runAutomated = true;

            for (int n = hiddenNumberFrom; n < hiddenNumberTo; n++)
            {
                hiddenNumberPar = n;

                for (double l = learningRateFrom; l < learningRateTo; l *= learningRateMultiplicity)
                {
                    learningRatePar = l;
                    BPCreateNN();
                    BPstart();
                }
            }
        }

        private static void automatedRunLM()
        {
            int hiddenNumberFrom = 2,
                hiddenNumberTo = 21,
                hiddenHitUp = 13;
            double coefficientMIFrom = 0.0001,
                coefficientMITo = 100.01,
                afterNormalHitMI = 1;
            int coefficientMIMultiplicity = 100;

            int adjustmentFactorVFrom = 5,
                adjustmentFactorVTo = 16,
                afterNormalHitV = 10,
                adjustmentFactorVMultiplicity = 2;

            setOptionsToDefault();
            setTest99();
            selectDataForAutomated();
            prepareData();
            configured = runAutomated = true;

            for (int n = hiddenNumberFrom; n < hiddenNumberTo; n++)
            {
                hiddenNumberPar = n;
                if (hiddenNumberPar < hiddenHitUp)
                {
                    for (double m = coefficientMIFrom; m < coefficientMITo; m *= coefficientMIMultiplicity)
                    {
                        coefficientMIpar = m;

                        for (int v = adjustmentFactorVFrom; v < adjustmentFactorVTo; v *= adjustmentFactorVMultiplicity)
                        {
                            adjustmentFactorVpar = v;
                            LMCreateNN();
                            LMStart();
                        }
                    }
                }
                else
                {
                    coefficientMIpar = afterNormalHitMI;
                    adjustmentFactorVpar = afterNormalHitV;
                    LMCreateNN();
                    LMStart();
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
