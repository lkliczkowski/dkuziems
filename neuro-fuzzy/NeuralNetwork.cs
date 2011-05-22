using System;

namespace FunahashiNeuralNetwork
{
    /* Struktura sieci
     * inputs(+bias)->hiddens(+bias)->output
     * 
     * funkcje aktywacji dla neuronów w warstwie ukrytej - sigmoid unipolarny
     * aktywacja dla wyjœcia - liniowa
     * 
     * */
	public class NeuralNetwork
    {
        #region parametry

        /// <summary>
        /// tzw. "wejœcie zerowe", bias
        /// </summary>
		public const int BIAS = 1;

        /// <summary>
        /// liczba wejœæ
        /// </summary>
        public int numInput { get; set; }
        /// <summary>
        /// liczba jednostek ukrytych
        /// </summary>
        public int numHidden { get; set; }

        /// <summary>
        /// wejœcia
        /// </summary>
        public double[] Inputs { get; set; }
        /// <summary>
        /// neurony w warstwie ukrytej
        /// </summary>
        public double[] HiddenNeurons { get; set; }
        /// <summary>
        /// wyjœcie
        /// </summary>
        public double OutputNeuron { get; set; }
		
        /// <summary>
        /// suma wa¿ona od neuronów wejœciowych do warstwy ukrytej
        /// </summary>
		public double[] weightedSumHidden { get; set; }
        /// <summary>
        /// od hidden do output
        /// </summary>
		public double weightedSumOutput { get; set; }

        /// <summary>
        /// Wagi pomiedzy WE a warstwa ukryta
        /// </summary>
        public double[][] wInputHidden { get; set; }

        /// <summary>
        /// Wagi pomiedzy warstwa ukryta a WY
        /// </summary>
        public double[] wHiddenOutput { get; set; }

        #endregion

        /// <summary>
        /// konstruktor sieci
        /// </summary>
        /// <param name="nI">liczba wejœæ</param>
        /// <param name="nH">liczba jednostek ukrytych</param>
		public NeuralNetwork (int nI, int nH)
		{
			numInput = nI;
			numHidden = nH;
			
			Inputs = new double[numInput + 1];
			Inputs[numInput] = BIAS;
			
			HiddenNeurons = new double[numHidden + 1];
			HiddenNeurons[numHidden] = BIAS;
			
			weightedSumHidden = new double[numHidden];

			initializeWeights();
		}
		
        /// <summary>
        /// faza w przód dla podanej próbki danych
        /// </summary>
        /// <param name="Patterns">próbka danych</param>
		public void FeedForward(double[] Patterns)
		{
			//przypisujemy wartosci na WE (wejsciu)
            for (int i = 0; i < numInput; i++) 
				Inputs[i] = Patterns[i];
			
			//dla ukrytej
			for(int j=0; j < numHidden; j++)
			{
				weightedSumHidden[j] = 0;				
				
				for( int i=0; i <= numInput; i++ ) 
					weightedSumHidden[j] += Inputs[i] * wInputHidden[i][j];
				
				HiddenNeurons[j] = ActivationFunction( weightedSumHidden[j] );			
			}
			
			//wyliczamy wartosci na WY
			weightedSumOutput = 0;				
				
			for( int j=0; j <= numHidden; j++ ) 
				weightedSumOutput += HiddenNeurons[j] * wHiddenOutput[j];
			
            //wyjœcie liniowe
			OutputNeuron = weightedSumOutput;

		}
		
		/// <summary>
		/// funkcja aktywacji Sigmoidalna Unipolarna
		/// </summary>
		/// <param name="x">najczêœæiej suma wa¿onych wejœæ</param>
		/// <returns></returns>
		public double ActivationFunction(double x)
		{
			return 2 / (1 + Math.Exp(-x)) - 1;
		}
		
        /// <summary>
        /// Inicjalizacja wag i przypisanie im wartoœci losowych
        /// </summary>
		private void initializeWeights()
		{
			Random r = new Random();
			wInputHidden = new double[numInput + 1][];
	        for ( int i = 0; i <= numInput; i++ ) 
	        {
		        wInputHidden[i] = new double[numHidden];
				for(int j = 0; j < numHidden; j++)
				{
					if(i != numInput)
					{
						wInputHidden[i][j] = (r.NextDouble() * 2 - 1);
					}
					else
						wInputHidden[i][j] = BIAS;	//ostatnia waga to BIAS
				}
	        }

            wHiddenOutput = new double[numHidden + 1];
	        for ( int i = 0; i <= numHidden; i++ ) 
	        {
				if(i != numHidden)
					wHiddenOutput[i] = (r.NextDouble() * 2 - 1);
				else
					wHiddenOutput[i] = BIAS;	//ostatnia waga to BIAS
	        }
		}
	}
}

