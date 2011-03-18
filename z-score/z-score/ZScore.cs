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
			
			
			}
		
		
		
		public static void Normalize(ref List<NormalizedRecord> NormalizedRecordList, List<RawRecord> RawRecordList)
		{
			
			double genderMissing = 0, genderM = 0, genderF = 0;
			double incomeMissing = 0, incomePresent = 0;
			double ageMissing = 0, agePresent = 0;
			byte ownerMissing = 0, ownerYes = 0, ownerNo = 0;
			GenderEnum genderOption;
			OwnerEnum ownerOption;
			
			foreach (RawRecord MyRecord in RawRecordList)
			{
				
				//Gender - wartosc dyskretna
				genderOption = (MyRecord.Gender == null)?GenderEnum.Null:
					(GenderEnum)Enum.Parse(typeof(GenderEnum), MyRecord.Gender);
				
				switch(genderOption)
					{
					case GenderEnum.Female:
						genderF = ZScoreDiscrete(true, ZScorePreCalc.AverageGenderList[(int)GenderEnum.Female], 
							ZScorePreCalc.StandardDeviationGenderList[(int)GenderEnum.Female]);
						genderM = -genderF;
						genderMissing = 0;					
						break;
					case GenderEnum.Male:
						genderM = ZScoreDiscrete(true, ZScorePreCalc.AverageGenderList[(int)GenderEnum.Male], 
							ZScorePreCalc.StandardDeviationGenderList[(int)GenderEnum.Male]);
						genderF = -genderM;
						genderMissing = 0;
						break;
					case GenderEnum.Null:
						genderMissing = ZScoreDiscrete(false, ZScorePreCalc.AverageGenderList[(int)GenderEnum.Null], 
							ZScorePreCalc.StandardDeviationGenderList[(int)GenderEnum.Null]);
						genderF = genderM = -genderMissing;
						break;
					default:
						break;
					}
				
				//Income - wartosc ciagla
				if( MyRecord.Income.HasValue == true )
				{
					incomeMissing = 0;
					incomePresent = ZscoreContinuous((double)MyRecord.Income, ZScorePreCalc.AverageIncome, 
					                                 ZScorePreCalc.StandardDeviationIncome);
				}
				else
				{
					incomeMissing = ZscoreContinuous(0, ZScorePreCalc.AverageIncome, ZScorePreCalc.StandardDeviationIncome);
					incomePresent = 0;
				}
				
				//Age - wartosc ciagla
				if( MyRecord.Age.HasValue == true )
				{
					ageMissing = 0;
					agePresent = ZscoreContinuous((double)MyRecord.Age, ZScorePreCalc.AverageAge, 
					                              ZScorePreCalc.StandardDeviationAge);
				}
				else
				{
					ageMissing = ZscoreContinuous(0, ZScorePreCalc.AverageAge, ZScorePreCalc.StandardDeviationAge);
					agePresent = 0;
				}
				
				//Owner - binary 0/1/missing
				if( MyRecord.Owner != null )
				{
					try
					{
						ownerOption = (OwnerEnum)Enum.Parse(typeof(OwnerEnum), MyRecord.Owner);
						switch(ownerOption)
						{
						case OwnerEnum.Yes:
							ownerYes = (byte)OwnerEnum.Yes;
							ownerNo = (byte)OwnerEnum.No;
							ownerMissing = (byte)OwnerEnum.No;
							break;
						case OwnerEnum.No:
							ownerYes = (byte)OwnerEnum.No;
							ownerNo = (byte)OwnerEnum.Yes;
							ownerMissing = (byte)OwnerEnum.No;
							break;
						default:
							break;
						}
					}
					catch
					{
						Console.WriteLine(">>Failed while parsing OwnerEnum");
						ownerMissing = ownerYes = ownerNo = 2;
					}
				}
				else
				{
					ownerYes = (byte)OwnerEnum.No;
					ownerNo = (byte)OwnerEnum.No;
					ownerMissing = (byte)OwnerEnum.Yes;
				}
				
				
				NormalizedRecordList.Add(new NormalizedRecord(MyRecord.Id, genderMissing, 
				                                              genderM, genderF, incomeMissing, 
				                                              incomePresent, ageMissing, agePresent,
				                                              ownerMissing, ownerYes, ownerNo));

			}
			
		}
		
		public static double ZscoreContinuous(double val, double mu, double sigma)
		{
			return ((val - mu)/sigma);
		}
		
		public static double ZScoreDiscrete(bool val_present, double mu, double sigma)
		{
			return val_present?
				((1-mu)/sigma):
					(-mu/sigma);
		}
		
		
	}
}
