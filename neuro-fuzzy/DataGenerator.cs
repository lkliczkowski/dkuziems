using System;
using System.IO;

namespace MyData
{
	public static class DataGenerator
	{
		//znak rozdzielający dane w pliku
		const char Delimeter = (char)9; //9 == tab
		
		/// <summary>
		///	sin(x)+ε [-0.05, 0.05]
		/// </summary>
		/// <param name="path">ścieżka do pliku</param>
		public static bool Function1a(string path, int numberOfPoints)
		{
			//losowa wartość
			Random r = new Random();
			
			//wartość x i szum
			double x, e;
			
			double domainFrom = 0, domainTo = 2*Math.PI;
			double noiseDomainFrom = -0.05, noiseDomainTo = 0.05;

			try
            {
                TextWriter writeFile = new StreamWriter(path);
				writeFile.WriteLine("#{0}{1}{2}", "x", Delimeter, "f(x)");
				
                for (int j = 0; j < numberOfPoints; j++)
                {
					x = r.NextDouble() * Math.Abs(domainFrom - domainTo) + domainFrom;
					e = r.NextDouble() * Math.Abs(noiseDomainFrom - noiseDomainTo) + noiseDomainFrom;
					
					writeFile.WriteLine(String.Format("{0}{1}{2}", x, Delimeter, Math.Sin(x) + e).Replace(",","."));
					writeFile.Flush();
                }
                writeFile.Close();
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine("Nieudany zapis do pliku: ", err.Message);
				return false;
            }

		}
		
		/// <summary>
		///	sin(x)+ε [-0.1,0.1]
		/// </summary>
		/// <param name="path">ścieżka do pliku</param>
		public static bool Function1b(string path, int numberOfPoints)
		{
			//losowa wartość
			Random r = new Random();
			
			//wartość x i szum
			double x, e;
			
			double domainFrom = 0, domainTo = 2*Math.PI;
			double noiseDomainFrom = -0.1, noiseDomainTo = 0.1;

			try
            {
                TextWriter writeFile = new StreamWriter(path);
				writeFile.WriteLine("#{0}{1}{2}", "x", Delimeter, "f(x)");
				
                for (int j = 0; j < numberOfPoints; j++)
                {
					x = r.NextDouble() * Math.Abs(domainFrom - domainTo) + domainFrom;
					e = r.NextDouble() * Math.Abs(noiseDomainFrom - noiseDomainTo) + noiseDomainFrom;
					
					writeFile.WriteLine(String.Format("{0}{1}{2}", x, Delimeter, Math.Sin(x) + e).Replace(",","."));
					writeFile.Flush();
                }
                writeFile.Close();
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine("Nieudany zapis do pliku: ", err.Message);
				return false;
            }

		}
		
		public static bool Function2(string path, int numberOfPoints)
		{
			//losowa wartość
			Random r = new Random();
			
			//wartość x
			double x;
			
			double domainFrom = 0, domainTo = 1;

			try
            {
                TextWriter writeFile = new StreamWriter(path);
				writeFile.WriteLine("#{0}{1}{2}", "x", Delimeter, "f(x)");
				
                for (int j = 0; j < numberOfPoints; j++)
                {
					x = r.NextDouble() * Math.Abs(domainFrom - domainTo) + domainFrom;

					//[%e^(-2*log(2)*((x-0.08)/0.854)^2)*((sin(5*%pi*(3^(3/4*x)-0.05))^6))
					writeFile.WriteLine(String.Format("{0:N4}{1}{2:N4}", x, Delimeter, Math.Exp
					                                  (-2*Math.Log10(2)*Math.Pow((x-0.08)/0.854, 2)) 
					                                  * Math.Pow((Math.Sin(5*Math.PI*(Math.Pow(x, 0.75) - 0.05))), 6))
					                    .Replace(",","."));
					writeFile.Flush();
                }
                writeFile.Close();
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine("Nieudany zapis do pliku: ", err.Message);
				return false;
            }

		}
		
		public static bool Function3(string path, int numberOfPoints)
		{
			//losowa wartość
			Random r = new Random();
			
			//wartość x
			double x, y;
			
			double domainFrom = -5, domainTo = 5;

			try
            {
                TextWriter writeFile = new StreamWriter(path);
				writeFile.WriteLine("#{0}{1}{2}{3}{4}", "x", Delimeter, "y", Delimeter, "f(x,y)");
				                    
                for (int j = 0; j < numberOfPoints; j++)
                {
					x = r.NextDouble() * Math.Abs(domainFrom - domainTo) + domainFrom;
					y = r.NextDouble() * Math.Abs(domainFrom - domainTo) + domainFrom;
					
					writeFile.WriteLine(String.Format("{0:N4}{1}{2:N4}{3}{4:N4}", x, Delimeter, y, Delimeter, 
					                                  200 - Math.Pow(x*x + y - 11, 2)-(x+ y*y -7)).Replace(",","."));
					writeFile.Flush();
                }
                writeFile.Close();
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine("Nieudany zapis do pliku: ", err.Message);
				return false;
            }

		}
		
		
		
		
	}
}

