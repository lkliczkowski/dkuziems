using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    partial class ZScore
    {
        private static void normalize(ref Column<float>[] normalized, 
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
                    for (int i = 0; i < tabType.Length; i++)
                    {
                        switch (tabType[i])
                        {
                            case (int)EnumHeartDisease.Value:
                                float mean = discretized[i].ColumnToArray().Average();
                                float sigma = stdDevContinuous(discretized[i].ColumnToArray());

                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData(zScoreContinuous(cell, mean, sigma));
                                }

                                normalized_index++;
                                break;

                            case (int)EnumHeartDisease.LowMediumHigh:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.LowMediumHigh);
                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    switch((int)cell)
                                    {
                                        case (int)EnumLowMediumHigh.low:
                                            addNormalizedLMH(ref normalized, probability, 
                                                EnumLowMediumHigh.low, normalized_index);
                                            break;
                                        case (int)EnumLowMediumHigh.medium:
                                            addNormalizedLMH(ref normalized, probability, 
                                                EnumLowMediumHigh.medium, normalized_index);
                                            break;
                                        case (int)EnumLowMediumHigh.high:
                                            addNormalizedLMH(ref normalized, probability, 
                                                EnumLowMediumHigh.high, normalized_index);
                                            break;

                                        case (int)EnumLowMediumHigh.unknown:
                                            Print("in Normalize.switch(dataType)", 
                                                "unknown EnumLowMediumHigh :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumLowMediumHigh :: unhandled");
                                            break;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1;
                                break;

                            case (int)EnumHeartDisease.AbsentPresent: //binary 1/0 -> YES/NO
                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData((cell == 1?1:0));
                                    normalized[normalized_index + 1].AddData((cell == 1?0:1));
                                }
                                normalized_index += Enum.GetValues(typeof(EnumAbsentPresent)).Length - 1;
                                break;

                            case (int)EnumHeartDisease.Obesity:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.Obesity);

                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumObesity.underweight:
                                            addNormalizedObesity(ref normalized, 
                                                probability, EnumObesity.underweight, normalized_index);
                                            break;
                                        case (int)EnumObesity.Healthy:
                                            addNormalizedObesity(ref normalized, 
                                                probability, EnumObesity.Healthy, normalized_index);
                                            break;
                                        case (int)EnumObesity.overweight:
                                            addNormalizedObesity(ref normalized, 
                                                probability, EnumObesity.overweight, normalized_index);
                                            break;
                                        case (int)EnumObesity.clinicallyobese:
                                            addNormalizedObesity(ref normalized, 
                                                probability, EnumObesity.clinicallyobese, normalized_index);
                                            break;
                                        case (int)EnumObesity.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumObesity :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumObesity :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumObesity)).Length - 1;
                                break;

                            case (int)EnumHeartDisease.AgeRange:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.AgeRange);

                                foreach (float cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumAgeRange.young:
                                            addNormalizedAgeRange(ref normalized, probability, EnumAgeRange.young, normalized_index);
                                            break;
                                        case (int)EnumAgeRange.middleaged:
                                            addNormalizedAgeRange(ref normalized, probability, EnumAgeRange.middleaged, normalized_index);
                                            break;
                                        case (int)EnumAgeRange.old:
                                            addNormalizedAgeRange(ref normalized, probability, EnumAgeRange.old, normalized_index);
                                            break;
                                        case (int)EnumAgeRange.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumAgeRange :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
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


        private static void addNormalizedLMH(ref Column<float>[] normalizedTable, 
            float[] probabilityList, EnumLowMediumHigh ourCase, int normalized_index)
        {
            float val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalizedObesity(ref Column<float>[] normalizedTable,
            float[] probabilityList, EnumObesity ourCase, int normalized_index)
        {
            float val;
            val = zScoreDiscrete(probabilityList[(int)ourCase], 
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumObesity)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase?val:(-val)));
            }
        }

        private static void addNormalizedAgeRange(ref Column<float>[] normalizedTable,
            float[] probabilityList, EnumAgeRange ourCase, int normalized_index)
        {
            float val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumAgeRange)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }
    }
}
