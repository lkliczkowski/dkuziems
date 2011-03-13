
using System;

namespace zscore
{
	public struct RawRecord
	{
		public uint Id { get; set; }
		public string Gender { get; set; }
		public double Income { get; set; }
		public byte Age { get; set; }
		public string Owner { get; set; }
		
		public RawRecord(uint Id, 
		                 string Gender, 
		                 double Income, 
		                 byte Age,
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
		
		public NormalizedRecord(uint Id,
		                        double IncomePresent)
		{
			this.Id = Id;
			
			this.IncomeMissing = 0;
			this.IncomePresent = IncomePresent;
			
			this.GenderMissing = this.GenderM = this.GenderF = 0;
			this.AgeMissing = this.AgePresent = 0;
			this.OwnerMissing = this.OwnerYes = this.OwnerNo = 0;
		}
	}
}
