using System;
using System.Collections.Generic;

namespace zscore
{
	#region Average static class
	public static class Average
	{
		public static double Income(List<RawRecord> RawRecordList)
		{
			double average = 0;
			foreach(RawRecord ourRawRecord in RawRecordList)
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
	}
	#endregion
	
	#region StandardDeviation static class
	public static class StandardDeviation
	{
		
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
		
	}
	#endregion
	
}
