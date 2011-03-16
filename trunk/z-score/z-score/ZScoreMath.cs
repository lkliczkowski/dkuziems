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
		
		public static double Income(List<RawRecord> MyRecordList)
		{
			double average = 0;
			foreach(RawRecord ourRawRecord in MyRecordList)
			{
				if(ourRawRecord.Income.HasValue == true)
				{
					average += (double)ourRawRecord.Income;
				}
				else
				{
					Console.WriteLine(">>>>>>>> null::during avarage.income");
				}
				
			}
			
			return average/(double)RawRecordOperator.Counter;
		}
		
		public static double Age(List<RawRecord> MyRecordList)
		{
			double average = 0;
			foreach(RawRecord ourRawRecord in MyRecordList)
			{
				if(ourRawRecord.Age.HasValue == true)
				{
					average += (double)ourRawRecord.Age;
				}
				else
				{
					Console.WriteLine(">>>>>>>> null::during avarage.age");
				}
				
			}
			
			return average/(double)RawRecordOperator.Counter;
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
		
		/**@param average argument average przesyłamy jako obliczona wartosc 
		 * aby nie obliczac tej wartosci wielokrotnie, np. dla
		 * bazy 50.000 rekordow...
		 */
		public static double Income(List<RawRecord> MyRecordList, double average)
		{
			double sumOfDerivation = 0;
			foreach(RawRecord ourRawRecord in MyRecordList)
			{
				if(ourRawRecord.Income.HasValue == true)
				{
					sumOfDerivation += (double)ourRawRecord.Income * (double)ourRawRecord.Income;
				}
				else
				{
					Console.WriteLine(">>>>>>>> null::during standarddeviation.income");
				}
			}
			
			sumOfDerivation = sumOfDerivation / RawRecordOperator.Counter;
			return Math.Sqrt(sumOfDerivation - (average*average));
		}
		
		public static double Age(List<RawRecord> MyRecordList, double average)
		{
			double sumOfDerivation = 0;
			foreach(RawRecord ourRawRecord in MyRecordList)
			{
				if(ourRawRecord.Age.HasValue == true)
				{
					sumOfDerivation += (double)ourRawRecord.Age * (double)ourRawRecord.Age;
				}
				else
				{
					Console.WriteLine(">>>>>>>> null::during standarddeviation.Age");
				}
			}
			
			sumOfDerivation = sumOfDerivation / RawRecordOperator.Counter;
			return Math.Sqrt(sumOfDerivation - (average*average));
		}
		

		
	}
	#endregion
	
}
