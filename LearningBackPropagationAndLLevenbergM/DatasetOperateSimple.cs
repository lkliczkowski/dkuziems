﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LearningBPandLM
{
    /// <summary>
    /// prosty losowy podzial danych na zbiór treningowy i walidacyjny,
    /// w każdej epoce algorytm nauczania będzie miał dostęp do całego zbioru uczącego
    /// </summary>
    class DatasetOperateSimple : DatasetStructure
    {
        //niedostępny
        private DatasetOperateSimple()
        { }

        /*
         * Konstruktor glowny
         */
        public DatasetOperateSimple(int setLength, int sz)
            : base(setLength, DefaultGeneralizationSetSize, sz)
        {
            actualRange = 0;
        }

        public override int[] TrainingSet
        {
            get { return trainingSet.ToArray(); }
        }

        /// <summary>
        /// Zwieksza zakres zbioru, nie dopuszcza przekroczenia wartosci gornej czy ogolnej wielkosci zbioru
        /// zeruje zakres gdy "okienko" dochodzi do konca zbioru
        /// </summary>
        public override void IncreaseRange()
        {
            //nic nie rób - pełny zbiór treningowy dostępny
        }

        /// <summary>
        /// Metoda losujaca dostepne indeksy ze zbioru
        /// </summary>
        /// <param name="howMany">Jak wiele indeksow ma wziac</param>
        /// <param name="fromSet"></param>
        /// <returns></returns>
        protected override List<int> drawIndexes(int howMany, ref List<int> fromSet)
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

    }
}
