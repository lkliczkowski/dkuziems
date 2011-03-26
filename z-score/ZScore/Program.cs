using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ZScore
{
    class Program
    {
        static void Main(string[] args)
        {
            normalizeHeartDisease();
            Console.ReadKey();
        }

        static void normalizeHeartDisease()
        {
            string FILE1 = "HeartDiseaseShort.csv";
            Records<string>[] rawData = ZScoreCSVread.parseCSV(FILE1, ZScoreRecordTypes.HeartDisease.Length);

            Console.WriteLine(">>{0}", rawData.Length);
            RemoveFromRecords(ref rawData, 0, 2);

            Records<float>[] discretizedData = ZScoreDiscretize.Discretize
                (rawData, EnumDataTypes.HeartDisease, ZScoreRecordTypes.HeartDisease);

            PrintList(discretizedData);
        }

        public static void PrintList<T>(Records<T>[] toPrint)
        {
            for (int j = 0; j < toPrint[0].GetNum(); j++)
            {
                for (int i = 0; i < toPrint.Length; i++)
                {
                    Console.Write("{0:N2}\t", toPrint[i].Get(j));
                }
                Console.WriteLine();
            }
        }

        public static void RemoveFromRecords<T>(ref Records<T>[] toRemove, int from, int to)
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
    }
}
