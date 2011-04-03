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
            const double eta = 1;

            run(initData(DATAFILE), eta);
        }

        private static List<double[]> initData(string filePath)
        {
            List<double[]> data = new List<double[]>();
            ReadData.ReadDoubleList(filePath, ref data);
            return data;
        }

        private static void run(List<double[]> learningSet, double eta)
        {

            List<double> bias = new List<double>();
            List<double[]> weights = new List<double[]>();
            double R = new Double();

            const int percent = 20;
            List<double[]> validateSet = SplitSetEqually(ref learningSet, percent);
			//List<double[]> validateSet = SplitSetRandomly(ref learningSet, percent);
			
            //Print("LEARNING SET", learningSet.Count.ToString());
            //PrintList(learningSet);
            //Print("TESTING SET", validateSet.Count.ToString());
            //PrintList(validateSet);

            initValues(ref weights, ref bias, ref R, learningSet);

            for (int t = 0, k = 0, success_count = 0; t < learningSet.Count; )
            {
                Console.WriteLine("\n:::::::::::::: [i == {0}] ::::::::::::::::", t + 1);
				//if (Classify(validateSet[k], weights[t], bias.Last() == validateSet[k][validateSet[k].Length - 1]) 
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
                if (++k >= validateSet.Count) k = 0; 
            }
			
			Println();
            Print("WAGI");
            PrintList(weights);
			Println();
            Print("BIAS'y");
            PrintList(bias);


        }

        private static double Classify(double[] Xs, double[] w, double b)
        {
            double fx = 0;

            for (int i = 0; i < Xs.Length - 1; i++)
                fx += Xs[i] * w[i];

            if ((fx + b) >= 0)
                return 1;
            else
                return -1;
        }

        private static void initValues(ref List<double[]> weights,
            ref List<double> bias, ref double R, List<double[]> learningSet)
        {
            double[] w = new double[learningSet[0].Length - 1];
            for (int j = 0; j < w.Length; j++)
                w[j] = 0;

            weights.Add(w);
            bias.Add(0);
            R = MaxFromArray(VectorNorm(learningSet).ToArray());
        }

        private static List<double[]> SplitSetEqually(ref List<double[]> Set, int percent)
        {
            List<double[]> validateSet = new List<double[]>();
            int multiple = 100 / percent; 
			
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
