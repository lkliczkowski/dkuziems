using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ZScore
{
    partial class ZScore
    {
        static void Main(string[] args)
        {
            normalizeHeartDisease();

            //float[] test = new float[9] { 0,0,0,0,0,1,1,1,2 };
            //float[] p_test = ProbabilityDiscrete(test, EnumHeartDisease.LowMediumHigh);
            //Print(p_test);

            Console.ReadKey();
        }

        static void normalizeHeartDisease()
        {
            string FILE1 = "HeartDiseaseShort.csv",
                FILE2 = "Normalized.csv";
            Column<string>[] rawData = ZScore.CSVread(FILE1, ColumnTypes.HeartDisease.Length);

            Console.WriteLine(">>{0}", rawData.Length);
            RemoveFromRecords(ref rawData, 0, 2);

            Column<float>[] discretizedData = ZScoreDiscretize.Discretize
                (rawData, EnumDataTypes.HeartDisease, ColumnTypes.HeartDisease);

            PrintList(discretizedData);

            Column<float>[] normalizedData = new Column<float>
                [GetNormalizeLength(ColumnTypes.HeartDisease, EnumDataTypes.HeartDisease)];
            Normalize(ref normalizedData, discretizedData, EnumDataTypes.HeartDisease, ColumnTypes.HeartDisease);

            PrintList(normalizedData);

            CSVwrite(FILE2, normalizedData);
        }

        public static void PrintList<T>(Column<T>[] toPrint)
        {
            for (int j = 0; j < toPrint[0].GetNum(); j++)
            {
                for (int i = 0; i < toPrint.Length; i++)
                {
                    Console.Write("{0:N2}\t", toPrint[i].Get(j));
                }
                Console.WriteLine("\n==========================");
            }
        }

        public static void RemoveFromRecords<T>(ref Column<T>[] toRemove, int from, int to)
        {
            for(int i = 0; i < toRemove.Length; i++)
            {
                toRemove[i].RemoveRange(from, to);
            }
        }

        public static void PrintEnum<T>()
        {
            Array a = Enum.GetValues(typeof(T));
            foreach (T o in a)
                Console.WriteLine("{0}\t\t{1}", o, o.GetHashCode());
        }

        public static int GetNormalizeLength(int[] tabTypes, EnumDataTypes en)
        {
            int len = 0;
            switch(en)
            {
                case EnumDataTypes.HeartDisease:
                    foreach (int i in tabTypes)
                    {
                        switch (i)
                        {
                            case (int)EnumHeartDisease.Value:
                                len++;
                                break;
                            case (int)EnumHeartDisease.LowMediumHigh:
                                len += (Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1);
                                break;
                            case (int)EnumHeartDisease.AbsentPresent:
                                len += (Enum.GetValues(typeof(EnumAbsentPresent)).Length - 1);
                                break;
                            case (int)EnumHeartDisease.Obesity:
                                len += (Enum.GetValues(typeof(EnumObesity)).Length - 1);
                                break;
                            case (int)EnumHeartDisease.AgeRange:
                                len += (Enum.GetValues(typeof(EnumAgeRange)).Length -1);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }

            return len;
        }

        public static void Print(float toPrint)
        {
            Print(toPrint.ToString());
        }

        public static void Print(string toPrint)
        {
            Console.WriteLine(">>>> {0}", toPrint);
        }

        public static void Print(float[] toPrint)
        {
            foreach(float f in toPrint)
                Console.WriteLine(">>>> {0}", f);
        }


    }
}
