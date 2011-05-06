using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImprovedLM
{
    class Program
    {
        static void Main(string[] args)
        {
            //TESTOWE
            int hiddenNodeRatio = 6;
            ulong maxEpochs = 1500;
            double desiredAcc = 100;

            string inputFile = "letter-a-recognition.csv";
            //string inputFile = "HeartDisease.csv";
            //ZScore.EnumDataTypes dataType = ZScore.EnumDataTypes.HeartDisease;
            ZScore.EnumDataTypes dataType = ZScore.EnumDataTypes.LetterRecognitionA;
            Console.WriteLine("Przygotowywanie danych");
            ZScore.ZScore Dataset = new ZScore.ZScore(inputFile, dataType);
            int hiddenNodeRatioPar;
            if (Dataset.NormalizeRun())
            {
                Console.WriteLine("Standaryzacja Z-Score zakończona z powodzeniem!");
                hiddenNodeRatioPar = Dataset.sample(0).Length - 1;

                ImprovedLMTrainer nn = new ImprovedLMTrainer(Dataset, hiddenNodeRatio, maxEpochs, desiredAcc);
                if (nn.trainNetwork())
                {
                    Console.WriteLine("ZAKOŃCZONO TRENOWANIE SIECI NEURONOWEJ!");
                    nn.PrintStatus();
                }
                else
                {
                    Console.WriteLine("ZAKOŃCZONO TRENOWANIE SIECI NEURONOWEJ! (NIEPOWODZENIE)");
                    nn.PrintStatus();
                }

            }
            else
            {
                Console.WriteLine("StandaryzacjaZ (ZScore) nie powiodła się!");
            }

            Console.ReadKey();
        }

        public static void PrintLongLine()
        {
            Console.WriteLine("============================================================================");
        }
    }
}
