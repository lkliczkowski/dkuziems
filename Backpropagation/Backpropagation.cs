using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backpropagation
{
    class Backpropagation : FFNNStructure
    {
        private ZScore.ZScore Dataset;

        private DatasetOperate DatasetIndexes;// = new GrowingDatasetOperate(10000);

        private Backpropagation()
        { }

        /// <summary>
        /// Konstruktor klasy propagacji wstecznej
        /// </summary>
        /// <param name="lr">wspolczynnik uczenia (learning rate)</param>
        /// <param name="nI">liczba neuronow warstwy WE</param>
        /// <param name="nH">liczba neuronow warstwy ukrytej</param>
        /// <param name="nO">liczba neuronow warstwy WY</param>
        /// <param name="Data">dane</param>
        public Backpropagation(ZScore.ZScore dataset, int nH, float lr)
        {
            Dataset = dataset;
            DatasetIndexes = new DatasetOperate(Dataset.Data[0].GetNum());

            learningRate = lr;
            DesiredAccuracy = 0.5f;

            numInput = Dataset.Data.Length - 2; //2 ostatnie kolumny to y_i (tu 0/1)
            numHidden = nH;
            numOutput = 1;

            initNeurons();
            initWeights();
            initDelta();

            Epoch = new int();
            MaxEpoch = 500;

            for (int i = 0; i < Dataset.Data.Length - 1; i++)
            {
                Console.Write("{0}\t", Dataset.SingleRecord(0)[i]);
            }
        }

        

        public bool Run()
        {
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < DatasetIndexes.TrainingSet.Length; i++)
                {
                    Print("epoch", i.ToString());
                    float[] sample = Dataset.SingleRecord(DatasetIndexes.TrainingSet[i]);
                    calculateOfOutputsForOutputAndHiddenLayer(sample);
                    calculateOfErrorsForHiddenAndOutputLayer(sample);
                    calculateNewWeights();
                }
                endEpoch();
                Stats();
                Console.ReadKey();
            }

            return false;
        }

        private void endEpoch()
        {
            Epoch++;
            DatasetIndexes.IncreaseRange();

            weightsInputHidden = tempWeightsInputHidden;
            weightsHiddenOutput = tempWeightsHiddenOutput;
        }

        /// <summary>
        /// Inicjuje wyliczenie nowych wag
        /// </summary>
        private void calculateNewWeights()
        {
            tempWeightsHiddenOutput = calculateWeights(tempWeightsHiddenOutput, learningRate, deltaOutput, hiddenNeurons);
            tempWeightsInputHidden = calculateWeights(tempWeightsInputHidden, learningRate, deltaHidden, inputNeurons);

        }

        /// <summary>
        /// Oblicza nowe wagi weg reguly delta
        /// </summary>
        /// <param name="weightList">lista wag</param>
        /// <param name="l">parametr uczacy</param>
        /// <param name="delta">lista gradientow</param>
        /// <param name="neurons">lista neronow x_i</param>
        /// <returns></returns>
        private static float[][] calculateWeights(float[][] weightList, float l, float[] delta, float[] neurons)
        {
            for (int i = 0; i < weightList.Length; i++)
            {
                for (int j = 0; j < weightList[i].Length; j++)
                {
                    weightList[i][j] += l * delta[j] * neurons[i];
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
                (inputNeurons, hiddenNeurons, tempWeightsInputHidden, false);
            
            //wyliczamy wartosci na WY
            outputNeurons = calculateOfOutputs
                (hiddenNeurons, outputNeurons, tempWeightsHiddenOutput,  true);
        }

        //private static float[] calculateOfError(float[] inputs) { return inputs; }


        /// <summary>
        /// Wylicza wartosci dla wskazanej tablicy danych i wybranych warstw
        /// </summary>
        /// <param name="layerFrom">warstwa WE (z ktorej przychodzi sygnal</param>
        /// <param name="currLayer">warstwa, dla ktorej wyliczamy wartosci</param>
        /// <param name="weightList">wagi pomiedzy warstwami</param>
        /// <param name="useSigmoid">okresla ktora funkcje aktywacji uzywamy:
        /// dla hiddenLayer Tanh(x), dla outputLayer sigmoid(x)</param>
        /// <returns>obliczone wartosci dla currLayer</returns>
        private static float[] calculateOfOutputs
            (float[] layerFrom, float[] currLayer, float[][] weightList, 
            bool useSigmoid)
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
            }
            return currLayer;
        }

        /// <summary>
        /// sum-of-squares error,
        /// delta_k = y_k * (1 - y_k) * (d_k - y_k)
        /// delta_j = y_j * (1 - y_j) * Sum (w_jk * delta_k)
        /// </summary>
        /// <param name="inputs">lista wartosci na WE, 
        /// zakladamy, ze ostatni element jest WY</param>
        private void calculateOfErrorsForHiddenAndOutputLayer(float[] inputs)
        {
            int placeOf_y_i = 2; //drugi od konca to kolumna z przewidywana wartoscia w danych
            for (int i = 0; i < numOutput; i++)
            {
                deltaOutput[i] = outputNeurons[i] * (1 - outputNeurons[i])
                    * (inputs[inputs.Length - placeOf_y_i] - deltaOutput[i]);
            }

            for (int i = 0; i < numHidden + 1; i++)
            {
                //tmp_val = Err_j*w_ij
                float tmp_val = new float();
                for (int j = 0; j < numOutput; j++)
                {
                    tmp_val = deltaOutput[j] * tempWeightsHiddenOutput[i][j];
                }
                deltaHidden[i] = hiddenNeurons[i] * (1 - hiddenNeurons[i]) * tmp_val;
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


            tempWeightsInputHidden = initArray(tempWeightsInputHidden, numInput + 1, numHidden + 1);
            tempWeightsHiddenOutput = initArray(tempWeightsHiddenOutput, numHidden + 1, numOutput);

            tempWeightsInputHidden = setRandomWeightPlusBIAS(tempWeightsInputHidden, numInput + 1, numHidden + 1);
            tempWeightsHiddenOutput = setRandomWeightPlusBIAS(tempWeightsHiddenOutput, numHidden + 1, numOutput);
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
                    if (i != numLayerFrom - 1)
                        weightList[i][j] = (float)(r.NextDouble() * 2 - 1);
                    else
                        weightList[i][j] = BIAS;
                }
            }
            return weightList;
        }

        /// <summary>
        /// Inicjalizacja list przechowywujacych wyliczenia dla delty
        /// </summary>
        protected void initDelta()
        {
            deltaHidden = new float[numHidden + 1];
            deltaOutput = new float[numOutput];
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
