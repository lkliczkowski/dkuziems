using System;
using System.Collections.Generic;
using System.Linq;

namespace LearningBPandLM
{
    /// <summary>
    /// prosty losowy podzial danych na zbiór treningowy i walidacyjny,
    /// w każdej epoce algorytm nauczania będzie miał dostęp do całego zbioru uczącego
    /// </summary>
    class DatasetOperateSimpleDouble : DatasetStructure
    {
        bool firstPart = true;
        /*
         * Konstruktor glowny
         */
        public DatasetOperateSimpleDouble(int setLength, int holdout, int sz)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < setLength; i++)
                indexes.Add(i);

            Program.PrintInfo("Tworzenie podzbiorow");

            trainingSet = drawIndexes(((100 - holdout) * setLength) / 100, ref indexes);

            doubleSet = drawIndexes( trainingSet.Count / 3, ref trainingSet);

            Print(String.Format("\b\b\b\tZbiór uczący - zakończono!\t{0} przypadków\t({1:N2}%)",
                trainingSet.Count, 2 * (100 - holdout) / 3));

            Print(String.Format("\b\b\b\tZbiór dublujący - zakończono!\t{0} przypadków\t({1:N2}%)",
                doubleSet.Count, (100 - holdout) / 3 ));

            generalizationSet = indexes;

            Print(String.Format("\b\b\b\tZbiór walidacyjny - zakończono!\t{0} przypadków\t({1:N2}%)",
                generalizationSet.Count, holdout));
        }

        public override int[] TrainingSet
        {
            get 
            {
                if (firstPart)
                {
                    return trainingSet.Take(trainingSet.Count / 2).ToArray();
                }
                else
                {
                    return trainingSet.Skip(trainingSet.Count / 2).ToArray();
                }

            }
        }

        public override int[] DoubleSet
        {
            get { return doubleSet.ToArray(); }
        }

        /// <summary>
        /// Zwieksza zakres zbioru, nie dopuszcza przekroczenia wartosci gornej czy ogolnej wielkosci zbioru
        /// zeruje zakres gdy "okienko" dochodzi do konca zbioru
        /// </summary>
        public override void IncreaseRange()
        {
            if (firstPart)
            {
                firstPart = false;
            }
            else
            {
                firstPart = true;
                MixAgainTrainingData();
            }
        }

        protected List<int> drawIndexesFromLast(int howMany, ref List<int> fromSet)
        {
            List<int> newIndexSet = new List<int>();
            int tmp = 0;

            while (howMany != 0)
            {
                tmp = fromSet.First();
                if (fromSet.Contains(tmp))
                {
                    newIndexSet.Add(tmp);
                    fromSet.Remove(tmp);
                    howMany--;
                }
            }

            return newIndexSet;
        }
    }
}
