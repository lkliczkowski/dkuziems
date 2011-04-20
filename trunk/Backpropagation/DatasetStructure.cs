using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backpropagation
{
    /// <summary>
    /// Klasa przygotowujaca (losowo wybierane) indeksy ze zbioru danych
    /// </summary>
    abstract class DatasetStructure
    {
        protected List<int> trainingSet;
        protected List<int> generalizationSet;
        protected List<int> validationSet;

        protected DatasetStructure() { }

        protected DatasetStructure(int setLength, int tPercent, int gPercent, int vPercent)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < setLength; i++)
                indexes.Add(i);

            Print("Creating subsets...");
            trainingSet = drawIndexes(tPercent * setLength / 100, ref indexes);
            Print("\b\b\b\tTraining set - finished!");
            generalizationSet = drawIndexes(gPercent * setLength / 100, ref indexes);
            Print("\b\b\b\tGeneralization set - finished!");
            validationSet = indexes;
            Print(String.Format("\b\b\b\tValidation set created of remaining (size: {0}%)", vPercent));
        }

        /// <summary>
        /// Metoda losujaca dostepne indeksy ze zbioru
        /// </summary>
        /// <param name="howMany">Jak wiele indeksow ma wziac</param>
        /// <param name="fromSet"></param>
        /// <returns></returns>
        protected static List<int> drawIndexes(int howMany, ref List<int> fromSet)
        {
            List<int> newIndexSet = new List<int>();
            Random r = new Random();
            int tmp = 0;

            while (howMany != 0)
            {
                //if (fromSet.Count == 0)
                //    return newIndexSet;

                tmp = r.Next(fromSet.Last());
                if (fromSet.Contains(tmp))
                {
                    newIndexSet.Add(tmp);
                    fromSet.Remove(tmp);
                    howMany--;
                }
            }

            return newIndexSet;
        }

        protected static void Print(string toPrint)
        {
            Console.WriteLine(">> {0}", toPrint);
        }
    }
}
