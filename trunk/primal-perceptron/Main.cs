using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimalPerceptronAlgorithm
{
    partial class Program
    {
        static void Main(string[] args)
        {
            const string DATAFILE = "perc.txt";


            /* eta 		wspolczynnik uczenia
             */
            const double eta_02 = 0.2;
			const string OUTPUTGNUPLOT_02 = "graphs_02.gp", graphName_02 = "perceptron02";
            run(initData(DATAFILE), eta_02, OUTPUTGNUPLOT_02, graphName_02);
			
			/*
			const double eta_06 = 0.6;
			const string OUTPUTGNUPLOT_06 = "graphs_06.gp", graphName_06 = "perceptron06";
            run(initData(DATAFILE), eta_06, OUTPUTGNUPLOT_06, graphName_06);
			
			const double eta_10 = 1;
			const string OUTPUTGNUPLOT_10 = "graphs_10.gp", graphName_10 = "perceptron10";
            run(initData(DATAFILE), eta_10, OUTPUTGNUPLOT_10, graphName_10);
            */
			
        }

        private static List<double[]> initData(string filePath)
        {
            List<double[]> data = new List<double[]>();
            ReadData.ReadDoubleList(filePath, ref data);
            return data;
        }

        private static void run(List<double[]> learningSet, double eta, string outputGnuplot, string graphName)
        {
            /*
             * bias(b_t) 		tzw parametr sterujacy
             */
            List<double> bias = new List<double>();

            /*
             * weights(w_t) 	wagi
             */
            List<double[]> weights = new List<double[]>();

            /* 
             * maks norma wektora (potrzebna przy zmianach bias
             */ 
            double R = new Double();


            /* <x,y> 
             * x 	przyklady (set[][0] do set[][n-1])
             * y 	etykiety przykladu (data[n] - ostatni w kazdym wierszu)
             * 
             * podzielimy set[n] na zbiory uczacy(treningowy) i walidacyjny 
             * w stosunku 1:4, 
             * probka rownomierna - SplitSetEqually
             * probka losowa - SplitSetRandomly
             */
            const int percent = 15; //or number
			List<double[]> validateSet = SplitSetTakeNumber(ref learningSet, percent);
            //List<double[]> validateSet = SplitSetEqually(ref learningSet, percent);
			//List<double[]> validateSet = SplitSetRandomly(ref learningSet, percent);
			
            //Print("LEARNING SET", learningSet.Count.ToString());
            //PrintList(learningSet);
            //Print("TESTING SET", validateSet.Count.ToString());
            //PrintList(validateSet);
			
			int t = new int(); //licznik iteracji

            /* 
             * (a) Inicjalizacja
             */ 
            initValues(ref t, ref weights, ref bias, ref R, learningSet);

            /*
             * (b) Powtarzaj...
             * =================================================================
             * t		iterator po kolejno dobieranych wagach i biasach,
             * 
             * dodatkowo:
             * k				iterator po trainingSet 
             * 					(bedziemy brali kolejne przyklady sprawdzajac poprawnosc klasyfikatora)
             * success_count	warunek stopu -> jesli poprawnie sklasyfikuje caly zbior 
             * 					walidacyjny przerwij nauke	
             * =================================================================
             *
             * if(sukces w klasyfikacji)
             * {
             * 		zwieksz iteracje sukcesow
             * 		jesli == wielkosci zbioru walidacyjnego -> stop
             * }
             * else
             * {
             * 		(i) 	w_{t+1} = w_t + eta*y_i*x_i	//dla kazdej wagi w_i
             * 		(ii) 	b_{t+1} = b_t + eta*y_i*R^2
             * 		(iii) 	t++
             * }
             * ...dopoki perceptron nie popelni zadnego bledu 
             * (tu: przejdzie bezblednie przez caly zbior walidacyjny lub przez caly zbior uczacy)
             * 
             * Koniec
             * 
             */
            for (int k = 0, success_count = 0; t < learningSet.Count; )
            {
                Console.WriteLine("\n:::::::::::::: [i == {0}] ::::::::::::::::", t + 1);
				//if (Classify(validateSet[k], weights[t], bias.Last() == validateSet[k][validateSet[k].Length - 1]) //w .net 2.0 nie ma List<T>.Last()
                if (Classify(validateSet[k], weights[t], bias[bias.Count - 1]) == validateSet[k][validateSet[k].Length - 1])
                {
					success_count++;
                    Print("Klasyfikacja udana", success_count.ToString());
					PrintArray<double>(validateSet[k], "sample");
                }
                else
                {
                    Print("Klasyfikacja niepowodzenie!");
                    success_count = 0; 
					
					PrintArray<double>(validateSet[k], "sample");

                    /* (i) aktualizujemy wagi */
                    double[] tmp_weight = new double[learningSet[0].Length - 1];
                    for (int j = 0; j < tmp_weight.Length; j++)
                        tmp_weight[j] = 0;
                    for (int j = 0; j < tmp_weight.Length; j++)
                    {
                        tmp_weight[j] = weights[weights.Count - 1][j] + eta * learningSet[t][learningSet[t].Length - 1] * learningSet[t][j];
                    }
					PrintArray<double>(weights[weights.Count - 1], "wagi w_t");
                    weights.Add(tmp_weight);
					PrintArray<double>(weights[weights.Count - 1], "wagi w_t+1");
					
                    /* (ii) aktualizujemy bias */
                    double b = new double();
                    //b = bias[bias.Count - 1] + eta * learningSet[t][learningSet[t].Length - 1] * Math.Pow(R, 2);
					b = bias[bias.Count - 1] + eta * learningSet[t][learningSet[t].Length - 1] * Math.Pow(R, 2);
					Print("bias w_t: ", bias[bias.Count - 1]);
                    bias.Add(b);
					Print("bias w_t+1: ", bias[bias.Count - 1]);
					
                    t++;
                }

                if (success_count == validateSet.Count) 
				{
					Print("Poprawnie rozpoznano wszystkie przypadki walidacyjne");
					break;
				}
                if (++k >= validateSet.Count) k = 0; //elementy z validate od poczatku
            }
			
			Println();
            Print("WAGI");
            PrintList(weights);
			Println();
            Print("BIAS'y");
            PrintList(bias);
			
			Println();
			Print("Liczba iteracji na zbiorze uczacym", t.ToString());
			Print("zbiór walidacyjny", validateSet.Count);
			Print("uczacy", learningSet.Count);
			
			const int multipleGraph = 10;
			SaveToGnuplot.Save(outputGnuplot, graphName, weights, bias, multipleGraph, eta);
			//SaveToGnuplot.SaveWithAdd(outputGnuplot, graphName, weights, bias, multipleGraph, eta);
			
        }

        private static double Classify(double[] Xs, double[] w, double b)
        {
            double fx = 0;

            //f(x) = <x,w> + b = (Suma_{i=1}^{n}x_i * w_i) + b
            for (int i = 0; i < Xs.Length - 1; i++)
                fx += Xs[i] * w[i];

            if ((fx + b) >= 0)
                return 1;
            else
                return -1;
        }

        private static void initValues(ref int t, ref List<double[]> weights,
            ref List<double> bias, ref double R, List<double[]> learningSet)
        {
			t = 0; //formalizm...
            double[] w = new double[learningSet[0].Length - 1];
            for (int j = 0; j < w.Length; j++)
                w[j] = 0;
			
            weights.Add(w);
            bias.Add(0);
            R = MaxFromArray(VectorNorm(learningSet).ToArray());
        }

        private static List<double[]> SplitSetTakeNumber(ref List<double[]> Set, int how_many)
        {
			int m = Set.Count / how_many;
            List<double[]> validateSet = new List<double[]>();

            for (int i = Set.Count - 1; i > 1; i--)
            {
                if (((i+1) % m) == 0)
                {
                    validateSet.Add(Set[i]);
                    Set.RemoveAt(i);
                }
            }

            return validateSet;
        }
		
		private static List<double[]> SplitSetEqually(ref List<double[]> Set, int percent)
        {
            List<double[]> validateSet = new List<double[]>();
            int multiple = 100 / percent;   //co ktory przyklad bierzemy...

            for (int i = Set.Count - 1; i > 1; i--)
            {
                if (((i + 1) % multiple) == 0)
                {
                    validateSet.Add(Set[i]);
                    Set.RemoveAt(i);
                }
            }

            return validateSet;
        }

        private static List<double[]> SplitSetRandomly(ref List<double[]> Set, int percent)
        {
            int validateSetLength = (Set.Count * percent) / 100;
            List<double[]> validateSet = new List<double[]>();

            
            Random r = new Random();
            int j;
            for(int i = 0; i < validateSetLength; i++)
            {
                /*(int)DateTime.Now.Ticks - "zapewnia losowosc",
                na MonoDev sie sypie, w niektórych godzinach (-:
                wina srodowiska OS: Unix + .NET?*/
                //j = r.Next((((int)DateTime.Now.Ticks) % Set.Count)); 
				
                //pseudolosowe
                j = r.Next(Set.Count); 
				
                validateSet.Add(Set[j]);
                Set.RemoveAt(j);
            }
            


            return validateSet;
        }

        private static List<double> VectorNorm(List<double[]> vectorList)
        {
            List<double> norms = new List<double>();

            foreach (double[] xList in vectorList)
            {
                double val = 1;
                //bierzemy x-y bez ostatniego elementu y
                for (int i = 0; i < xList.Length - 1; i++)
                {
                    val *= xList[i];
                }
                norms.Add(Math.Sqrt(val));
                val = 1;
            }

            return norms;
        }

        private static double MaxFromArray(double[] array)
        {
            double max = 0;
            for (int i = 0; i < array.Length; i++)
            {
                max = Math.Max(max, array[i]);
            }
            return max;
        }
    }
}
