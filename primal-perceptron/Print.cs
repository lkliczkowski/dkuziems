using System;
using System.Collections.Generic;

namespace PrimalPerceptronAlgorithm
{


    partial class Program
	{
	    #region funkcje wypisujace (dla ulatwienia)
        //Nie lubie ciagle wpisywac 
        //Console.WriteLine(">>> {0} :: {1}", what, about);
        public static void Print(string what)
        {
            Print(what, null);
        }
		
		public static void Println()
		{
			Console.WriteLine();
		}

        public static void Print(string what, string about)
        {
            Console.WriteLine(">> {0} {1} {2}", what,
                              about == null ? "" : "::", about);
        }
		
		public static void Print(string what, double val)
        {
            Console.WriteLine("{0}:\t{1:N1}", what, val);
        }

        public static void PrintList(List<double[]> toPrint)
        {
			int i = new int();
            foreach (double[] line in toPrint)
            {
				Console.Write("{0}:\t", i++);
                foreach (double cell in line)
                {
                    Console.Write("{0:N2} \t", cell);
                }
                Console.WriteLine();
            }
        }

        public static void PrintList(List<double> toPrint)
        {
			int i = new int();
            foreach (double cell in toPrint)
            {
				Console.Write("{0}:\t", i++);
                Console.WriteLine("{0:N2} \t", cell);
            }
            Console.WriteLine();
        }
		
		public static void PrintArray<T>(T[] array, string etykieta)
		{
			if(etykieta != null) Console.Write("{0}:\t", etykieta);
			foreach(T a in array)
			{
				Console.Write("{0:N1}\t", a);
			}
			Console.WriteLine();
		}
		
		public static void PrintArray<T>(T[] array)
		{
			PrintArray<T>(array, null);
		}
        #endregion
	}
}
