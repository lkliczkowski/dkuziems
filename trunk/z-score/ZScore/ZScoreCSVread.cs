using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZScore
{
    class ZScoreCSVread
    {
        public static Records<string>[] parseCSV(string path, int record_num)
        {

            Records<string>[] Data = new Records<string>[record_num];
            for (int i = 0; i < record_num; i++)
                Data[i] = new Records<string>();

            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = SplitBy(line, (int)';');
                        row = GetRidOf(row);
                        for (int i = 0; i < row.Length; i++)
                        {
                            Data[i].AddRecord(row[i]);
                        }
                        //parsedData.Add(row);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(">>>> StreamReaderError::Wrong file path? \n\n{0}\n", e);
            }
            return Data;
        }

        public static string[] SplitBy(string toParse, int delimiter)
        {
            char[] delimiterOption = new char[] { (char)delimiter };
            return toParse.Split(delimiterOption, StringSplitOptions.None);
        }

        public static string[] GetRidOf(string[] toClear)
        {
            for (int i = 0; i < toClear.Length; i++)
            {
                if (toClear[i].Contains("\"") == true)
                    toClear[i] = toClear[i].Replace("\"", "");
                if (toClear[i].Contains(" ") == true)
                    toClear[i] = toClear[i].Replace(" ", "");
            }
            return toClear;
        }
    }
}
