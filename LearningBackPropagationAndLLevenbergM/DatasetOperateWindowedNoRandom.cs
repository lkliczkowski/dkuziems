using System;
using System.Collections.Generic;
using System.Linq;

namespace LearningBPandLM
{
    /// <summary>
    /// siec bedzie sie uczyc w kolejnych epokach na kolejnych wycinkach zbioru uczącego
    /// </summary>
    class DatasetOperateWindowedNoRandom : DatasetStructure
    {
        /*
         * Konstruktor glowny
         */
        public DatasetOperateWindowedNoRandom(int setLength, int holdout, int sz)
            : base(setLength, holdout, sz, true)
        {
            actualRange = 0;
            
            IncreaseRange();
        }

        public override int[] TrainingSet
        {
            get { return trainingSet.Skip(actualRange).Take(step).ToArray(); }
        }

        /// <summary>
        /// Zwieksza zakres zbioru, nie dopuszcza przekroczenia wartosci gornej czy ogolnej wielkosci zbioru
        /// zeruje zakres gdy "okienko" dochodzi do konca zbioru
        /// </summary>
        public override void IncreaseRange()
        {
            if (actualRange + step >= trainingSet.Count - 1)
            {
                MixAgainTrainingData();
                actualRange = 0;
            }
            else
                actualRange += step;
        }
    }
}
