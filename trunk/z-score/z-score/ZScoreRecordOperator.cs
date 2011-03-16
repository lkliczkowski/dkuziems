
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
					                 byte? age,
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
				                  (ourRawRecord.Gender == null)?"null\t":ourRawRecord.Gender);
				
				//Console.Write("\t");

				if(ourRawRecord.Income == null)
					Console.Write("{0}\t", nullable);
				else
					Console.Write("{0}",ourRawRecord.Income);
				
				Console.Write("\t");
				
				if(ourRawRecord.Age == null)
					Console.Write("{0}", nullable);
				else
					Console.Write("{0}",ourRawRecord.Age);
				
				Console.WriteLine("\t{0}", ourRawRecord.Owner);
				
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
			Console.WriteLine("ID \t\tGender \t\t\t\t  Income \t\t\t\t  Age \t\t\t  Owner");
			Console.WriteLine(" \t -\tM\tF \t\t\t -\t\t val \t\t\t-\t\t val \t\t\t-\tY\tN");
			foreach (NormalizedRecord ourNormalizedRecord in NormalizedRecordList)
			{
				Console.WriteLine("{0}\t {1}\t{2:N2}\t{3:N2} \t\t{4:N1}\t\t{5:N2} " +
					"\t\t{6:N1}\t\t{7:N2} \t\t{8}\t{9}\t{10}", 
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


}
