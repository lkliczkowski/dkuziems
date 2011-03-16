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
		
		//public static List<string>DiscreteList = new List<string>();
		//public static List<double>ContinuousList = new List<double>();
		
		public static void InitVariables(List<RawRecord> RawRecordList)
		{
			List<string>DiscreteList = new List<string>();
			List<double>DiscretizedList = new List<double>();
			List<double>ContinuousList = new List<double>();
			
			DiscreteType discreteType;

			discreteType = DiscreteType.Gender;
			DiscreteList.Clear();
			foreach(RawRecord ourRawRecord in RawRecordList)
			{
				if(ourRawRecord.Gender != null)
					DiscreteList.Add((string)ourRawRecord.Gender);
				else
					DiscreteList.Add("Null");
			}
			DiscretizedList = ZScoreDiscretize.Discretize(DiscreteList, discreteType);
			foreach(double dr in DiscretizedList)
				Console.WriteLine(">>{0}", dr);
			AverageGender = Average.Gender(RawRecordList);
			StandardDeviationGender = StandardDeviation.Gender(RawRecordList, AverageGender);
			
			
			
			
			ContinuousList.Clear();
			foreach(RawRecord ourRawRecord in RawRecordList)
			{
				if(ourRawRecord.Income.HasValue)
					ContinuousList.Add((double)ourRawRecord.Income);
			}
			//AverageIncome = Average.Income(RawRecordList);
			AverageIncome = Average.AverageContinuous(ContinuousList);
			//StandardDeviationIncome = StandardDeviation.Income(RawRecordList, AverageIncome);
			StandardDeviationIncome = StandardDeviation.StdDevContinuous(ContinuousList, AverageIncome);
			
			
			
			ContinuousList.Clear();
			foreach(RawRecord ourRawRecord in RawRecordList)
			{
				if(ourRawRecord.Age.HasValue)
					ContinuousList.Add((double)ourRawRecord.Age);
			}
			//AverageAge = Average.Age(RawRecordList);
			AverageAge = Average.AverageContinuous(ContinuousList);
			//StandardDeviationAge = StandardDeviation.Age(RawRecordList, AverageAge);
			StandardDeviationAge = StandardDeviation.StdDevContinuous(ContinuousList, AverageAge);
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
