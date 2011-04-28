using System;
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
    class DatasetOperateGrowingNoRandom : DatasetStructure
    {
        //niedostępny
        private DatasetOperateGrowingNoRandom()
        { }

        /*
         * Konstruktor z domyslnym podzialem zbiorow
         */ 
        public DatasetOperateGrowingNoRandom(int setLength)
            :this(setLength, 20)
        { }

        /*
         * Konstruktor glowny
         */
        public DatasetOperateGrowingNoRandom(int setLength, int gPercent)
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

    }
}
