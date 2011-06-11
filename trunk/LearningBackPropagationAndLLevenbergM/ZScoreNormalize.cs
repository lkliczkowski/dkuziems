using System;
using System.Linq;

namespace ZScore
{
    partial class ZScore
    {
        private static void normalize(ref Column<double>[] normalized,
            Column<double>[] discretized, EnumDataTypes dataType, int[] tabType)
        {
            for (int i = 0; i < normalized.Length; i++)
                normalized[i] = new Column<double>();
            int normalized_index = 0;
            double[] probability;

            switch (dataType)
            {
                #region heart disease
                case EnumDataTypes.HeartDisease:
                    Print("ZScore.Normalize", "case EnumDataTypes.HeartDisease");
                    for (int i = 0; i < tabType.Length; i++)
                    {
                        switch (tabType[i])
                        {
                            case (int)EnumHeartDisease.Value:
                                double mean = discretized[i].ColumnToArray().Average();
                                double sigma = stdDevContinuous(discretized[i].ColumnToArray());

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData(zScoreContinuous(cell, mean, sigma));
                                }

                                normalized_index++;
                                break;

                            case (int)EnumHeartDisease.LowMediumHigh:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.LowMediumHigh);
                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumLowMediumHigh)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumLowMediumHigh :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1;
                                break;

                            case (int)EnumHeartDisease.AbsentPresent: //binary 1/0 -> YES/NO
                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData((cell == 1 ? 1 : 0));
                                    normalized[normalized_index + 1].AddData((cell == 1 ? 0 : 1));
                                }
                                normalized_index += Enum.GetValues(typeof(EnumAbsentPresent)).Length - 1;
                                break;

