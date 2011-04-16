using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backpropagation
{
    class SetsOperate
    {
        protected List<int> trainingSet;
        protected List<int> generalizationSet;
        protected List<int> validationSet;

        private SetsOperate() { }

        public SetsOperate(int setLength)
            :this(setLength, 60, 20, 20)
        { }

        public SetsOperate(int setLength, int tPercent, int gPercent, int vPercent)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < setLength; i++)
                indexes.Add(i);

            trainingSet = drawIndexes(tPercent * setLength / 100, ref indexes);
            generalizationSet = drawIndexes(gPercent * setLength / 100, ref indexes);
            validationSet = indexes;

            PrintSetIndexes();
        }

        protected static List<int> drawIndexes(int howMany, ref List<int> fromSet)
        {
            List<int> newIndexSet = new List<int>();
            Random r = new Random();
            int tmp = 0;

            while (howMany != 0)
            {
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


    }
}
