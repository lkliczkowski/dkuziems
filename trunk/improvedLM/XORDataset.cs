using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImprovedLM
{
    class XORDataset
    {
        private double[][] data = new double[4][];

        public XORDataset()
        {
            initXORDataset();
        }

        private void initXORDataset()
        {
            double[] sample;

            sample = new double[3] {-1,-1,1};
            data[0] = sample;

            sample = new double[3] { -1, 1, -1 };
            data[1] = sample;

            sample = new double[3] { 1, -1, -1 };
            data[2] = sample;

            sample = new double[3] { 1, 1, 1 };
            data[3] = sample;

            Console.WriteLine("Zakończono tworzenie zbioru XOR!");
        }

        public double[] sample(int f)
        {
            double[] record = new double[data[0].Length - 1];

            for (int i = 0; i < record.Length; i++)
                record[i] = data[f][i];

                return record;
        }

        public double target(int f)
        {
            return data[f][data[f].Length - 1];
        }
    }
}
