using System;
using System.Collections.Generic;
using System.IO;

namespace Backpropagation
{
    public static class ReadData
    {
        static ReadData()
        {
            Print("Using ReadData class...");
        }

        public static void ReadFloatList(string FILE, ref List<float[]> data)
        {
            List<string[]> rawData = readFile(FILE);

            //usuwamy pierwszy wiersz - informacje o danych beda pobierane automatycznie
            rawData.RemoveRange(0, 1);

            foreach (string[] record in rawData)
            {
                data.Add(ParseLineToFloat(record));
            }
        }

        private static float[] ParseLineToFloat(string[] toParse)
        {
            List<float> tab = new List<float>();
            double val;
            foreach (string cell in toParse)
            {
                if (Double.TryParse(cell, out val))
                {
                    tab.Add((float)val);
                }
                else
                {
                    if (!cell.Equals(""))
                        Print("TryParse() (NaN?)", cell);
                }
            }
            return tab.ToArray();
        }

        private static List<string[]> readFile(string path)
        {
            List<string[]> rawData = new List<string[]>();

            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = ChangeFormat(SplitBy(line, (int)' '));
                        #region Drukowanie zawartosci pliku
                        //for (int i = 0; i < row.Length; i++)
                        //    Print(row[i], i.ToString());
                        #endregion
                        rawData.Add(row);
                    }
                    readFile.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(">>>> StreamReaderError::Wrong file path? \n\n{0}\n", e.Message);
            }

            return rawData;
        }

        private static string[] SplitBy(string toParse, int delimiter)
        {
            char[] delimiterOption = new char[] { (char)delimiter };
            return toParse.Split(delimiterOption, StringSplitOptions.None);
        }

        private static string[] ChangeFormat(string[] toChange)
        {
            for (int i = 0; i < toChange.Length; i++)
            {
                if (toChange[i].Contains(".") == true)
                    toChange[i] = toChange[i].Replace(".", ",");
            }
            return toChange;
        }


        public static void Print(string what)
        {
            Print(what, null);
        }

        public static void Print(string what, string about)
        {
            Console.WriteLine(">>>> {0} {1} {2}", what,
                              (about == null || about == "") ?
                              "" : "::", about);
        }

    }
}
