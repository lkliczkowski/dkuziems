using System;
using System.Collections.Generic;

namespace zscore
{
	#region Average static class
	public static class Average
	{
		
		public static double Gender(List<RawRecord> MyRecordList)
		{
			//double average = 0;
			int state = 0, num_of_states = 0;
			
			foreach(RawRecord ourRawRecord in MyRecordList)
			{
				switch (ourRawRecord.Gender) 
				{
				case "Male":
					state++;
					num_of_states++;
					break;
				case "Female":
					num_of_states++;
					break;
				case null:
					Console.WriteLine(">>>>>>>> null::during avarage.gender");
					break;
				default:
					Console.WriteLine(">>>>>>>> unknown::during avarage.gender");
					break;
				}
			}
			
			return (double)state/(double)num_of_states;
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
		public static double Gender(List<RawRecord> MyRecordList, double average)
		{
			return Math.Sqrt((average*(1-average)));
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
