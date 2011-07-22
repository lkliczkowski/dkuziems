using System;
using System.IO;

namespace ZScore
{
    partial class ZScoreData
    {
        private static bool CSVread(string path, ref Column<string>[] Data)
        {
            const char DELIMETER_IN_DATA = (char)9; //9 == tab
            int count = 0;
            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;
                    
                    while ((line = readFile.ReadLine()) != null)
                    {
                        count++;
                        row = SplitBy(line, DELIMETER_IN_DATA);
                        row = SReplace(row);
                        for (int i = 0; i < row.Length; i++)
                        {
                            if (checkTheCompleteness(row))
                                Data[i].AddData(row[i]);
                            //else Console.WriteLine("not-completed data");
                        }
                    }
                    readFile.Close();

                    Print(String.Format("RawSetSize/DataSetSize\t{0}/{1}\t({2:N2}% pominięto)", count, Data[0].GetNum(), (double)(count - Data[0].GetNum()) / count));
                    
                    return true;
                }
            }
            catch (Exception e)
            {
                Print("Nieudany odczyt z pliku: ", e.Message);
            }

            return false;
        }

        public static string[] SplitBy(string toParse, char delimiter)
        {
            char[] delimiterOption = new char[] { delimiter };
            return toParse.Split(delimiterOption, StringSplitOptions.None);
        }

        public static string[] SReplace(string[] toClear)
        {
            for (int i = 0; i < toClear.Length; i++)
            {
                toClear[i] = toClear[i].Replace("\"", "");
                toClear[i] = toClear[i].Replace(" ", "");
                if (toClear[i].Contains("0Balance"))
                    toClear[i] = toClear[i].Replace("0Balance", "Balance");
            }
            return toClear;
        }

        protected static bool checkTheCompleteness(string[] toCheck)
        {
            foreach (string s in toCheck)
            {
                if (s == "")
                    return false;
            }
            return true;
        }
    }
}
