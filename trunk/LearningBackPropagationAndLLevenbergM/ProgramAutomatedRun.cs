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
            //automatedRunBP(EnumDataTypes.LetterRecognitionA);
        }

        private static void automatedRunBP()
        {
            int hiddenRatioFrom = 2,
                hiddenRatioTo = 10;
            double learningRateFrom = 0.0001,
                learningRateTo = 0.9;
            int learningRateMultiplicity = 50;

            setOptionsToDefault();
            selectDataForAutomated();
            prepareData();
            configured = runAutomated = true;

            for (int n = hiddenRatioFrom; n < hiddenRatioTo; n++)
            {
                hiddenNodeRatioPar = n;

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
            int hiddenRatioFrom = 2,
                hiddenRatioTo = 10;
            double coefficientMIFrom = 0.01, 
                coefficientMITo = 0.1;
            int coefficientMIMultiplicity = 3;

            int adjustmentFactorVFrom = 5,
                adjustmentFactorVTo = 11,
                adjustmentFactorVMultiplicity = 2;
            
            setOptionsToDefault();
            sampleSizePar = 30;
            selectDataForAutomated();
            prepareData();
            configured = runAutomated = true;

            for (int n = hiddenRatioFrom; n < hiddenRatioTo; n++)
            {
                hiddenNodeRatioPar = n;

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

        }

        private static void selectDataForAutomated()
        {
            while (!readyToZScore)
            {
                Console.WriteLine("Wybierz zestaw danych");
                Menu selectData = new Menu();
                selectData.Add("[wybor danych] Ustaw Opcje dla HeartDisease", SetToHeartDisease);
                selectData.Add("[wybor danych] Ustaw Opcje dla GermanCreditData", SetToGermanCreditData);
                selectData.Add("[wybor danych] Ustaw Opcje dla LetterRecognition", SetToLetterRecognition);
                selectData.Add("[wybor danych] Ustaw Opcje dla CreditRisk", SetToCreditRisk);

                selectData.Show();
            }
        }

        private static void AEnd()
        {
            AMenuFlag = true;
        }
    }
}