                            case (int)EnumHeartDisease.Obesity:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.Obesity);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumObesity)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumObesity :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumObesity)).Length - 1;
                                break;

                            case (int)EnumHeartDisease.AgeRange:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumHeartDisease.AgeRange);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumAgeRange)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumAgeRange :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumAgeRange)).Length - 1;
                                break;
                        }
                    }
                    break;
                #endregion
                #region GermanCreditData
                case EnumDataTypes.GermanCreditData:
                    Print("ZScore.Normalize", "case EnumDataTypes.GermanCreditData");
                    for (int i = 0; i < tabType.Length; i++)
                    {
                        switch (tabType[i])
                        {
                            case (int)EnumGermanCreditData.Numerical:
                                double mean = discretized[i].ColumnToArray().Average();
                                double sigma = stdDevContinuous(discretized[i].ColumnToArray());

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData(zScoreContinuous(cell, mean, sigma));
                                }

                                normalized_index++;
                                break;

                            case (int)EnumGermanCreditData.A19: //binary 1/0 -> YES/NO
                            case (int)EnumGermanCreditData.A20:
                            case (int)EnumGermanCreditData.Classification: 
                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData((cell == 1 ? 1 : 0));
                                    normalized[normalized_index + 1].AddData((cell == 1 ? 0 : 1));
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA20)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A1:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A1);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA1)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA1 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA1)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A3:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A3);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA3)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA3 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA3)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A4:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A4);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA4)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA4 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA4)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A6:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A6);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA6)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA6 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA6)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A7:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A7);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA7)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA7 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA7)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A9:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A9);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA9)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA9 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA9)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A10:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A10);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA10)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA10 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA10)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A12:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A12);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA12)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA12 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA12)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A14:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A14);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA14)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA14 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA14)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A15:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A15);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA15)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA15 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA15)).Length - 1;
                                break;

                            case (int)EnumGermanCreditData.A17:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumGermanCreditData.A17);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability,
                                                (EnumA17)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumA17 :: unhandled"); ;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumA17)).Length - 1;
                                break;

                        }
                    }
                    break;
                #endregion
                #region letterRecognitionA

                case EnumDataTypes.LetterRecognitionA:
                    for (int i = 0; i < tabType.Length; i++)
                    {
                        switch (tabType[i])
                        {
                            case 1:
                                double mean = discretized[i].ColumnToArray().Average();
                                double sigma = stdDevContinuous(discretized[i].ColumnToArray());

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData(zScoreContinuous(cell, mean, sigma));
                                    //normalized[normalized_index].AddData(cell); //oryginalna wartość
                                }

                                normalized_index++;
                                break;
                            case 0:
                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData(cell);
                                }
                                normalized_index++;
                                break;
                        }
                    }
                    break;
                #endregion
                #region creditRisk
                case EnumDataTypes.CreditRisk:
                    Print("ZScore.Normalize", "case EnumDataTypes.CreditRisk");
                    for (int i = 0; i < tabType.Length; i++)
                    {
                        switch (tabType[i])
                        {
                            case (int)EnumCreditRisk.MonthsAcct: //continuous
                            case (int)EnumCreditRisk.ResidenceTime:
                            case (int)EnumCreditRisk.Age:
                                double mean = discretized[i].ColumnToArray().Average();
                                double sigma = stdDevContinuous(discretized[i].ColumnToArray());

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData(zScoreContinuous(cell, mean, sigma));
                                }

                                normalized_index++;
                                break;


                            case (int)EnumCreditRisk.Telephone: //binary 1/0 -> YES/NO
                            case (int)EnumCreditRisk.Foreign:
                            case (int)EnumCreditRisk.CreditStanding:
                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    normalized[normalized_index].AddData((cell == 1 ? 1 : 0));
                                    normalized[normalized_index + 1].AddData((cell == 1 ? 0 : 1));
                                }
                                normalized_index += Enum.GetValues(typeof(EnumYesNo)).Length - 1;
                                break;


                            case (int)EnumCreditRisk.CheckingAcct:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.CheckingAcct);
                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumCheckingAcct)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)", "unknown EnumCheckingAcct :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumCheckingAcct)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.CreditHist:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.CreditHist);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumCreditHist)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)", "unknown EnumCreditHist :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumCreditHist)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.Purpose:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Purpose);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumPurpose)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)", "unknown EnumPurpose :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumPurpose)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.SavingsAcct:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.SavingsAcct);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumSavingsAcct)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)", "unknown EnumSavingsAcct :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumSavingsAcct)).Length - 1;
                                break;


                            case (int)EnumCreditRisk.Employment:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Employment);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumEmployment)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)", "unknown EnumEmployment :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumEmployment)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.Gender:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Gender);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumGender)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)", "default EnumGender :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumGender)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.PersonalStatus:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.PersonalStatus);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumPersonalStatus)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "default EnumPersonalStatus :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumPersonalStatus)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.Housing:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Housing);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumHousing)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "default EnumHousing :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumHousing)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.Job:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Job);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    try
                                    {
                                        addNormalized(ref normalized, probability, (EnumJob)((int)cell), normalized_index);
                                    }
                                    catch
                                    {
                                        Print("in Normalize.switch(dataType)",
                                                "unknown EnumJob :: unhandled");
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumJob)).Length - 1;
                                break;

                        }
                    }
                    break;
                #endregion
                default:
                    break;
            }

        }

        private static double zScoreContinuous(double val, double mu, double sigma)
        {
            return ((val - mu) / sigma);
        }

        private static double zScoreDiscrete(double mu, double sigma)
        {
            return ((1 - mu) / sigma);
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumLowMediumHigh ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumObesity ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumObesity)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumAgeRange ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumAgeRange)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA1 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA1)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA3 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA3)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA4 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA4)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA6 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA6)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA7 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA7)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA9 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA9)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA10 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA10)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA12 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA12)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA14 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA14)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA15 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA15)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA17 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA17)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


/*        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA19 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA19)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }


        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumA20 ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumA20)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }
*/
        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumCheckingAcct ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumCheckingAcct)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumCreditHist ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumCreditHist)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumPurpose ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumPurpose)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumSavingsAcct ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumSavingsAcct)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumEmployment ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumEmployment)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumGender ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumGender)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumPersonalStatus ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumPersonalStatus)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumHousing ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumHousing)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

        private static void addNormalized(ref Column<double>[] normalizedTable,
            double[] probabilityList, EnumJob ourCase, int normalized_index)
        {
            double val;
            val = zScoreDiscrete(probabilityList[(int)ourCase],
                stdDevDiscrete(probabilityList[(int)ourCase]));

            for (int i = 0; i < Enum.GetValues(typeof(EnumJob)).Length - 1; i++)
            {
                normalizedTable[normalized_index + i].
                    AddData((i == (int)ourCase ? val : (-val)));
            }
        }

    }
}
