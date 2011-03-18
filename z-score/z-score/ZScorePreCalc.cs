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
		public static List<double> AverageGenderList = new List<double>();
		public static double StandardDeviationGender = 0;
		public static List<double> StandardDeviationGenderList = new List<double>();
		
		//public static List<string>DiscreteList = new List<string>();
		//public static List<double>ContinuousList = new List<double>();
		
		public static void InitVariables(List<RawRecord> RawRecordList)
		{
			List<string>DiscreteList = new List<string>();
			List<double>DiscretizedList = new List<double>();
			List<double>ContinuousList = new List<double>();
			
			DiscreteType discreteType;

			//Gender
			DiscreteList.Clear();
			discreteType = DiscreteType.Gender;
			foreach(RawRecord MyRecord in RawRecordList)
			{
				if(MyRecord.Gender != null)
					DiscreteList.Add((string)MyRecord.Gender);
				else
					DiscreteList.Add("Null");
			}
			DiscretizedList = ZScoreDiscretize.Discretize(DiscreteList, discreteType);
			//DisplayOther.DisplayDoubleList(DiscretizedList, "discretized"); //sprawdzenie wyniku mapowania
				
			
			AverageGenderList = Average.AverageDiscrete(DiscretizedList, discreteType);
			StandardDeviationGenderList = StandardDeviation.StdDevContinuousDiscrete(AverageGenderList, discreteType);
			
			//Income
			ContinuousList.Clear();
			foreach(RawRecord ourRawRecord in RawRecordList)
			{
				if(ourRawRecord.Income.HasValue)
					ContinuousList.Add((double)ourRawRecord.Income);
			}
			AverageIncome = Average.AverageContinuous(ContinuousList);
			StandardDeviationIncome = StandardDeviation.StdDevContinuous(ContinuousList, AverageIncome);
			
			//Age
			ContinuousList.Clear();
			foreach(RawRecord ourRawRecord in RawRecordList)
			{
				if(ourRawRecord.Age.HasValue)
					ContinuousList.Add((double)ourRawRecord.Age);
			}
			AverageAge = Average.AverageContinuous(ContinuousList);
			StandardDeviationAge = StandardDeviation.StdDevContinuous(ContinuousList, AverageAge);
		}
		
		public static void RecordStats()
		{
			Console.WriteLine (">>>>AverageGender: {0}:{1}:{2}, \n>>>>StdDevGender {3}", 
			                   ZScorePreCalc.AverageGenderList[(int)GenderEnum.Female], ZScorePreCalc.AverageGenderList[(int)GenderEnum.Male], 
			                   ZScorePreCalc.AverageGenderList[(int)GenderEnum.Null], ZScorePreCalc.StandardDeviationGender);
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
			///*
			RawRecordOperator.AddRecord(ref RawRecordList, null, 58000, 53, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, "Female", null, 23, "No");
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 59000, null, "Yes");
			RawRecordOperator.AddRecord(ref RawRecordList, null, 39100, 32, "Yes");
			RawRecordOperator.AddRecord(ref RawRecordList, "Male", 69400, 55, null);
			//*/
		}
		
	}
}
