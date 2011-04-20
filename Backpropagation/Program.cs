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
            ZScore.ZScore heartDiseaseData = new ZScore.ZScore();
            heartDiseaseData.NormalizeHeartDisease();


            Backpropagation nnForHeartDisease = new Backpropagation(heartDiseaseData, 30, 0.8f);

            nnForHeartDisease.Stats();
            nnForHeartDisease.Run();
            nnForHeartDisease.Stats();
            //nnForHeartDisease.
            //GrowingDatasetOperate heartDiseaseDataIndexes = new GrowingDatasetOperate(heartDiseaseData.Data[0].GetNum());
            //GrowingDatasetOperate heartDiseaseDataIndexes = new GrowingDatasetOperate(10000);



            //TEST


            //Console.ReadKey();
        }
    }
      
}
