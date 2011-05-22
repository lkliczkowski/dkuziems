using System;
using System.IO;
using System.Collections.Generic;

namespace MyData
{
	public static class DataRead
	{
		const char Delimeter = (char)9; //9 == tab
		
		public static List<double[]> ReadData(string path)
		{
			List<double[]> data = new List<double[]>();
			
            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;

                    while ((line = readFile.ReadLine()) != null)
                    {
						List<double> record = new List<double>();
						if(!line.Contains("#"))
						{
							foreach(string s in splitBy(line, Delimeter))
							{
								
									record.Add(Double.Parse(s.Replace(".",",")));
							}
							data.Add(record.ToArray());
						}
                    }
                    readFile.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Nieudany odczyt z pliku: ", e.Message);
            }
			

            
            return data;
        }
		
		/// <summary>
		/// Rozdziela string wg podanego separatora
		/// </summary>
		/// <param name="toParse"></param>
		/// <param name="delimiter">separator</param>
		/// <returns></returns>
		private static string[] splitBy(string toParse, char delimiter)
        {
            char[] delimiterOption = new char[] { delimiter };
            return toParse.Split(delimiterOption, StringSplitOptions.None);
        }

	}
}

