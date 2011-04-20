using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZScore
{
    partial class ZScore
    {
        private static bool CSVread(string path, ref Column<string>[] Data)
        {
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
                            if (checkTheCompleteness(row))
                                Data[i].AddData(row[i]);
                        }
                    }
                    readFile.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Print("Failed Reading file", e.Message);
            }

            return false;
        }

        private static bool CSVwrite(string path, Column<float>[] toWrite)
        {
            try
            {
                TextWriter writeFile = new StreamWriter(path);

                for (int j = 0; j < toWrite[1].GetNum(); j++)
                {
                    for (int i = 0; i < toWrite.Length; i++)
                    {

                        writeFile.Write("{0:N2};", toWrite[i].Get(j));
                        writeFile.Flush();
                    }
                    writeFile.WriteLine();
                }
                writeFile.Close();
                return true;
            }
            catch (Exception e)
            {
                Print("Failed Writing to file", e.Message);
            }

            return false;
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
                toClear[i] = toClear[i].Replace("\"", "");
                toClear[i] = toClear[i].Replace(" ", "");
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
