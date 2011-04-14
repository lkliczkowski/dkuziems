using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backpropagation
{
    /// <summary>
    /// Feed Forward Neural Network structure
    /// </summary>
    abstract class FFNNStructure
    {
        //protected const float BIAS = 1f;
        protected float BIAS = 1f;

        protected float learningRate;

        //liczba neuronow w kazdej warstwie
        protected int numInput, numHidden, numOutput;

        //neurony w kazdej warstwie
        protected float[] inputNeurons,
            hiddenNeurons, outputNeurons;

        //wagi pomiedzy warstwami
        protected float[][] weightsInputHidden,
            weightsHiddenOutput;

        //licznik epok i limit
        public int Epoch { get; set; } 
        public int MaxEpoch {get; set;}

        //docelowa dokladnosc
        public float DesiredAccuracy {get; set;}

        //wyniki z obl. delta-reguly
        protected float[] deltaHidden, deltaOutput;

        //pomocnicze fnkcje
        public void Stats()
        {
            Print(String.Format("Epoch " + Epoch));
            Print("Input-Hidden weights");
            foreach (float[] f in weightsInputHidden)
                Print(f);

            Print();

            Print("Hidden-Output weights");
            foreach (float[] f in weightsHiddenOutput)
                Print(f);

            Print();
            Print("Input neurons");
            Print(inputNeurons);

            Print();
            Print("Hidden neurons");
            Print(hiddenNeurons);

            Print();
            Print("Output neurons");
            Print(outputNeurons);
        }

        protected static void Print(float[] toPrint)
        {
            foreach (float f in toPrint)
                Console.Write("{0:N2}\t", f);
            Console.WriteLine();
        }

        protected static void Print(string toPrint, string par)
        {
            Console.WriteLine(">> {0} :: {1}", toPrint, par);
        }

        protected static void Print(string toPrint)
        {
            Console.WriteLine(">> {0}", toPrint);
        }

        protected static void Print()
        {
            Console.WriteLine();
        }

    }
}
