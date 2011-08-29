using System;
using System.Collections.Generic;
using System.Linq;

namespace LearningBPandLM
{
    /// <summary>
    /// typy zbiorow danych obslugiwanych przez algorytm
    /// </summary>
    public enum EnumDatasetStructures
    {
        Growing,
        Windowed,
        Simple
    }

    /// <summary>
    /// Klasa przygotowujaca (losowo wybierane) indeksy ze zbioru danych
    /// </summary>
    abstract class DatasetStructure
    {
        /// <summary>
        /// zbior uczacy (trenujacy) uzywany takze do aktualizowania wag
        /// </summary>
        protected List<int> trainingSet;

        /// <summary>
        /// zbior sprawdzajacy poprawnosc sieci pod koniec kazdej epoki
        /// </summary>
        protected List<int> generalizationSet;

        public const int DefaultGeneralizationSetSize = 20;

        /// <summary>
        /// Ustalana w konstruktorze stala wyznaczajaca wielkosc przyrostu zbioru uczacego
        /// </summary>
        protected readonly int step;

        /// <summary>
        /// Stala informujaca nas o ile % zwieksza sie w kolejnej epoce zbior uczacy
        /// </summary>
        protected readonly int StepSampleSize; //1 = 1%

        /// <summary>
        /// Zmienna wskazujaca obecny zakres zbioru uczacego dostepnego dla metody nauczania sieci
        /// </summary>
        protected int actualRange;

        private DatasetStructure() { }

        //protected DatasetStructure(int setLength, int gPercent)
        //    :this (setLength, gPercent, 1, true)
        //{ }
        protected DatasetStructure(int setLength, int gPercent, double sz, bool showSampleSize)
        {
            StepSampleSize = ((int)sz * (setLength - gPercent * setLength / 100) / 100 == 0) && (setLength > 0) ?
                1 : (int)sz * (setLength - gPercent * setLength / 100) / 100;

            List<int> indexes = new List<int>();
            for (int i = 0; i < setLength; i++)
                indexes.Add(i);

            Program.PrintInfo("Tworzenie podzbiorow");

            step = StepSampleSize;

            trainingSet = drawIndexes(((100 - gPercent) * setLength) / 100, ref indexes);
            Print(String.Format("\b\b\b\tZbiór uczący - zakończono!\t{0} przypadków\t({1:N2}%)",
                trainingSet.Count, (100 - gPercent)));

            generalizationSet = indexes;
            Print(String.Format("\b\b\b\tZbiór walidacyjny - zakończono!\t{0} przypadków\t({1:N2}%)",
                generalizationSet.Count, gPercent));

            if(showSampleSize)
            Print(String.Format("\b\b\b\tWielkosc pojedynczej próbki uczącej:\t{0} ({1}%)",
                StepSampleSize, StepSampleSize * 100 / trainingSet.Count));

        }

        public virtual void MixAgainTrainingData()
        {
            List<int> newIndexSet = new List<int>();
            Random r = new Random();
            int tmp = 0;

            while (trainingSet.Count != 0)
            {
                tmp = r.Next(trainingSet.Count);

                newIndexSet.Add(trainingSet[tmp]);
                trainingSet.RemoveAt(tmp);

            }

            trainingSet = newIndexSet;
        }

        protected virtual List<int> drawIndexes(int howMany, ref List<int> fromSet)
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

        public virtual int[] TrainingSet
        {
            get
            {
                return trainingSet.GetRange(actualRange, step).ToArray(); 
            }
        }

        public int[] GeneralizationSet
        {
            get { return generalizationSet.ToArray(); }
        }

        /// <summary>
        /// Zwieksza zakres zbioru, nie dopuszcza przekroczenia wartosci gornej czy ogolnej wielkosci zbioru
        /// </summary>
        public virtual void IncreaseRange()
        {
            if (actualRange + step >= trainingSet.Count)
                {
                    MixAgainTrainingData();
                    actualRange = 0;
                }
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
        }

        protected static void Print(string toPrint)
        {
            Console.WriteLine(">> {0}", toPrint);
        }
    }
}
