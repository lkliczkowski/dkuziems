using System;
using System.Collections.Generic;

namespace zscore
{
	#region Average static class
	public static class Average
	{
		
		//srednia dla wartosci dyskretnych
		public static List<double> AverageDiscrete(List<double> MyRecordList, DiscreteType discreteType)
		{
			List<double> average = new List<double>();
			int num_of_states = 0;
			
			switch(discreteType)
			{
			case DiscreteType.Unknown:
				Console.WriteLine(">>>> averagediscrete.discreteType::unknown");
				break;
				
			case DiscreteType.Gender:
				Console.WriteLine(">>>> averagediscrete.discreteType::gender");
				average.Add(0); average.Add(0); average.Add(0);
				foreach(double ourRecord in MyRecordList)
				{
					switch ((int)ourRecord)
					{
					case (int)GenderEnum.Male:
						average[(int)GenderEnum.Male]+=1;
						num_of_states++;
						break;
					case (int)GenderEnum.Female:
						num_of_states++;
						average[(int)GenderEnum.Female]+=1;
						break;
					case (int)GenderEnum.Null:
						num_of_states++;
						average[(int)GenderEnum.Null]+=1;
						Console.WriteLine(">>>>>>>> null::during avarage.gender");
						break;
					default:
						Console.WriteLine(">>>>>>>> unknown::during avarage.gender");
						break;
					}
				}
				average[(int)GenderEnum.Male]/=num_of_states;
				average[(int)GenderEnum.Female]/=num_of_states;
				average[(int)GenderEnum.Null]/=num_of_states;
				break;
				
			case DiscreteType.Education:
				Console.WriteLine(">>>> averagediscrete.discreteType::education");
				break;
				
			default:
				Console.WriteLine(">>>> averagediscrete.discreteType::default");
				break;
			}

			//DisplayOther.DisplayDoubleList(average, "averagediscrete");
			return average;
		}
		
		//Srednia dla wartosci ciaglych
		public static double AverageContinuous(List<double> MyRecordList)
		{
			double average = 0;
			foreach(double ourRecord in MyRecordList)
			{
				average += ourRecord;
			}
			
			return average/(double)MyRecordList.Count;
		}
		
	}
	#endregion
	
	#region StandardDeviation static class
	public static class StandardDeviation
	{
		/* StdDevContinuousDiscrete(p, type_of_discrete)
		 * 
		 * sqrt(p(1-p))
		 */
		public static List<double> StdDevContinuousDiscrete(List<double> AverageDiscrete, DiscreteType discreteType)
		{
			List<double> stddev = new List<double>();
			
			switch(discreteType)
			{
			case DiscreteType.Unknown:
				Console.WriteLine(">>>> stddevdiscrete.discreteType::unknown");
				break;
				
			case DiscreteType.Gender:
				Console.WriteLine(">>>> stddevdiscrete.discreteType::gender");
				stddev.Add(0); stddev.Add(0); stddev.Add(0);
				
				stddev[(int)GenderEnum.Female] = 
					Math.Sqrt((AverageDiscrete[(int)GenderEnum.Female]*(1-AverageDiscrete[(int)GenderEnum.Female])));
				stddev[(int)GenderEnum.Male] = 
					Math.Sqrt((AverageDiscrete[(int)GenderEnum.Male]*(1-AverageDiscrete[(int)GenderEnum.Male])));
				stddev[(int)GenderEnum.Null] = 
					Math.Sqrt((AverageDiscrete[(int)GenderEnum.Null]*(1-AverageDiscrete[(int)GenderEnum.Null])));
				break;
				
			case DiscreteType.Education:
				Console.WriteLine(">>>> stddevdiscrete.discreteType::education");
				break;
				
			default:
				Console.WriteLine(">>>> stddevdiscrete.discreteType::default");
				break;
			}
			
			//DisplayOther.DisplayDoubleList(stddev, "stddevdiscrete");
			return stddev;
		}
		
		public static double StdDevContinuous(List<double> MyRecordList, double average)
		{
			double sumOfDerivation = 0;
			foreach(double ourRecord in MyRecordList)
			{
				sumOfDerivation += (double)ourRecord * (double)ourRecord;
			}
			
			sumOfDerivation = sumOfDerivation / MyRecordList.Count;
			return Math.Sqrt(sumOfDerivation - (average*average));
		}
		
	}
	#endregion
	
}
