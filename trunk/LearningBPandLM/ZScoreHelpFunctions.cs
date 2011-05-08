using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    partial class ZScore
    {
        private static double stdDevContinuous(double[] doubleList)
        {
            double average = doubleList.Average();
            double sumOfDerivation = 0;
            foreach (double value in doubleList)
            {
                sumOfDerivation += (value) * (value);
            }
            double sumOfDerivationAverage = sumOfDerivation / doubleList.Count();
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }

        private static double[] probabilityDiscrete(double[] discretizedList,
            EnumHeartDisease discreteType)
        {
            List<double> probabilityList = new List<double>();
            switch (discreteType)
            {
                case EnumHeartDisease.LowMediumHigh:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumLowMediumHigh.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumHeartDisease.Obesity:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumObesity)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumObesity.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumHeartDisease.AgeRange:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumAgeRange)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumAgeRange.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                default:
                    Print("ProbabilityDiscrete", "default");
                    break;
            }
            for (int i = 0; i < probabilityList.Count(); i++)
            {
                probabilityList[i] /= discretizedList.Length;
            }

            return probabilityList.ToArray();
        }

        private static double[] probabilityDiscrete(double[] discretizedList, EnumCreditRisk discreteType)
        {
            List<double> probabilityList = new List<double>();
            switch (discreteType)
            {
                case EnumCreditRisk.CheckingAcct:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumCheckingAcct)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumCheckingAcct.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumCreditRisk.CreditHist:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumCreditHist)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumCreditHist.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumCreditRisk.Purpose:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumPurpose)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumPurpose.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;

                case EnumCreditRisk.SavingsAcct:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumSavingsAcct)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumSavingsAcct.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumCreditRisk.Employment:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumEmployment)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumEmployment.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumCreditRisk.Gender:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumGender)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumGender.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumCreditRisk.PersonalStatus:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumPersonalStatus)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumPersonalStatus.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumCreditRisk.Housing:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumHousing)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumHousing.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumCreditRisk.Job:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumJob)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumJob.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                default:
                    Print("ProbabilityDiscrete", "default");
                    break;
            }
            for (int i = 0; i < probabilityList.Count(); i++)
            {
                probabilityList[i] /= discretizedList.Length;
            }

            return probabilityList.ToArray();
        }

        private static double stdDevDiscrete(double probability)
        {
            return Math.Sqrt(probability * (1 - probability));
        }


        private static void addZero(ref List<double> list, int times)
        {
            for (int i = 0; i < times; i++)
                list.Add(0);
        }

        private static int GetNormalizeLength(int[] tabTypes, EnumDataTypes en)
        {
            int len = 0;
            switch (en)
            {
                case EnumDataTypes.HeartDisease:
                    foreach (int i in tabTypes)
                    {
                        switch (i)
                        {
                            case (int)EnumHeartDisease.Value:
                                len++;
                                break;
                            case (int)EnumHeartDisease.LowMediumHigh:
                                len += (Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1);
                                break;
                            case (int)EnumHeartDisease.AbsentPresent:
                                len += (Enum.GetValues(typeof(EnumAbsentPresent)).Length - 1);
                                break;
                            case (int)EnumHeartDisease.Obesity:
                                len += (Enum.GetValues(typeof(EnumObesity)).Length - 1);
                                break;
                            case (int)EnumHeartDisease.AgeRange:
                                len += (Enum.GetValues(typeof(EnumAgeRange)).Length - 1);
                                break;
                        }
                    }
                    break;

                case EnumDataTypes.LetterRecognitionA:
                    len = tabTypes.Length;
                    break;
                default:
                    break;

                case EnumDataTypes.CreditRisk:
                    foreach (int i in tabTypes)
                    {
                        switch (i)
                        {
                            case (int)EnumCreditRisk.CheckingAcct:
                                len += (Enum.GetValues(typeof(EnumCheckingAcct)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.CreditHist:
                                len += (Enum.GetValues(typeof(EnumCreditHist)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.Purpose:
                                len += (Enum.GetValues(typeof(EnumPurpose)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.SavingsAcct:
                                len += (Enum.GetValues(typeof(EnumSavingsAcct)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.Employment:
                                len += (Enum.GetValues(typeof(EnumEmployment)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.Gender:
                                len += (Enum.GetValues(typeof(EnumGender)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.PersonalStatus:
                                len += (Enum.GetValues(typeof(EnumPersonalStatus)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.Housing:
                                len += (Enum.GetValues(typeof(EnumHousing)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.Job:
                                len += (Enum.GetValues(typeof(EnumJob)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.Telephone:
                            case (int)EnumCreditRisk.Foreign:
                                len += (Enum.GetValues(typeof(EnumYesNo)).Length - 1);
                                break;
                            case (int)EnumCreditRisk.MonthsAcct:
                            case (int)EnumCreditRisk.ResidenceTime:
                            case (int)EnumCreditRisk.Age:
                                len++;
                                break;
                            case (int)EnumCreditRisk.CreditStanding:
                                len += (Enum.GetValues(typeof(EnumGoodBad)).Length - 1);
                                break;
                            default:
                                Print("GetNormalizeLength", "default");
                                break;
                        }
                    }
                    break;
            }

            return len;
        }

        private static void RemoveFromRecords<T>(ref Column<T>[] toRemove, int from, int to)
        {
            for (int i = 0; i < toRemove.Length; i++)
            {
                toRemove[i].RemoveRange(from, to);
            }
        }

        private static void Print(string what, string where)
        {
            Console.WriteLine(">>>> {0}::{1}", what, where);
        }

        private static void Print(double[] toPrint)
        {
            foreach (double f in toPrint)
                Console.Write("{0:N2}\t", f);
            Console.WriteLine();
        }

        private static void Print(string toPrint)
        {
            Console.WriteLine(">>>> {0}", toPrint);
        }

        private static void Print()
        {
            Console.WriteLine();
        }

    }
}
