
using System;
using System.Collections.Generic;

namespace zscore
{
	public static class ZScoreDiscretize
	{
		
		public static List<double>Discretize
			(List<string>MyRecordList, DiscreteType discreteType)
		{
			List<double>DiscretizedList = new List<double>();
			GenderEnum genderOption;
			
			switch(discreteType)
			{
			case DiscreteType.Unknown:
				Console.WriteLine(">>>> discreteType::unknown");
				break;
			case DiscreteType.Gender:
				Console.WriteLine(">>>> discreteType::gender");
				foreach(string MyRecord in MyRecordList)
				{
					genderOption = (GenderEnum)Enum.Parse(typeof(GenderEnum), MyRecord);
					switch(genderOption)
					{
					case GenderEnum.Null:
						DiscretizedList.Add((double)GenderEnum.Null);
						break;
					case GenderEnum.Female:
						DiscretizedList.Add((double)GenderEnum.Female);
						break;
					case GenderEnum.Male:
						DiscretizedList.Add((double)GenderEnum.Male);
						break;
					default:
						break;
					}
				}
					
				break;
			case DiscreteType.Education:
				Console.WriteLine(">>>> discreteType::education");
				break;
			default:
				Console.WriteLine(">>>> discreteType::default");
				break;
			}
			
			return DiscretizedList;
		}
		
	}
}
