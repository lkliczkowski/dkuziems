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
    class DatasetOperate : DatasetStructure
    {

        /// <summary>
        /// Ustalana w konstruktorze stala wyznaczajaca wielkosc przyrostu zbioru uczacego
        /// </summary>
        private readonly int step;
        
        /// <summary>
        /// Stala informujaca nas o ile % zwieksza sie w kolejnej epoce zbior uczacy
        /// </summary>
        private const float STEP_SIZE = 20f;

        /// <summary>
        /// Zmienna wskazujaca obecny zakres zbioru uczacego dostepnego dla metody nauczania sieci
        /// </summary>
        private int actualRange;

        //niedostępny
        private DatasetOperate()
        { }

        /*
         * Konstruktor z domyslnym podzialem zbiorow
         */ 
        public DatasetOperate(int setLength)
            :this(setLength, 60, 20, 20)
        { }

        /*
         * Konstruktor glowny
         */ 
        public DatasetOperate(int setLength, int tPercent, int gPercent, int vPercent)
            : base (setLength, tPercent, gPercent, vPercent)
        {
            actualRange = 0;
            step = ((setLength * STEP_SIZE / 100) < 1) ? 1 : (int)((float)setLength * STEP_SIZE / 100);
            IncreaseRange();
        }

        public int[] TrainingSet
        {
            get { return trainingSet.Take(actualRange).ToArray(); }
        }

        public int[] GeneralizationSet
        {
            get { return generalizationSet.ToArray(); }
        }

        public int[] ValidationSet
        {
            get { return validationSet.ToArray(); }
        }

        /// <summary>
        /// Zwieksza zakres zbioru, nie dopuszcza przekroczenia wartosci gornej czy ogolnej wielkosci zbioru
        /// </summary>
        public void IncreaseRange()
        {
            if (actualRange + step >= trainingSet.Count)
                actualRange = trainingSet.Count;
            else
                actualRange += step;
        }


        public void PrintSetIndexes()
        {
            foreach (int i in trainingSet)
                Console.Write("{0}\t", i);
            Console.WriteLine("\n");
            foreach (int i in generalizationSet)
                Console.Write("{0}\t", i);
            Console.WriteLine("\n");
            foreach (int i in validationSet)
                Console.Write("{0}\t", i);
        }

        public void PrintStepSize(int epoch)
        {
            Console.WriteLine("StepSize: 0:{0} [e: {1}]", actualRange, epoch);
        }


    }
}
