using System;
using System.Collections;
using System.Collections.Generic;

using MyData;

namespace FunahashiNeuralNetwork
{
	public class Backpropagation
	{
        /// <summary>
        /// gradient funkcji błędu dla WY, na jego podstawie są ustalane zmiany wag pomiędzy hidden-output
        /// </summary>
		private double outputErrorGradient;
        /// <summary>
        /// gradient błędu dla warstwy ukrytej, na jego podstawie są ustalane zmiany wag pomiędzy input-hidden
        /// </summary>
		private double[] hiddenErrorGradients;

        /// <summary>
        /// wektor delta (suma outputErrorGradient)
        /// </summary>
        private double[] deltaHiddenOutput;
        /// <summary>
        /// wektor delta (suma hiddenErrorGradients)
        /// </summary>
        private double[][] deltaInputHidden;

        /// <summary>
        /// Czy korzystać z BatchLearning
        /// </summary>
        private bool useBatchLearning;
		
        /// <summary>
        /// współczynnik uczenia
        /// </summary>
		private double learningRate;

        /// <summary>
        /// Mean squared error (suma kwadratów błędów uczenia)
        /// </summary>
		private double mse;

        /// <summary>
        /// licznik epok, maksymalna liczba epok
        /// </summary>
		private ulong epoch, maxEpoch;

        /// <summary>
        /// sieć neuronowa MLP z jednym wyjściem
        /// </summary>
		private NeuralNetwork NN;

        /// <summary>
        /// Obsługuje przepływ danych:
        /// - Pattern(int i) - dane WE dla wskazanego indeksu i
        /// - Target(int i) - docelowe WY dla wskazanego indeksu i
        /// Inicjajcja DataSet("ścieżkaDoPliku")
        /// </summary>
		private DataSet dataset;
		
        /// <summary>
        /// Przedziały funkcji (dziedzina)
        /// </summary>
		private double domainFrom, domainTo;

        /// <summary>
        /// plik do którego zapisywane są wyniki
        /// </summary>
		private string resultFilename;
		
        /// <summary>
        /// konstruktor dla backprop
        /// </summary>
        /// <param name="hiddenRatio">liczba neuronów w warstwie ukrytej</param>
        /// <param name="lr">współczynnik uczenia</param>
        /// <param name="mE">maksymalna liczba epok</param>
        /// <param name="path">ścieżka do pliku z danymi</param>
        /// <param name="results">ściezka do pliku z wynikami</param>
        /// <param name="df">dziedzina (przedział od)</param>
        /// <param name="dt">dziedzina (przedział do)</param>
		public Backpropagation (int hiddenRatio, double lr, ulong mE, string path, string results, double df, double dt)
		{
            //tworzymy zbiór danych
			dataset = new DataSet(path);
            //tworzymy sieć z ilością wejść równą wielkości pojedynczej próbki
			NN = new NeuralNetwork(dataset.LengthOfPattern, hiddenRatio);
			
			maxEpoch = mE;
			learningRate = lr;
			
            //inicjacja wektora błędu
			hiddenErrorGradients = new double[NN.numHidden];
			
			resultFilename = results;
			domainFrom = df;
			domainTo = dt;

            useBatchLearning = false;

            deltaHiddenOutput = new double[NN.numHidden + 1];
            deltaInputHidden = new double[NN.numInput + 1][];
            for (int i = 0; i <= NN.numInput; i++)
            {
                deltaInputHidden[i] = new double[NN.numHidden];
            }
		}
		
        /// <summary>
        /// Uczenie sieci, główna funkcja
        /// </summary>
		public void Train()
		{
			epoch = 0;
            bool done = false;
            Hashtable errorTable = new Hashtable();

            ulong howOftenSaveResult = 50000 / (ulong)Math.Pow(10, dataset.LengthOfPattern - 1);
            howOftenSaveResult /= (ulong)(dataset.Count / 10 + 1);

            Console.WriteLine("Wyniki będą automatycznie zapisywane do pliku {0} co {1} epok", 
                resultFilename, howOftenSaveResult);

            Console.WriteLine("\n{0}\t{1}", "Epoka", "MSE");
			while(epoch < maxEpoch && !done)
			{
				epoch++;
				for(int i = 0; i < dataset.Count; i++)
				{
					//faza w przód
					NN.FeedForward(dataset.Pattern(i));
					//faza wstecz
					backward(dataset.Target(i));
					//mean square error
					mse += Math.Pow(dataset.Target(i) - NN.OutputNeuron, 2); 
				}
				mse /= dataset.Count;

                if(useBatchLearning)
                    updateWeights();

				//co n epok wyświetla info o stanie nauczania
                if (epoch % (howOftenSaveResult / 50 + 1) == 0)
                {
                    Console.WriteLine("{0}\t{1}", epoch, mse);
                }

				//co n epok wyświetla info o stanie nauczania
                if (epoch % ((howOftenSaveResult * 2) / 50 + 1) == 0)
				{
                    errorTable.Add(epoch, mse);
                }
				
                try
                {
                    //co n epok zapisujemy wyniki
                    if ((epoch % howOftenSaveResult) == 0)
                    {
                        if (!SaveResults(domainFrom, domainTo, dataset.Count, resultFilename))
                            Console.WriteLine("Nieudany zapis wyników!");
                    }
                }
                catch (DivideByZeroException)
                {
                    if ((epoch % howOftenSaveResult + 1) == 0)
                    {
                        if (!SaveResults(domainFrom, domainTo, dataset.Count, resultFilename))
                            Console.WriteLine("Nieudany zapis wyników!");
                    }
                }
				
                if (mse < 0.05)
                    done = true;
				
				//zerujemy błąd
				mse = 0;
			}
		}
		
