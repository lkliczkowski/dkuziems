
using System;

namespace zscore
{
	public struct RawRecord
	{
		public uint Id { get; set; }
		public string Gender { get; set; }
		public double? Income { get; set; }
		public byte? Age { get; set; }
		public string Owner { get; set; }
		
		public RawRecord(uint Id, string Gender, 
		                 double? Income, byte? Age,
		                 string Owner)
		{
			this.Id = Id;
			this.Gender = Gender;
			this.Income = Income;
			this.Age = Age;
			this.Owner = Owner;			
		}	
	}
	
	public struct NormalizedRecord
	{
		//Id bez zmian (index)
		public uint Id { get; set; }
		
		//Gender jest wartoscia dyskretna
		public double GenderMissing { get; set; }
		public double GenderM { get; set; }
		public double GenderF { get; set; }
		
		//Income jest wartoscia ciagla
		public double IncomeMissing { get; set; }
		public double IncomePresent { get; set; }
		
		//Age jest ciagly, 0+ (byte 0-255)
		public double AgeMissing { get; set; }
		public double AgePresent  { get; set; }
		
		//Owner jest binary Miss/Yes/No 0/1/0
		public byte OwnerMissing { get; set; }
		public byte OwnerYes { get; set; }
		public byte OwnerNo { get; set; }
		
		public NormalizedRecord(uint Id, double GenderMissing,
		                        double GenderM, double GenderF,
		                        double IncomeMissing, double IncomePresent,
		                        double AgeMissing, double AgePresent,
		                        byte OwnerMissing, byte OwnerYes,
		                        byte OwnerNo)
		{
			this.Id = Id;
			this.GenderMissing = GenderMissing;
			this.GenderF = GenderF;
			this.GenderM = GenderM;
			this.IncomeMissing = IncomeMissing;
			this.IncomePresent = IncomePresent;
			this.AgeMissing = AgeMissing;
			this.AgePresent = AgePresent;
			this.OwnerMissing = OwnerMissing;
			this.OwnerYes = OwnerYes;
			this.OwnerNo = OwnerNo;
		}
		
		public NormalizedRecord(uint Id, double GenderMissing,
		                        double GenderM, double GenderF,
		                        double IncomeMissing, double IncomePresent,
		                        double AgeMissing, double AgePresent)
		: this (Id, GenderMissing, GenderM, GenderF, IncomeMissing, IncomePresent,
		        AgeMissing, AgePresent, 0, 0, 0)
		{}
		
		public NormalizedRecord(uint Id, double GenderMissing,
		                        double GenderM, double GenderF,
		                        double IncomeMissing, double IncomePresent)
		: this (Id, GenderMissing, GenderM, GenderF, IncomeMissing, IncomePresent, 0, 0)
		{}
		
		public NormalizedRecord(uint Id, double GenderMissing, 
		                        double GenderM, double GenderF)
		: this (Id, GenderMissing, GenderM, GenderF, 0, 0)
		{}
	}
}
