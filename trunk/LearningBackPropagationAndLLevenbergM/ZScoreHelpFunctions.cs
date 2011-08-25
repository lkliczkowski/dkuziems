using System;
using System.Collections.Generic;
using System.Linq;

namespace ZScore
{
    partial class ZScoreData
    {
        /// <summary>
        /// odchylenie standardowe dla kolumn z danymi typu ciaglego
        /// </summary>
        /// <param name="doubleList">kolumna danych</param>
        /// <returns>odchylenie standardowe</returns>
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

        /// <summary>
        /// prawdopodobienstwa posteri wystepowania poszczegolnych jednostek
        /// typu dyskretnego dla HeartDisease
        /// </summary>
        /// <param name="discretizedList">kolumna danych</param>
        /// <param name="discreteType">typ kolumny</param>
        /// <returns>tablice z obliczonymi prawdopodobienstwami</returns>
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

        /// <summary>
        /// prawdopodobienstwa posteri wystepowania poszczegolnych jednostek
        /// typu dyskretnego dla GermanCreditData
        /// </summary>
        /// <param name="discretizedList">kolumna danych</param>
        /// <param name="discreteType">typ kolumny</param>
        /// <returns>tablice z obliczonymi prawdopodobienstwami</returns>
        private static double[] probabilityDiscrete(double[] discretizedList,
            EnumGermanCreditData discreteType)
        {
            List<double> probabilityList = new List<double>();
            switch (discreteType)
            {
                case EnumGermanCreditData.A1:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA1)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA1.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A3:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA3)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA3.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A4:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA4)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA4.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A6:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA6)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA6.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A7:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA7)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA7.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A9:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA9)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA9.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A10:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA10)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA10.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A12:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA12)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA12.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A14:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA14)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA14.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A15:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA15)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA15.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A17:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA17)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA17.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A19:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA19)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA19.unknown)
                            continue;
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumGermanCreditData.A20:
                    addZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumA20)).Length - 1);
                    foreach (double cell in discretizedList)
                    {
                        if ((int)cell == (int)EnumA20.unknown)
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

        /// <summary>
        /// prawdopodobienstwa posteri wystepowania poszczegolnych jednostek
        /// typu dyskretnego dla CreditRisk
        /// </summary>
        /// <param name="discretizedList">kolumna danych</param>
        /// <param name="discreteType">typ kolumny</param>
        /// <returns>tablice z obliczonymi prawdopodobienstwami</returns>
        private static double[] probabilityDiscrete(double[] discretizedList, 
            EnumCreditRisk discreteType)
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

        /// <summary>
        /// odchylenie standardowe dla wartosci dyskretnych
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        private static double stdDevDiscrete(double probability)
        {
            return Math.Sqrt(probability * (1 - probability));
        }

        /// <summary>
        /// tworzy liste o podanej dlugosci (uzyteczne w celu refaktoryzacji)
        /// </summary>
        /// <param name="list">lista</param>
        /// <param name="times">wskazana dlugosc</param>
        private static void addZero(ref List<double> list, int times)
        {
            for (int i = 0; i < times; i++)
                list.Add(0);
        }

        /// <summary>
        /// oblicza ilosc kolumn danych ustandaryzowanych przez proces z-score
        /// </summary>
        /// <param name="tabTypes">typy kolumn dla wskazanych danych</param>
        /// <param name="en">typ danych</param>
        /// <returns></returns>
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

                case EnumDataTypes.GermanCreditData:
                    foreach (int i in tabTypes)
                    {
                        switch (i)
                        {
                            case (int)EnumGermanCreditData.Numerical:
                                len++;
                                break;
                            case (int)EnumGermanCreditData.A1:
                                len += (Enum.GetValues(typeof(EnumA1)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A3:
                                len += (Enum.GetValues(typeof(EnumA3)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A4:
                                len += (Enum.GetValues(typeof(EnumA4)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A6:
                                len += (Enum.GetValues(typeof(EnumA6)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A7:
                                len += (Enum.GetValues(typeof(EnumA7)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A9:
                                len += (Enum.GetValues(typeof(EnumA9)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A10:
                                len += (Enum.GetValues(typeof(EnumA10)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A12:
                                len += (Enum.GetValues(typeof(EnumA12)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A14:
                                len += (Enum.GetValues(typeof(EnumA14)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A15:
                                len += (Enum.GetValues(typeof(EnumA15)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A17:
                                len += (Enum.GetValues(typeof(EnumA17)).Length - 1);
                                break;
                            case (int)EnumGermanCreditData.A19:
                            case (int)EnumGermanCreditData.A20:
                            case (int)EnumGermanCreditData.Classification:
                                len += (Enum.GetValues(typeof(EnumA20)).Length - 1);
                                break;
                        }
                    }
                    break;

                case EnumDataTypes.LetterRecognitionA:
                    len = tabTypes.Length + 1;
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

        /// <summary>
        /// usuwa wskazany przedzial danych
        /// </summary>
        /// <typeparam name="T">typ danych (string/double/..)</typeparam>
        /// <param name="toRemove">dane typu Column<T>[]</param>
        /// <param name="from">index od</param>
        /// <param name="to">index do</param>
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
