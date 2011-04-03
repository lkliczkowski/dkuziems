using System;
using System.Collections.Generic;
using System.IO;

namespace PrimalPerceptronAlgorithm
{


	public static class SaveToGnuplot
	{

		static SaveToGnuplot ()
		{
			Print("Using SaveToGnuplot class (supposed to work with 2 weights)");
		}
		
		public static bool Save(string path, string graphName, List<double[]> weights, List<double> bias, int multiply, double learningRate)
        {
            try
            {
				int m = (int)(bias.Count / multiply);
				TextWriter writeFile = new StreamWriter(path);

				writeFile.WriteLine("set terminal png");
				writeFile.WriteLine("set output \"{0}.png\"", graphName);
				writeFile.WriteLine("set title \"Learning rate = {0}\"", learningRate);
				writeFile.WriteLine("set xrange [-3:0]");
				writeFile.WriteLine("set yrange [-7:-5]");
				writeFile.Write("plot ");
				writeFile.Flush();
				for (int j = 0; j < bias.Count; j++)
				{
					if((j%m) == 1)
					{
						string output = String.Format("-(" + weights[j][0] + "* x + " + bias[j] + ")/" 
						                              + weights[j][1] + " title \"iteracja t = " + j + "\"");

						output = ChangeFormat(output);
						writeFile.Write(output);
						Print("Saving results from iteration:", j.ToString());
						
						if((j+m) >= bias.Count)
							writeFile.Write(";");
						else 
							writeFile.Write(",");
						
						writeFile.Flush();
					}
				}
                writeFile.Close();
                return true;
				
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed Writing to file: \n{0}", e.Message);
            }
			
            return false;
        }
		
		/*
		 * Pozwala na zapisanie teoretycznie lepszych wykresow
		 */ 
		public static bool SaveWithAdd(string path, string graphName, List<double[]> weights, List<double> bias, int multiply, double learningRate)
        {
            try
            {
				int m = (int)(bias.Count / multiply), addition = 2;
				TextWriter writeFile = new StreamWriter(path);

				writeFile.WriteLine("set terminal png");
				writeFile.WriteLine("set output \"{0}.png\"", graphName);
				writeFile.WriteLine("set title \"Learning rate = {0}\"", learningRate);
				writeFile.WriteLine("set xrange [-3:0]");
				writeFile.WriteLine("set yrange [-7:-5]");
				writeFile.Write("plot ");
				writeFile.Flush();
				for (int j = 0; j < bias.Count; j++)
				{
					if((j%m) == 1)
					{
						int t = new int();
						string output = "";
						if(j == 1)
						{
							output = String.Format("-(" + weights[j][0] + "* x + " + bias[j] + ")/" 
							                       + weights[j][1] + " title \"iteracja t = " + j + "\"");
							Print("Saving results from iteration:", j.ToString());
						}
						else
							try
							{
								t = j + addition;
								output = String.Format("-(" + weights[t][0] + "* x + " + bias[t] + ")/" 
								                       + weights[t][1] + " title \"iteracja t = " + t + "\"");
								Print("Saving results from iteration:", t.ToString());
							}
							catch (Exception ep)
							{
								Console.WriteLine("Out of Range in SaveWithAdd, Gnuplot set normal", ep.Message);
								output = String.Format("-(" + weights[j][0] + "* x + " + bias[j] + ")/" 
								                       + weights[j][1] + " title \"iteracja t = " + j + "\"");
								Print("Saving results from iteration:", j.ToString());
							}
						
						output = ChangeFormat(output);
						writeFile.Write(output);
						
						
						if((j+m) > bias.Count)
							writeFile.Write(";");
						else 
							writeFile.Write(",");
						
						writeFile.Flush();
					}
				}
                writeFile.Close();
                return true;
				
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed Writing to file: \n{0}", e.Message);
            }
			
            return false;
        }
		
		private static string ChangeFormat(string toChange)
		{
            return toChange.Replace(",", ".");
        }		
		
		public static void Print(string what)
        {
            Print(what, null);
        }

        public static void Print(string what, string about)
        {
            Console.WriteLine(">> {0} {1} {2}", what,
                              (about == null || about == "") ?
                              "" : "::", about);
        }
	}
}
