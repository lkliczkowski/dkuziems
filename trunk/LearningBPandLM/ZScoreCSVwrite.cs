using System;
using System.IO;

namespace ZScore
{
    partial class ZScore
    {
        private static bool CSVwrite(string path, Column<double>[] toWrite)
        {
            const char DELIMETER_IN_DATA = (char)9; //9 == tab

            try
            {
                TextWriter writeFile = new StreamWriter(path);

                for (int j = 0; j < toWrite[1].GetNum(); j++)
                {
                    for (int i = 0; i < toWrite.Length; i++)
                    {

                        writeFile.Write("{0:N2}{1}", toWrite[i].Get(j), DELIMETER_IN_DATA);
                        writeFile.Flush();
                    }
                    writeFile.WriteLine();
                }
                writeFile.Close();
                return true;
            }
            catch (Exception e)
            {
                Print("Nieudany zapis do pliku: ", e.Message);
            }

            return false;
        }
    }
}
