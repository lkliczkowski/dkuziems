using System;
using System.Collections.Generic;

namespace zscore
{	
	class ZScore
	{
		public static void Main (string[] args)
		{
			List<RawRecord>RawRecordList = new List<RawRecord>();
			List<NormalizedRecord>NormalizedRecordList = new List<NormalizedRecord>();
			
			ZScorePreCalc.AddSomeRecords(ref RawRecordList);
			RawRecordOperator.DisplayRecords(RawRecordList);

			ZScorePreCalc.InitVariables(RawRecordList);
			ZScorePreCalc.RecordStats();

			Normalize(ref NormalizedRecordList, RawRecordList);
			NormalizedRecordOperator.DisplayRecords(NormalizedRecordList);
			
			//DiscreteType enumDiscrete = DiscreteType.Education;
			//Console.WriteLine(">>>> {0}", (int)enumDiscrete);
			
		}
		
		
		
		public static void Normalize(ref List<NormalizedRecord> NormalizedRecordList, List<RawRecord> RawRecordList)
		{
			
			double genderMissing = 0, genderM = 0, genderF = 0;
			double incomeMissing = 0, incomePresent = 0;
			double ageMissing = 0, agePresent = 0;
			foreach (RawRecord ourRawRecord in RawRecordList)
			{
				
				//Income - wartosc ciagla
				if( ourRawRecord.Income.HasValue == true )
				{
					incomeMissing = 0;
					incomePresent = ((double)ourRawRecord.Income - ZScorePreCalc.AverageIncome)/ZScorePreCalc.StandardDeviationIncome;
				}
				else
				{
					incomeMissing = -(ZScorePreCalc.AverageIncome/ZScorePreCalc.StandardDeviationIncome);
					incomePresent = 0;
				}
				
				//Age - wartosc ciagla
				if( ourRawRecord.Age.HasValue == true )
				{
					ageMissing = 0;
					agePresent = ((double)ourRawRecord.Age - ZScorePreCalc.AverageAge)/ZScorePreCalc.StandardDeviationAge;
				}
				else
				{
					ageMissing = -(ZScorePreCalc.AverageAge/ZScorePreCalc.StandardDeviationAge);
					agePresent = 0;
				}
				
				//Gender - wartosc dyskretna
				//Console.WriteLine(">>>> {0}", ourRawRecord.Gender);
				
				NormalizedRecordList.Add(new NormalizedRecord(ourRawRecord.Id, genderMissing, genderM, genderF, incomeMissing, incomePresent, ageMissing, agePresent));

			}
			
		}
	}
}