        /// <summary>
        /// faza WSTECZ, wyliczamy gradient błędu i aktualizujemy od razu wagi (uczenie on-line),
        /// lepsze wyniki (szybsza zbieżność) można osiagnąć w uczeniu stochastycznym (czyli on-line) 
        /// stosując dodatkowo "momentum" lub rezygnując z uczenia on-line i stosując "batch learning" gdzie 
        /// aktualizacja wag następuje na koniec każdej epoki na podstawie zmian wag, które są sumowane 
        /// do nowej zmiennej w trakcie backpropagate (np.: do deltaHiddenOutput dla wag hidden-output)
        /// </summary>
        /// <param name="desiredOutput">wartość data.Target(i)</param>
		void backward(double desiredOutput)
		{
            //błąd dla wyjścia
			outputErrorGradient = (desiredOutput - NN.OutputNeuron) *
				derivationOfActivation(NN.weightedSumOutput);
			for(int j = 0; j < NN.numHidden; j++)
			{
                if(useBatchLearning)
				    deltaHiddenOutput[j] += learningRate * outputErrorGradient * NN.HiddenNeurons[j];
                else
                    NN.wHiddenOutput[j] += learningRate * outputErrorGradient * NN.HiddenNeurons[j];
			}
			
            //błąd dla warstwy ukrytej
			for(int j = 0; j < NN.numHidden; j++)
			{
				hiddenErrorGradients[j] = outputErrorGradient * NN.wHiddenOutput[j] 
                    * derivationOfActivation(NN.weightedSumHidden[j]);
				for(int i = 0; i < NN.numInput; i++)
				{
                    if(useBatchLearning)
					    deltaInputHidden[i][j] += learningRate * hiddenErrorGradients[j] * NN.Inputs[i];
                    else
                        NN.wInputHidden[i][j] += learningRate * hiddenErrorGradients[j] * NN.Inputs[i];
				}
			}
		}

        /// <summary>
        /// aktualizacja wag
        /// </summary>
        private void updateWeights()
        {
            for (int j = 0; j < NN.numHidden; j++)
            {
                NN.wHiddenOutput[j] += deltaHiddenOutput[j];
                deltaHiddenOutput[j] = 0;
            }

            for (int j = 0; j < NN.numHidden; j++)
            {
                for (int i = 0; i < NN.numInput; i++)
                {
                    NN.wInputHidden[i][j] += deltaInputHidden[i][j];
                    deltaInputHidden[i][j] = 0;
                }
            }
        }
				
        /// <summary>
        /// pochodna z funkcji aktywacji
        /// </summary>
        /// <param name="x">suma ważonych wejść</param>
        /// <returns>pochodna dla funkcji aktywacji w punkcie net_ij</returns>
		double derivationOfActivation(double x)
		{
			double h = 0.000001;
			return (NN.ActivationFunction(x+h) - NN.ActivationFunction(x)) / h;
		}
		
        /// <summary>
        /// wylicza wyniki działania sieci i zapisuje do pliku
        /// </summary>
        /// <param name="domainFrom">dziedzina od</param>
        /// <param name="domainTo">dziedzina do</param>
        /// <param name="howMany">ile punktów ma zostać zbadane na przedziale</param>
        /// <param name="path">ścieżka do pliku</param>
        /// <returns>"true" dla udanego zapisu do pliku</returns>
        public bool SaveResults(double domainFrom, double domainTo, int howMany, string path)
        {
            double[] xs = new double[dataset.LengthOfPattern];

            List<double[]> results = new List<double[]>();
            List<double> singleRecord = new List<double>();
            Random r = new Random();

            for (int i = 0; i < howMany; i++)
            {
                if (dataset.LengthOfPattern == 1)
                {
                    xs[0] = domainFrom + Math.Abs(domainFrom - domainTo) / howMany * (i + 1);
                }
                else
                {
                    for (int j = 0; j < dataset.LengthOfPattern; j++)
                    {

                        xs[j] = r.NextDouble() * Math.Abs(domainFrom - domainTo) + domainFrom;
                    }
                }
                NN.FeedForward(xs);
                foreach (double d in xs)
                {
                    singleRecord.Add(d);
                }

                singleRecord.Add(NN.OutputNeuron);

                results.Add(singleRecord.ToArray());
                singleRecord.Clear();
            }

            //plik nagłówkowy umieszczony w komentarzu w pierwszej linii
            string header = String.Format("Epoka: {0}\tBłąd: {1}\tj.ukrytych: {2}\tlr: {3}",
                                          epoch, mse, NN.numHidden, learningRate);

            if (DataWrite.WriteData(path, results, header))
                Console.WriteLine("Zapisano wyniki w {0}", path);
            else return false;

            return true;
        }
	}
}

