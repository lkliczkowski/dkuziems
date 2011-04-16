using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Backpropagation
{
    class Program
    {
        static void Main(string[] args)
        {
            ZScore.ZScore heartDiseaseData = new ZScore.ZScore("HeartDisease.csv", "Normalized.csv", ZScore.EnumDataTypes.HeartDisease);
            heartDiseaseData.NormalizeHeartDisease();

            Print(heartDiseaseData.Data[0].GetNum().ToString());

            SetsOperate temp = new SetsOperate(20
                );
            //TEST
            //float learningRate = 0.8f;
            //Backpropagation bp = new Backpropagation(learningRate, 3, 2, 1);

            //bp.Stats();
            //bp.Run();
            //bp.Stats();



            Console.ReadKey();
        }

        protected static void Print(float[] toPrint)
        {
            foreach (float f in toPrint)
                Console.Write("{0:N2}\t", f);
            Console.WriteLine();
        }

        protected static void Print(string toPrint, string par)
        {
            Console.WriteLine(">> {0} :: {1}", toPrint, par);
        }

        protected static void Print(string toPrint)
        {
            Console.WriteLine(">> {0}", toPrint);
        }

        protected static void Print()
        {
            Console.WriteLine();
        }
    }
      
}
