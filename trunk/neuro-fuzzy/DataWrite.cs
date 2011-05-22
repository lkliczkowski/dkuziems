using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace MyData
{
	public static class DataWrite
	{
		const char Delimeter = (char)9; //9 == tab
		
		public static bool WriteData(string path, List<double[]> toWrite, string header)
		{			
            try
            {
                TextWriter write = new StreamWriter(path);
				write.WriteLine("# {0}", header);
				
				foreach(double[] dd in toWrite)
				{
					foreach(double d in dd)
					{
						write.Write(String.Format("{0:N3}{1}", d, Delimeter).Replace(",","."));
					}
					write.WriteLine();
					write.Flush();
				}
				
				write.Close();
				return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Nieudany zapis do pliku \"{0}\": {1}", path, e.Message);
				return false;
            }
			
        }
		
		public static bool WriteData(string path, List<double[]> toWrite)
		{	
			if(WriteData(path, toWrite, null))
				return true;
			return false;
        }

        public static bool WriteData(string path, Hashtable toWrite, string header)
        {
            try
            {
                TextWriter write = new StreamWriter(path);
                write.WriteLine("# {0}", header);

                foreach (DictionaryEntry d in toWrite)
                {
                    write.WriteLine("{0}\t{1}", d.Key, d.Value);
                    write.Flush();
                }

                write.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Nieudany zapis do pliku: ", e.Message);
                return false;
            }

        }
	}
}

