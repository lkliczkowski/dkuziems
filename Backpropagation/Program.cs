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
            //ZScore.ZScore heartDiseaseData = new ZScore.ZScore("HeartDiseaseShort.csv", "Normalized.csv", ZScore.EnumDataTypes.HeartDisease);
            //heartDiseaseData.NormalizeHeartDisease();

            
            //TEST
            float learningRate = 0.8f;
            Backpropagation bp = new Backpropagation(learningRate, 3, 2, 1);

            bp.Stats();
            bp.Run();
            bp.Stats();



            Console.ReadKey();
        }


    }
      
}
