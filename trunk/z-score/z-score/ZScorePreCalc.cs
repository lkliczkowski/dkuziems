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
		
		public static double AverageAge = 0;
		public static double StandardDeviationAge = 0;
		
		public static double AverageGender = 0;
		public static double StandardDeviationGender = 0;
		
		public static void InitVariables(List<RawRecord> RawRecordList)
		{
			AverageGender = Average.Gender(RawRecordList);
			StandardDeviationGender = StandardDeviation.Gender(RawRecordList, AverageGender);
			
			AverageIncome = Average.Income(RawRecordList);
			StandardDeviationIncome = StandardDeviation.Income(RawRecordList, AverageIncome);
			
			AverageAge = Average.Age(RawRecordList);
			StandardDeviationAge = StandardDeviation.Age(RawRecordList, AverageAge);
		}
		
		public static void RecordStats()
		{
			Console.WriteLine (">>>>AverageGender: {0}, \n>>>>StdDevGender {1}", 
			                   ZScorePreCalc.AverageGender, ZScorePreCalc.StandardDeviationGender);
			Console.WriteLine (">>>>AverageIncome: {0}, \n>>>>StdDevIncome {1}", 
			                   ZScorePreCalc.AverageIncome, ZScorePreCalc.StandardDeviationIncome);
			Console.WriteLine (">>>>AverageAge: {0}, \n>>>>StdDevAge {1}", 
			                   ZScorePreCalc.AverageAge, ZScorePreCalc.StandardDeviationAge);
			Console.WriteLine();
		}
		
		public static void AddSomeRecords(ref List<RawRecord> RawRecordList)
		{
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 46500, 33, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 39600, 40, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 63400, 34, "Yes");
			RawRecordOperator.AddRecord(ref RawRecordList, "Female", 40400, 43, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, null, 58000, 53, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, "Female", null, 23, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 59000, null, "Yes");
			RawRecordOperator.AddRecord(ref RawRecordList, null, 39100, 32, "Yes");
		}
		
	}
}
