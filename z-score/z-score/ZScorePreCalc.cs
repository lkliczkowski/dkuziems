using System;
using System.Collections.Generic;

namespace zscore
{
	public static class ZScorePreCalc
	{
		//srednia dla Income - liczona jednokrotnie - oszczednosc czasu dla
		//duzych zbiorow danych
		public static double AverageIncome = 0;
		//odchylenie standardowe dla Income...
		public static double StandardDeviationIncome = 0;
		public static void InitVariables(List<RawRecord> RawRecordList)
		{
			AverageIncome = Average.Income(RawRecordList);
			StandardDeviationIncome = StandardDeviation.Income(RawRecordList, AverageIncome);
			
		}
		
		
		public static void AddSomeRecords(ref List<RawRecord> RawRecordList)
		{
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 46500, 33, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 39600, 40, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 63400, 34, "Yes");
			RawRecordOperator.AddRecord(ref RawRecordList, "Female", 40400, 43, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, null, null, 43, "No");
		}
		
		public static void RecordStats()
		{
			Console.WriteLine (">>>>Average: {0}", ZScorePreCalc.AverageIncome);
			Console.WriteLine (">>>>StdDev {0}\n", ZScorePreCalc.StandardDeviationIncome);
		}
	}
}
