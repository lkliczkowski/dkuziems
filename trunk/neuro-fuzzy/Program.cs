using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimplifiedFuzzyRules;

namespace ProjektNeuroFuzzy
{
    class Program
    {
        static void Main(string[] args)
        {
            bool end = false;
            while (!end)
            {
                int method;
                while (!selectMethod(out method)) {}

                switch(method)
                {
                    case 1:
                        Console.Clear();
                        SimplifiedFuzzyRules.SimplifiedFuzzyRules.SimplifiedFuzzyRulesTest();
                        break;
                    case 2:
                        Console.Clear();
                        FunahashiNeuralNetwork.BackpropagationTest.BackpropagationLearningTest();
                        break;

                    case 3:
                    default:
                        end = true;
                        break;
                }
            }
            Console.WriteLine("Program zakończy działanie...");
            Console.ReadKey();
        }

        private static bool selectMethod(out int option)
        {
            Console.WriteLine("Zadanie aproksymacji, wybierz metodę:\n");
            Console.WriteLine("1)  algorytm generujacy zbiór prostych reguł rozmytych zgodnie {0}",
                "\n\tz opisem z punktu 4.3.1 z notatek,");
            Console.WriteLine("2)  algorytm propagacji wstecznej,");
            Console.WriteLine("\n3)  zakończ.\n");
            try
            {
                int op = Int32.Parse(Console.ReadLine());
                if (op > 0 && op < 4)
                {
                    option = op;
                    return true;
                }
                else
                {
                    throw new System.ArgumentException("Opcje od 1-3");
                }
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine("Niepoprawna wartość ({0})", e.Message);
            }
            option = 0;
            return false;
        }
    }
}
