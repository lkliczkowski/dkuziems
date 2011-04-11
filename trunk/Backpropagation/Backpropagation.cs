using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backpropagation
{
    class Backpropagation : FFNNStructure
    {
        public Backpropagation()
            : this(0, 0, 0, 0)
        { }

        /// <summary>
        /// Konstruktor klasy propagacji wstecznej
        /// </summary>
        /// <param name="lr">wspolczynnik uczenia (learning rate)</param>
        /// <param name="nI">liczba neuronow warstwy WE</param>
        /// <param name="nH">liczba neuronow warstwy ukrytej</param>
        /// <param name="nO">liczba neuronow warstwy WY</param>
        public Backpropagation(float lr, int nI, int nH, int nO)
        {
            learningRate = lr;

            numInput = nI;
            numHidden = nH;
            numOutput = nO;

            initNeurons();
            initWeights();
            initDelta();
            initGradient();

            Epoch = new int();
            MaxEpoch = 500;

            DesiredAccuracy = 0.9f;
        }

        public bool Run()
        {
            float[] sample = { 1, 1, 1, 1 };
            calculateOfOutputsForOutputAndHiddenLayer(sample);
            calculateOfErrorsForHiddenAndOutputLayer(sample);
            calculateNewWeights();
            return false;
        }

        private void calculateNewWeights()
        {
            weightsHiddenOutput = calculateWeights(weightsHiddenOutput, learningRate, outputErrorGradient, hiddenNeurons);
            weightsInputHidden = calculateWeights(weightsInputHidden, learningRate, hiddenErrorGradient, inputNeurons);

        }

        private static float[][] calculateWeights(float[][] weightList, float l, float[] errorGradient, float[] neurons)
        {
            for (int i = 0; i < weightList.Length; i++)
            {
                for (int j = 0; j < weightList[i].Length; j++)
                {
                    weightList[i][j] += l * errorGradient[j] * neurons[i];
                }
            }

            return weightList;
        }

        /// <summary>
        /// oblicza WY dla podanych WE,
        /// wyjscie przechowywane jest w outputNeurons
        /// </summary>
        /// <param name="inputs">lista WE</param>
        private void calculateOfOutputsForOutputAndHiddenLayer(float[] inputs)
        {
            //przypisujemy wartosci na WE
            for (int i = 0; i < inputNeurons.Length - 1; i++)
                inputNeurons[i] = inputs[i];

            //obliczamy wartosci w hiddenLayer
            hiddenNeurons = calculateOfOutputs
                (inputNeurons, hiddenNeurons, weightsInputHidden, ref deltaInputHidden, false);
            
            //wyliczamy wartosci na WY
            outputNeurons = calculateOfOutputs
                (hiddenNeurons, outputNeurons, weightsHiddenOutput, ref deltaHiddenOutput, true);
        }

        //private static float[] calculateOfError(float[] inputs) { return inputs; }


        /// <summary>
        /// Wylicza wartosci dla wskazanej tablicy danych i wybranych warstw
        /// </summary>
        /// <param name="layerFrom">warstwa WE (z ktorej przychodzi sygnal</param>
        /// <param name="currLayer">warstwa, dla ktorej wyliczamy wartosci</param>
        /// <param name="weightList">wagi pomiedzy warstwami</param>
        /// <param name="deltaArr">przechowyje wyliczone sume wag [0]</param>
        /// <param name="useSigmoid">okresla ktora funkcje aktywacji uzywamy:
        /// dla hiddenLayer Tanh(x), dla outputLayer sigmoid(x)</param>
        /// <returns>obliczone wartosci dla currLayer</returns>
        private static float[] calculateOfOutputs
            (float[] layerFrom, float[] currLayer, float[][] weightList, 
            ref float[][] deltaArr, bool useSigmoid)
        {
            for (int i = 0; i < currLayer.Length; i++)
            {
                float tmp_val = new float();
                for (int j = 0; j < layerFrom.Length; j++)
                {
                    tmp_val += layerFrom[j] * weightList[j][i];
                }
                if(useSigmoid)
                    currLayer[i] = activationFunctionSigmoid(tmp_val);
                else
                    currLayer[i] = activationFunctionTanh(tmp_val);

                deltaArr[i][0] = tmp_val;
 
            }
            return currLayer;
        }


        /// <summary>
        /// sum-of-squares error,
        /// output Err_i = o_i*(1-o_i)*(y_i - o_i)
        /// hidden Err_i = o_i*(1-o_i)*Sum(Err_j*w_ij)
        /// </summary>
        /// <param name="inputs">lista wartosci na WE, 
        /// zakladamy, ze ostatni element jest WY</param>
        private void calculateOfErrorsForHiddenAndOutputLayer(float[] inputs)
        {
            for (int i = 0; i < numOutput; i++)
            {
                deltaHiddenOutput[i][1] = (inputs[inputs.Length - 1 - i] - outputErrorGradient[i]);
                outputErrorGradient[i] = outputNeurons[i] * (1 - outputNeurons[i])
                    * deltaHiddenOutput[i][1];
            }

            for (int i = 0; i < numHidden + 1; i++)
            {
                //tmp_val = Err_j*w_ij
                float tmp_val = new float();
                for (int j = 0; j < numOutput; j++)
                {
                    tmp_val = outputErrorGradient[j] * weightsHiddenOutput[i][j];
                }
                hiddenErrorGradient[i] = hiddenNeurons[i] * (1 - hiddenNeurons[i]) * tmp_val;
            }
        }

        //===================================================
        //===========FUNKCJE AKTYWACJI=======================
        /// <summary>
        /// Funkcja aktywacji dla HiddenLayer Tanh(x)
        /// </summary>
        /// <param name="x">x</param>
        /// <returns>2/(1+e^(-x))</returns>
        private static float activationFunctionTanh(float x)
        {
            return (float)(2/(1+Math.Exp(-x)));
        }

        /// <summary>
        /// Funkcja aktywacji dla OutputLayer sigmoid(x)
        /// </summary>
        /// <param name="x">x</param>
        /// <returns>1/(1+e^(-x))</returns>
        private static float activationFunctionSigmoid(float x)
        {
            return (float)(1 / (1 + Math.Exp(-x)));
        }

        //===================================================
        //===========FUNKCJE INICJUJACE======================
        /// <summary>
        /// Inicjalizacja list neuronow (+bias)
        /// </summary>
        protected void initNeurons()
        {
            //warstwa WE + bias
            inputNeurons = new float[numInput + 1];
            inputNeurons[numInput] = 1;

            //warstwa ukryta + bias
            hiddenNeurons = new float[numHidden + 1];
            hiddenNeurons[numHidden] = 1;

            //warstwa WY
            outputNeurons = new float[numOutput];
        }


        /// <summary>
        /// Inicjalizacja wag (-1,1)
        /// </summary>
        protected void initWeights()
        {
            weightsInputHidden = initArray(weightsInputHidden, numInput + 1, numHidden + 1);
            weightsHiddenOutput = initArray(weightsHiddenOutput, numHidden + 1, numOutput);

            weightsInputHidden = setRandomWeightPlusBIAS(weightsInputHidden, numInput + 1, numHidden + 1);
            weightsHiddenOutput = setRandomWeightPlusBIAS(weightsHiddenOutput, numHidden + 1, numOutput);
        }

        /// <summary>
        /// Inicjuje tablice wag miedzy podanymi warstwami,
        /// ostatnia waga w liscie to BIAS
        /// </summary>
        /// <param name="weightList">tablica wag do wpisania</param>
        /// <param name="numLayerFrom">ilosc neuronow w wartwie pierwszej</param>
        /// <param name="numLayerTo">ilosc neuronow w warstwie drugiej</param>
        /// <returns>zainicjowana liste wag</returns>
        protected float[][] setRandomWeightPlusBIAS(float[][] weightList, 
            int numLayerFrom, int numLayerTo)
        {
            Random r = new Random();

            for (int i = 0; i < numLayerFrom; i++)
            {
                for (int j = 0; j < numLayerTo; j++)
                {
                    //zainicjuj wagi, ostatnia to bias
                    if (i != numLayerFrom)
                        weightList[i][j] = (float)(r.NextDouble() * 2 - 1);
                    else
                        weightList[i][j] = BIAS;
                }
            }
            return weightList;
        }

        /// <summary>
        /// Inicjalizacja list z obl. delta-reguly
        /// </summary>
        protected void initDelta()
        {
            deltaInputHidden = initArray(deltaInputHidden, numInput + 1, 2);
            deltaHiddenOutput = initArray(deltaHiddenOutput, numHidden + 1, 2);
        }

        /// <summary>
        /// Inicjalizacja list przechowywujacych wyliczenia dla bledu
        /// </summary>
        protected void initGradient()
        {
            hiddenErrorGradient = new float[numHidden + 1];
            outputErrorGradient = new float[numOutput];
        }

        /// <summary>
        /// Funkcja w celu refaktoryzacji kodu, inicjuje tablice 2d
        /// </summary>
        /// <param name="array">tablica do zainicjowania</param>
        /// <param name="length">ilosc pozycji dla pierwszego wymiaru</param>
        /// <param name="num">ilosc pozycji dla drugiego wymiaru</param>
        /// <returns>zainicjowana tablica num-wymiarowa</returns>
        protected static float[][] initArray(float[][] array, int length, int num)
        {
            array = new float[length][];
            for (int i = 0; i < array.Length; i++)
                array[i] = new float[num];

            return array;
        }

    }
}
