using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                                    switch ((int)cell)
                                    {
                                        case (int)EnumLowMediumHigh.low:
                                            addNormalized(ref normalized, probability,
                                                EnumLowMediumHigh.low, normalized_index);
                                            break;
                                        case (int)EnumLowMediumHigh.medium:
                                            addNormalized(ref normalized, probability,
                                                EnumLowMediumHigh.medium, normalized_index);
                                            break;
                                        case (int)EnumLowMediumHigh.high:
                                            addNormalized(ref normalized, probability,
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
                                    switch ((int)cell)
                                    {
                                        case (int)EnumObesity.underweight:
                                            addNormalized(ref normalized,
                                                probability, EnumObesity.underweight, normalized_index);
                                            break;
                                        case (int)EnumObesity.Healthy:
                                            addNormalized(ref normalized,
                                                probability, EnumObesity.Healthy, normalized_index);
                                            break;
                                        case (int)EnumObesity.overweight:
                                            addNormalized(ref normalized,
                                                probability, EnumObesity.overweight, normalized_index);
                                            break;
                                        case (int)EnumObesity.clinicallyobese:
                                            addNormalized(ref normalized,
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

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumAgeRange.young:
                                            addNormalized(ref normalized, probability, EnumAgeRange.young, normalized_index);
                                            break;
                                        case (int)EnumAgeRange.middleaged:
                                            addNormalized(ref normalized, probability, EnumAgeRange.middleaged, normalized_index);
                                            break;
                                        case (int)EnumAgeRange.old:
                                            addNormalized(ref normalized, probability, EnumAgeRange.old, normalized_index);
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
                                    //normalized[normalized_index + 1].AddData((cell == 1?0:1));
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
                                    switch ((int)cell)
                                    {
                                        case (int)EnumCheckingAcct.Low:
                                            addNormalized(ref normalized, probability,
                                                EnumCheckingAcct.Low, normalized_index);
                                            break;
                                        case (int)EnumCheckingAcct.NoAcct:
                                            addNormalized(ref normalized, probability,
                                                EnumCheckingAcct.NoAcct, normalized_index);
                                            break;
                                        case (int)EnumCheckingAcct.Balance:
                                            addNormalized(ref normalized, probability,
                                                EnumCheckingAcct.Balance, normalized_index);
                                            break;
                                        case (int)EnumCheckingAcct.High:
                                            addNormalized(ref normalized, probability,
                                                EnumCheckingAcct.High, normalized_index);
                                            break;

                                        case (int)EnumCheckingAcct.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumCheckingAcct :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumCheckingAcct :: unhandled");
                                            break;
                                    }
                                }
                                normalized_index += Enum.GetValues(typeof(EnumCheckingAcct)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.CreditHist:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.CreditHist);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumCreditHist.Critical:
                                            addNormalized(ref normalized,
                                                probability, EnumCreditHist.Critical, normalized_index);
                                            break;
                                        case (int)EnumCreditHist.Delay:
                                            addNormalized(ref normalized,
                                                probability, EnumCreditHist.Delay, normalized_index);
                                            break;
                                        case (int)EnumCreditHist.BankPaid:
                                            addNormalized(ref normalized,
                                                probability, EnumCreditHist.BankPaid, normalized_index);
                                            break;
                                        case (int)EnumCreditHist.Current:
                                            addNormalized(ref normalized,
                                                probability, EnumCreditHist.Current, normalized_index);
                                            break;
                                        case (int)EnumCreditHist.AllPaid:
                                            addNormalized(ref normalized,
                                                probability, EnumCreditHist.AllPaid, normalized_index);
                                            break;

                                        case (int)EnumCreditHist.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumCreditHist :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumCreditHist :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumCreditHist)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.Purpose:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Purpose);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumPurpose.SmallAppliance:
                                            addNormalized(ref normalized, probability, EnumPurpose.SmallAppliance, normalized_index);
                                            break;
                                        case (int)EnumPurpose.Furniture:
                                            addNormalized(ref normalized, probability, EnumPurpose.Furniture, normalized_index);
                                            break;
                                        case (int)EnumPurpose.LargeAppliance:
                                            addNormalized(ref normalized, probability, EnumPurpose.LargeAppliance, normalized_index);
                                            break;
                                        case (int)EnumPurpose.Repairs:
                                            addNormalized(ref normalized, probability, EnumPurpose.Repairs, normalized_index);
                                            break;
                                        case (int)EnumPurpose.Other:
                                            addNormalized(ref normalized, probability, EnumPurpose.Other, normalized_index);
                                            break;
                                        case (int)EnumPurpose.CarUsed:
                                            addNormalized(ref normalized, probability, EnumPurpose.CarUsed, normalized_index);
                                            break;
                                        case (int)EnumPurpose.Retraining:
                                            addNormalized(ref normalized, probability, EnumPurpose.Retraining, normalized_index);
                                            break;
                                        case (int)EnumPurpose.Education:
                                            addNormalized(ref normalized, probability, EnumPurpose.Education, normalized_index);
                                            break;
                                        case (int)EnumPurpose.CarNew:
                                            addNormalized(ref normalized, probability, EnumPurpose.CarNew, normalized_index);
                                            break;
                                        case (int)EnumPurpose.Business:
                                            addNormalized(ref normalized, probability, EnumPurpose.Business, normalized_index);
                                            break;

                                        case (int)EnumPurpose.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumPurpose :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumPurpose :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumPurpose)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.SavingsAcct:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.SavingsAcct);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumSavingsAcct.Low:
                                            addNormalized(ref normalized, probability, EnumSavingsAcct.Low, normalized_index);
                                            break;
                                        case (int)EnumSavingsAcct.MedLow:
                                            addNormalized(ref normalized, probability, EnumSavingsAcct.MedLow, normalized_index);
                                            break;
                                        case (int)EnumSavingsAcct.NoAcct:
                                            addNormalized(ref normalized, probability, EnumSavingsAcct.NoAcct, normalized_index);
                                            break;
                                        case (int)EnumSavingsAcct.MedHigh:
                                            addNormalized(ref normalized, probability, EnumSavingsAcct.MedHigh, normalized_index);
                                            break;
                                        case (int)EnumSavingsAcct.High:
                                            addNormalized(ref normalized, probability, EnumSavingsAcct.High, normalized_index);
                                            break;

                                        case (int)EnumPurpose.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumSavingsAcct :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumSavingsAcct :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumSavingsAcct)).Length - 1;
                                break;


                            case (int)EnumCreditRisk.Employment:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Employment);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumEmployment.Unemployed:
                                            addNormalized(ref normalized, probability, EnumEmployment.Unemployed, normalized_index);
                                            break;
                                        case (int)EnumEmployment.VeryShort:
                                            addNormalized(ref normalized, probability, EnumEmployment.VeryShort, normalized_index);
                                            break;
                                        case (int)EnumEmployment.Short:
                                            addNormalized(ref normalized, probability, EnumEmployment.Short, normalized_index);
                                            break;
                                        case (int)EnumEmployment.Medium:
                                            addNormalized(ref normalized, probability, EnumEmployment.Medium, normalized_index);
                                            break;
                                        case (int)EnumEmployment.Long:
                                            addNormalized(ref normalized, probability, EnumEmployment.Long, normalized_index);
                                            break;

                                        case (int)EnumPurpose.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumEmployment :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumEmployment :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumEmployment)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.Gender:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Gender);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumGender.F:
                                            addNormalized(ref normalized, probability, EnumGender.F, normalized_index);
                                            break;
                                        case (int)EnumGender.M:
                                            addNormalized(ref normalized, probability, EnumGender.M, normalized_index);
                                            break;

                                        case (int)EnumPurpose.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumGender :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumGender :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumGender)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.PersonalStatus:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.PersonalStatus);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumPersonalStatus.Single:
                                            addNormalized(ref normalized, probability, EnumPersonalStatus.Single, normalized_index);
                                            break;
                                        case (int)EnumPersonalStatus.Divorced:
                                            addNormalized(ref normalized, probability, EnumPersonalStatus.Divorced, normalized_index);
                                            break;
                                        case (int)EnumPersonalStatus.Married:
                                            addNormalized(ref normalized, probability, EnumPersonalStatus.Married, normalized_index);
                                            break;

                                        case (int)EnumPurpose.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumPersonalStatus :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumPersonalStatus :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumPersonalStatus)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.Housing:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Housing);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumHousing.Other:
                                            addNormalized(ref normalized, probability, EnumHousing.Other, normalized_index);
                                            break;
                                        case (int)EnumHousing.Rent:
                                            addNormalized(ref normalized, probability, EnumHousing.Rent, normalized_index);
                                            break;
                                        case (int)EnumHousing.Own:
                                            addNormalized(ref normalized, probability, EnumHousing.Own, normalized_index);
                                            break;

                                        case (int)EnumPurpose.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumHousing :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumHousing :: unhandled");
                                            break;
                                    }

                                }
                                normalized_index += Enum.GetValues(typeof(EnumHousing)).Length - 1;
                                break;

                            case (int)EnumCreditRisk.Job:
                                probability = probabilityDiscrete
                                    (discretized[i].ColumnToArray(), EnumCreditRisk.Job);

                                foreach (double cell in discretized[i].ColumnToArray())
                                {
                                    switch ((int)cell)
                                    {
                                        case (int)EnumJob.Unskilled:
                                            addNormalized(ref normalized, probability, EnumJob.Unskilled, normalized_index);
                                            break;
                                        case (int)EnumJob.Unemployed:
                                            addNormalized(ref normalized, probability, EnumJob.Unemployed, normalized_index);
                                            break;
                                        case (int)EnumJob.Skilled:
                                            addNormalized(ref normalized, probability, EnumJob.Skilled, normalized_index);
                                            break;
                                        case (int)EnumJob.Management:
                                            addNormalized(ref normalized, probability, EnumJob.Management, normalized_index);
                                            break;

                                        case (int)EnumPurpose.unknown:
                                            Print("in Normalize.switch(dataType)",
                                                "unknown EnumJob :: unhandled");
                                            break;
                                        default:
                                            Print("in Normalize.switch(dataType)",
                                                "default EnumJob :: unhandled");
                                            break;
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
