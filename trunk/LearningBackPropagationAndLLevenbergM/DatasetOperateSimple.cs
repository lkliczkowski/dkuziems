using System;
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
        /*
         * Konstruktor glowny
         */
        public DatasetOperateSimple(int setLength, int holdout, int sz)
            : base(setLength, holdout, sz, false)
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

    }
}
