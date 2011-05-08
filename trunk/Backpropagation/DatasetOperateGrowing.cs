﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backpropagation
{
    /// <summary>
    /// siec bedzie sie uczyc w kolejnych epokach na powiekszajacym sie zbiorze uczacym,
    /// alternatywna metoda do "Growing Dataset" to "Windowed Data set" gdzie sieci
    /// pokazywane sa kolejne wycinki zbioru uczacego
    /// </summary>
    class DatasetOperateGrowing : DatasetStructure
    {
        //niedostępny
        private DatasetOperateGrowing()
        { }

        /*
         * Konstruktor z domyslnym podzialem zbiorow
         */ 
        public DatasetOperateGrowing(int setLength)
            :this(setLength, 20)
        { }

        /*
         * Konstruktor glowny
         */
        public DatasetOperateGrowing(int setLength, int gPercent)
            : base (setLength, gPercent)
        {
            actualRange = 0;
            
            IncreaseRange();
        }

        public override int[] TrainingSet
        {
            get { return trainingSet.Take(actualRange).ToArray(); }
        }

        /// <summary>
        /// Zwieksza zakres zbioru, nie dopuszcza przekroczenia wartosci gornej czy ogolnej wielkosci zbioru
        /// </summary>
        public override void IncreaseRange()
        {
            if (actualRange + step >= trainingSet.Count)
                actualRange = trainingSet.Count;
            else
                actualRange += step;
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