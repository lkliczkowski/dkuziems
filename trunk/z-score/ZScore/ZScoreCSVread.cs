using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZScore
{
    partial class ZScore
    {

        public static bool CSVwrite(string path, Column<float>[] toWrite)
        {
            bool done = false;
            try
            {
                TextWriter writeFile = new StreamWriter(path);

                for (int j = 0; j < toWrite[1].GetNum(); j++)
                {
                    for (int i = 0; i < toWrite.Length; i++)
                    {

                        writeFile.Write("{0:N2};", toWrite[i].Get(j));
                        writeFile.Flush();
                        Console.Write("{0}+{1}\t", i,j);
                    }
                    writeFile.WriteLine();
                    //Console.WriteLine("\n==========================");
                }
                writeFile.Close();
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed Writing to file: \n{0}", e);
            }

            return done;
        }

        public static Column<string>[] CSVread(string path, int column_num)
        {

            Column<string>[] Data = new Column<string>[column_num];
            for (int i = 0; i < column_num; i++)
                Data[i] = new Column<string>();

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
                            Data[i].AddData(row[i]);
                        }
                    }
                    readFile.Close();
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
