using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    partial class ZScore
    {
        public static void Normalize(ref Records<float>[] normalized, 
            Records<float>[] discretized, EnumDataTypes dataType, int[] tabType)
        {
            for (int i = 0; i < normalized.Length; i++)
                normalized[i] = new Records<float>();
            int normalized_index = 0;

            switch(dataType)
            {
                case EnumDataTypes.HeartDisease:
                    Print("ZScore.Normalize", "case EnumDataTypes.HeartDisease");
                    float[] probability;
                    float val;
                    for (int i = 0; i < tabType.Length; i++)
                    {
                        switch (tabType[i])
                        {


                            case (int)EnumHeartDisease.Value:
                                float mean = discretized[i].RecordTable().Average();
                                float sigma = StdDevContinuous(discretized[i].RecordTable());

                                foreach (float cell in discretized[i].RecordTable())
                                {
                                    normalized[normalized_index].AddRecord(ZscoreContinuous(cell, mean, sigma));
                                    //Print(ZscoreContinuous(cell, mean, sigma));
                                }
                                normalized_index++;
                                break;


                            case (int)EnumHeartDisease.LowMediumHigh:
                                probability = ProbabilityDiscrete
                                    (discretized[i].RecordTable(), EnumHeartDisease.LowMediumHigh);
                                foreach (float cell in discretized[i].RecordTable())
                                {
                                    switch((int)cell)
                                    {
                                        case (int)EnumLowMediumHigh.low:
                                            val = ZScoreDiscrete(probability[(int)EnumLowMediumHigh.low], 
                                                StdDevDiscrete(probability[(int)EnumLowMediumHigh.low]));
                                            normalized[normalized_index + (int)EnumLowMediumHigh.low].
                                                AddRecord(val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.medium].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.high].
                                                AddRecord(-val);
                                            break;
                                        case (int)EnumLowMediumHigh.medium:
                                            val = ZScoreDiscrete(probability[(int)EnumLowMediumHigh.medium],
                                                StdDevDiscrete(probability[(int)EnumLowMediumHigh.medium]));
                                            normalized[normalized_index + (int)EnumLowMediumHigh.low].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.medium].
                                                AddRecord(val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.high].
                                                AddRecord(-val);
                                            break;
                                        case (int)EnumLowMediumHigh.high:
                                            val = ZScoreDiscrete(probability[(int)EnumLowMediumHigh.high],
                                                StdDevDiscrete(probability[(int)EnumLowMediumHigh.high]));
                                            normalized[normalized_index + (int)EnumLowMediumHigh.low].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.medium].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.high].
                                                AddRecord(val);
                                            break;
                                        case (int)EnumLowMediumHigh.unknown:
                                            Print("in Normalize.switch(dataType).case (int)EnumHeartDisease.LowMediumHigh", 
                                                "unknown EnumLowMediumHigh :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType).case (int)EnumHeartDisease.LowMediumHigh",
                                                "default EnumLowMediumHigh :: unhandled");
                                            break;
                                    }
                                    
                                }
                                normalized_index += Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1;
                                break;


                            case (int)EnumHeartDisease.AbsentPresent: //binary 1/0 -> YES/NO
                                foreach (float cell in discretized[i].RecordTable())
                                {
                                    if (cell == 1)
                                    {
                                        normalized[normalized_index].AddRecord(1);
                                        normalized[normalized_index + 1].AddRecord(0);
                                    }
                                    else
                                    {
                                        normalized[normalized_index].AddRecord(0);
                                        normalized[normalized_index + 1].AddRecord(1);
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumAbsentPresent)).Length - 1;
                                break;


                            case (int)EnumHeartDisease.Obesity:
                                probability = ProbabilityDiscrete
                                    (discretized[i].RecordTable(), EnumHeartDisease.Obesity);

                                foreach (float cell in discretized[i].RecordTable())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumObesity.Healthy:
                                            val = ZScoreDiscrete(probability[(int)EnumObesity.Healthy],
                                                StdDevDiscrete(probability[(int)EnumObesity.Healthy]));
                                            normalized[normalized_index + (int)EnumObesity.Healthy].
                                                AddRecord(val);
                                            normalized[normalized_index + (int)EnumObesity.overweight].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumObesity.clinicallyobese].
                                                AddRecord(-val);
                                            break;
                                        case (int)EnumObesity.overweight:
                                            val = ZScoreDiscrete(probability[(int)EnumObesity.overweight],
                                                StdDevDiscrete(probability[(int)EnumObesity.overweight]));
                                            normalized[normalized_index + (int)EnumObesity.Healthy].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumObesity.overweight].
                                                AddRecord(val);
                                            normalized[normalized_index + (int)EnumObesity.clinicallyobese].
                                                AddRecord(-val);
                                            break;
                                        case (int)EnumObesity.clinicallyobese:
                                            val = ZScoreDiscrete(probability[(int)EnumObesity.clinicallyobese],
                                                StdDevDiscrete(probability[(int)EnumObesity.clinicallyobese]));
                                            normalized[normalized_index + (int)EnumObesity.Healthy].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumObesity.overweight].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumObesity.clinicallyobese].
                                                AddRecord(val);
                                            break;
                                        case (int)EnumObesity.unknown:
                                            Print("in Normalize.switch(dataType).case (int)EnumHeartDisease.Obesity",
                                                "unknown EnumObesity :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType).case (int)EnumHeartDisease.Obesity",
                                                "default EnumObesity :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumObesity)).Length - 1;
                                break;


                            case (int)EnumHeartDisease.AgeRange:
                                probability = ProbabilityDiscrete
                                    (discretized[i].RecordTable(), EnumHeartDisease.AgeRange);

                                foreach (float cell in discretized[i].RecordTable())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumAgeRange.young:
                                            val = ZScoreDiscrete(probability[(int)EnumAgeRange.young],
                                                StdDevDiscrete(probability[(int)EnumAgeRange.young]));
                                            normalized[normalized_index + (int)EnumAgeRange.young].
                                                AddRecord(val);
                                            normalized[normalized_index + (int)EnumAgeRange.middleaged].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumAgeRange.old].
                                                AddRecord(-val);
                                            break;
                                        case (int)EnumAgeRange.middleaged:
                                            val = ZScoreDiscrete(probability[(int)EnumAgeRange.middleaged],
                                                StdDevDiscrete(probability[(int)EnumAgeRange.middleaged]));
                                            normalized[normalized_index + (int)EnumAgeRange.young].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumAgeRange.middleaged].
                                                AddRecord(val);
                                            normalized[normalized_index + (int)EnumAgeRange.old].
                                                AddRecord(-val);
                                            break;
                                        case (int)EnumAgeRange.old:
                                            val = ZScoreDiscrete(probability[(int)EnumAgeRange.old],
                                                StdDevDiscrete(probability[(int)EnumAgeRange.old]));
                                            normalized[normalized_index + (int)EnumAgeRange.young].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumAgeRange.middleaged].
                                                AddRecord(-val);
                                            normalized[normalized_index + (int)EnumAgeRange.old].
                                                AddRecord(val);
                                            break;
                                        case (int)EnumAgeRange.unknown:
                                            Print("in Normalize.switch(dataType).case (int)EnumHeartDisease.AgeRange",
                                                "unknown EnumAgeRange :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType).case (int)EnumHeartDisease.AgeRange",
                                                "default EnumAgeRange :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumAgeRange)).Length - 1;
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }

        }

        public static float ZscoreContinuous(float val, float mu, float sigma)
		{
			return ((val - mu)/sigma);
		}

        public static float ZScoreDiscrete(float mu, float sigma)
        {
            return ((1 - mu) / sigma);
        }
		
        public static void Print(string what, string where)
        {
            Console.WriteLine(">>>> {0}::{1}", what, where);
        }

    }
}
