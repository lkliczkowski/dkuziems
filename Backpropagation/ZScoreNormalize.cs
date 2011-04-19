using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    partial class ZScore
    {
        private static void Normalize(ref Column<float>[] normalized, 
            Column<float>[] discretized, EnumDataTypes dataType, int[] tabType)
        {
            for (int i = 0; i < normalized.Length; i++)
                normalized[i] = new Column<float>();
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
                                float mean = discretized[i].ColumnToArray().Average();
                                float sigma = StdDevContinuous(discretized[i].ColumnToArray());

                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData(zScoreContinuous(cell, mean, sigma));
                                    //Print(ZscoreContinuous(cell, mean, sigma));
                                }
                                normalized_index++;
                                break;


                            case (int)EnumHeartDisease.LowMediumHigh:
                                probability = ProbabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.LowMediumHigh);
                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    switch((int)cell)
                                    {
                                        case (int)EnumLowMediumHigh.low:
                                            addNormalizedLMH(ref normalized, probability, EnumLowMediumHigh.low, normalized_index);
                                            break;
                                        case (int)EnumLowMediumHigh.medium:
                                            addNormalizedLMH(ref normalized, probability, EnumLowMediumHigh.medium, normalized_index);
                                            break;
                                        case (int)EnumLowMediumHigh.high:
                                            addNormalizedLMH(ref normalized, probability, EnumLowMediumHigh.high, normalized_index);
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
                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    if (cell == 1)
                                    {
                                        normalized[normalized_index].AddData(1);
                                        normalized[normalized_index + 1].AddData(0);
                                    }
                                    else
                                    {
                                        normalized[normalized_index].AddData(0);
                                        normalized[normalized_index + 1].AddData(1);
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumAbsentPresent)).Length - 1;
                                break;


                            case (int)EnumHeartDisease.Obesity:
                                probability = ProbabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.Obesity);

                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumObesity.underweight:
                                            val = zScoreDiscrete(probability[(int)EnumObesity.underweight],
                                                StdDevDiscrete(probability[(int)EnumObesity.underweight]));
                                            normalized[normalized_index + (int)EnumObesity.underweight].
                                                AddData(val);
                                            normalized[normalized_index + (int)EnumObesity.Healthy].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.overweight].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.clinicallyobese].
                                                AddData(-val);
                                            break;
                                        case (int)EnumObesity.Healthy:
                                            val = zScoreDiscrete(probability[(int)EnumObesity.Healthy],
                                                StdDevDiscrete(probability[(int)EnumObesity.Healthy]));
                                            normalized[normalized_index + (int)EnumObesity.underweight].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.Healthy].
                                                AddData(val);
                                            normalized[normalized_index + (int)EnumObesity.overweight].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.clinicallyobese].
                                                AddData(-val);
                                            break;
                                        case (int)EnumObesity.overweight:
                                            val = zScoreDiscrete(probability[(int)EnumObesity.overweight],
                                                StdDevDiscrete(probability[(int)EnumObesity.overweight]));
                                            normalized[normalized_index + (int)EnumObesity.underweight].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.Healthy].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.overweight].
                                                AddData(val);
                                            normalized[normalized_index + (int)EnumObesity.clinicallyobese].
                                                AddData(-val);
                                            break;
                                        case (int)EnumObesity.clinicallyobese:
                                            val = zScoreDiscrete(probability[(int)EnumObesity.clinicallyobese],
                                                StdDevDiscrete(probability[(int)EnumObesity.clinicallyobese]));
                                            normalized[normalized_index + (int)EnumObesity.underweight].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.Healthy].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.overweight].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumObesity.clinicallyobese].
                                                AddData(val);
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
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.AgeRange);

                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumAgeRange.young:
                                            val = zScoreDiscrete(probability[(int)EnumAgeRange.young],
                                                StdDevDiscrete(probability[(int)EnumAgeRange.young]));
                                            normalized[normalized_index + (int)EnumAgeRange.young].
                                                AddData(val);
                                            normalized[normalized_index + (int)EnumAgeRange.middleaged].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumAgeRange.old].
                                                AddData(-val);
                                            break;
                                        case (int)EnumAgeRange.middleaged:
                                            val = zScoreDiscrete(probability[(int)EnumAgeRange.middleaged],
                                                StdDevDiscrete(probability[(int)EnumAgeRange.middleaged]));
                                            normalized[normalized_index + (int)EnumAgeRange.young].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumAgeRange.middleaged].
                                                AddData(val);
                                            normalized[normalized_index + (int)EnumAgeRange.old].
                                                AddData(-val);
                                            break;
                                        case (int)EnumAgeRange.old:
                                            val = zScoreDiscrete(probability[(int)EnumAgeRange.old],
                                                StdDevDiscrete(probability[(int)EnumAgeRange.old]));
                                            normalized[normalized_index + (int)EnumAgeRange.young].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumAgeRange.middleaged].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumAgeRange.old].
                                                AddData(val);
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

        private static float zScoreContinuous(float val, float mu, float sigma)
		{
			return ((val - mu)/sigma);
		}

        private static float zScoreDiscrete(float mu, float sigma)
        {
            return ((1 - mu) / sigma);
        }

        /*
         *                               case (int)EnumLowMediumHigh.low:
                                            val = ZScoreDiscrete(probability[(int)EnumLowMediumHigh.low], 
                                                StdDevDiscrete(probability[(int)EnumLowMediumHigh.low]));
                                            normalized[normalized_index + (int)EnumLowMediumHigh.low].
                                                AddData(val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.medium].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.high].
                                                AddData(-val);
                                            break;
                                        case (int)EnumLowMediumHigh.medium:
                                            val = ZScoreDiscrete(probability[(int)EnumLowMediumHigh.medium],
                                                StdDevDiscrete(probability[(int)EnumLowMediumHigh.medium]));
                                            normalized[normalized_index + (int)EnumLowMediumHigh.low].
                                                AddData(-val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.medium].
                                                AddData(val);
                                            normalized[normalized_index + (int)EnumLowMediumHigh.high].
                                                AddData(-val);
                                            break;
         */

        private static void addNormalizedLMH(ref Column<float>[] normalizedTable, 
            float[] probabilityList, EnumLowMediumHigh ourCase, int normalized_index)
        {
            float val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                StdDevDiscrete(probabilityList[(int)ourCase]));
            for (int i = 0; i < Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1; i++)
            {
                if(i == (int)ourCase)
                    normalizedTable[normalized_index + i].AddData(val);
                else
                    normalizedTable[normalized_index + i].AddData(-val);

            }

        }
    }
}
