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
					                 double? income, 
					                 byte age,
					                 string owner)
		{
			incCounter();
			RawRecordList.Add(new RawRecord(Counter, gender, income, age, owner));
		}
		
		public static void DisplayRecords(List<RawRecord> RawRecordList)
		{
			string nullable = "null";
			Console.WriteLine("ID\tGender\tIncome\tAge\tOwner");
			foreach (RawRecord ourRawRecord in RawRecordList)
			{
				Console.Write("{0}\t{1}\t", ourRawRecord.Id,
				                  ourRawRecord.Gender==null?"null\t":ourRawRecord.Gender);
				
				if(ourRawRecord.Income == null)
					Console.Write("{0}\t", nullable);
				else
					Console.Write("{0}",ourRawRecord.Income);
				
				Console.WriteLine("\t{0}\t{1}", ourRawRecord.Age,
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
			Console.WriteLine("ID \t\tGender \t\t\t\t  Income \t\t\t  Age \t\t  Owner");
			Console.WriteLine(" \t -\tM\tF \t\t\t -\t\t val \t\t\t-\t val \t\t-\tY\tN");
			foreach (NormalizedRecord ourNormalizedRecord in NormalizedRecordList)
			{
				Console.WriteLine("{0}\t {1}\t{2:N2}\t{3:N2} \t\t{4:N1}\t\t{5:N2} \t\t{6}\t{7} \t\t{8}\t{9}\t{10}", 
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
	
	
	class ZScore
	{
		public static void Main (string[] args)
		{
			List<RawRecord>RawRecordList = new List<RawRecord>();
			List<NormalizedRecord>NormalizedRecordList = new List<NormalizedRecord>();
			
			ZScorePreCalc.AddSomeRecords(ref RawRecordList);
			ZScorePreCalc.InitVariables(RawRecordList);
			
			RawRecordOperator.DisplayRecords(RawRecordList);
			ZScorePreCalc.RecordStats();

			Normalize(ref NormalizedRecordList, RawRecordList);
			NormalizedRecordOperator.DisplayRecords(NormalizedRecordList);
		}
		
		public static void Normalize(ref List<NormalizedRecord> NormalizedRecordList, List<RawRecord> RawRecordList)
		{
			double incomePresent, incomeMissing;
			foreach (RawRecord ourRawRecord in RawRecordList)
			{
				//incomeN = (ourRawRecord.Income - ZScorePreCalc.AverageIncome)/ZScorePreCalc.StandardDeviationIncome;
				//incomeN = ourRawRecord.Income==null?1:(ourRawRecord.Income - ZScorePreCalc.AverageIncome)/ZScorePreCalc.StandardDeviationIncome;
				//dodajemy do listy strukture (id, normalized-income)
				//NormalizedRecordList.Add(new NormalizedRecord(ourRawRecord.Id, incomeN));
				if( ourRawRecord.Income.HasValue == true )
				{
					incomePresent = ((double)ourRawRecord.Income - ZScorePreCalc.AverageIncome)/ZScorePreCalc.StandardDeviationIncome;
					//dodajemy do listy strukture (id, normalized-missing-income(0),normalized-income)
					NormalizedRecordList.Add(new NormalizedRecord(ourRawRecord.Id, 0, incomePresent));
				}
				else
				{
					incomeMissing = -(ZScorePreCalc.AverageIncome/ZScorePreCalc.StandardDeviationIncome);
					NormalizedRecordList.Add(new NormalizedRecord(ourRawRecord.Id, incomeMissing, 0));
				}
			}
			
		}
	}
}
