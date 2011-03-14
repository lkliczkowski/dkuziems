using System;
using System.Collections.Generic;

namespace zscore
{
	#region RawRecordOperator static class
	public static class RawRecordOperator
	{
		private static uint counter = 0;
		public static uint Counter { get { return counter; } }
		private static void incCounter() { counter++; }
		
		public static void AddRecord(ref List<RawRecord> RawRecordList,
		                             string gender, 
					                 double income, 
					                 byte age,
					                 string owner)
		{
			incCounter();
			RawRecordList.Add(new RawRecord(Counter, gender, income, age, owner));
		}
		
		public static void DisplayRecords(List<RawRecord> RawRecordList)
		{
			Console.WriteLine("ID\tGender\tIncome\tAge\tOwner");
			foreach (RawRecord ourRawRecord in RawRecordList)
			{
				Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", 
				                  ourRawRecord.Id,
				                  ourRawRecord.Gender,
				                  ourRawRecord.Income,
				                  ourRawRecord.Age,
				                  ourRawRecord.Owner);
			}
			Console.WriteLine();
		}
		
	}
	#endregion
	
	#region NormalizedRecordOperator
	public static class NormalizedRecordOperator
	{
		public static void DisplayRecords(List<NormalizedRecord> NormalizedRecordList)
		{
			Console.WriteLine("ID \t\tGender \t\t\t  Income \t\t\t  Age \t\t  Owner");
			Console.WriteLine(" \t -\tM\tF \t\t\t -\t val \t\t\t-\t val \t\t-\tY\tN");
			foreach (NormalizedRecord ourNormalizedRecord in NormalizedRecordList)
			{
				Console.WriteLine("{0}\t {1}\t{2:N2}\t{3:N2} \t\t{4}\t{5:N2} \t\t{6}\t{7} \t\t{8}\t{9}\t{10}", 
				                  ourNormalizedRecord.Id,
				                  ourNormalizedRecord.GenderMissing,
				                  ourNormalizedRecord.GenderM,
				                  ourNormalizedRecord.GenderF,
				                  ourNormalizedRecord.IncomeMissing,
				                  ourNormalizedRecord.IncomePresent,
				                  ourNormalizedRecord.AgeMissing,
				                  ourNormalizedRecord.AgePresent,
				                  ourNormalizedRecord.OwnerMissing,
				                  ourNormalizedRecord.OwnerYes,
				                  ourNormalizedRecord.OwnerNo
				                  );
			}
		}
	}
	#endregion
	
	#region Average static class
	public static class Average
	{
		public static double Income(List<RawRecord> RawRecordList)
		{
			double average = 0;
			foreach(RawRecord ourRawRecord in RawRecordList)
			{
				average += ourRawRecord.Income;
			}
			
			return average/(double)RawRecordOperator.Counter;
		}
	}
	#endregion
	
	public static class StandardDeviation
	{
		
		/**@param average argument average przesyłamy jako obliczona wartosc 
		 * aby nie obliczac tej wartosci wielokrotnie, np. dla
		 * bazy 50.000 rekordow...
		 */
		public static double Income(List<RawRecord> RawRecordList, double average)
		{
			double sumOfDerivation = 0;
			foreach(RawRecord ourRawRecord in RawRecordList)
			{
				sumOfDerivation += ourRawRecord.Income * ourRawRecord.Income;	
			}
			
			sumOfDerivation = sumOfDerivation / RawRecordOperator.Counter;
			return Math.Sqrt(sumOfDerivation - (average*average));
		}
		
	}
	
	public static class ZScoreRawRecordListTests
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
		}
	}
	
	class MainClass
	{
		public static void Main (string[] args)
		{
			List<RawRecord>RawRecordList = new List<RawRecord>();
			List<NormalizedRecord>NormalizedRecordList = new List<NormalizedRecord>();
			
			ZScoreRawRecordListTests.AddSomeRecords(ref RawRecordList);
			ZScoreRawRecordListTests.InitVariables(RawRecordList);
			
			RawRecordOperator.DisplayRecords(RawRecordList);
			//Console.WriteLine (">>>>Average: {0}", ZScoreRawRecordListTests.AverageIncome);
			//Console.WriteLine (">>>>StdDev{0}", ZScoreRawRecordListTests.StandardDeviationIncome);
			Normalize(ref NormalizedRecordList, RawRecordList);
			NormalizedRecordOperator.DisplayRecords(NormalizedRecordList);
		}
		
		public static void Normalize(ref List<NormalizedRecord> NormalizedRecordList, List<RawRecord> RawRecordList)
		{
			double incomeN;
			foreach (RawRecord ourRawRecord in RawRecordList)
			{
				incomeN = (ourRawRecord.Income - ZScoreRawRecordListTests.AverageIncome)/ZScoreRawRecordListTests.StandardDeviationIncome;
				//dodajemy do listy strukture (id, normalized-income)
				NormalizedRecordList.Add(new NormalizedRecord(ourRawRecord.Id, incomeN));				
			}
		}
	}
}
