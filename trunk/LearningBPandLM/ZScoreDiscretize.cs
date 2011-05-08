using System;

namespace ZScore
{
    partial class ZScore
    {
        public static Column<double>[] Discretize(Column<string>[] rawData,
            EnumDataTypes switchType,int[] columnType)
        {
            Column<double>[] discretizedData = new Column<double>[columnType.Length];
            for (int i = 0; i < columnType.Length; i++)
                discretizedData[i] = new Column<double>();

            switch (switchType)
            {
                #region heartdisease
                case EnumDataTypes.HeartDisease:
                    Print("ZScoreDiscretize.Discretize", "case EnumDataTypes.HeartDisease");
                    for (int j = 0; j < rawData[0].GetNum(); j++)
                    {
                        for (int i = 0; i < columnType.Length; i++)
                        {
                            switch (columnType[i])
                            {
                                case (int)EnumHeartDisease.Value:
                                    double val;
                                    try
                                    {
                                        val = double.Parse(rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("value", i);
                                        Console.WriteLine(rawData[i].Get(j));
                                        val = -1;
                                    }
                                    discretizedData[i].AddData(val);
                                    break;

                                case (int)EnumHeartDisease.LowMediumHigh:
                                    EnumLowMediumHigh lmhOption;
                                    try
                                    {
                                        lmhOption = (EnumLowMediumHigh)Enum.
                                            Parse(typeof(EnumLowMediumHigh), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("lmhOption", i, j);
                                        lmhOption = EnumLowMediumHigh.unknown;
                                    }
                                    discretizedData[i].AddData(((int)lmhOption));
                                    break;

                                case (int)EnumHeartDisease.AbsentPresent:
                                    EnumAbsentPresent apOption;
                                    try
                                    {
                                        apOption = (EnumAbsentPresent)Enum.
                                            Parse(typeof(EnumAbsentPresent), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("apOption", i);
                                        apOption = EnumAbsentPresent.unknown;
                                    }
                                    discretizedData[i].AddData(((int)apOption)); //byte type 0/1/miss
                                    break;

                                case (int)EnumHeartDisease.Obesity:
                                    EnumObesity obeOption;
                                    try
                                    {
                                        obeOption = (EnumObesity)Enum.
                                            Parse(typeof(EnumObesity), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("obeOption", i, j);
                                        obeOption = EnumObesity.unknown;
                                    }
                                    discretizedData[i].AddData(((int)obeOption));
                                    break;

                                case (int)EnumHeartDisease.AgeRange:
                                    EnumAgeRange arOption;
                                    try
                                    {
                                        arOption = (EnumAgeRange)Enum.
                                            Parse(typeof(EnumAgeRange), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("arOption", i, j);
                                        arOption = EnumAgeRange.unknown;
                                    }
                                    discretizedData[i].AddData(((int)arOption));
                                    break;

                                default:
                                    Print("Unrecognized", "HeartDisease[switch by columnType]");
                                    discretizedData[i].AddData(-1);
                                    break;
                            }
                        }
                    }
                    break;
                #endregion
                #region letterRecognition
                case EnumDataTypes.LetterRecognitionA:
                    Print("ZScoreDiscretize.Discretize", "case LetterRecognitionA");
                    for (int j = 0; j < rawData[0].GetNum(); j++)
                    {
                        for (int i = 0; i < columnType.Length; i++)
                        {
                            switch (columnType[i])
                            {
                                case 1:
                                    double val;
                                    try
                                    {
                                        val = double.Parse(rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("value", i);
                                        Console.WriteLine(rawData[i].Get(j));
                                        val = -1;
                                    }
                                    discretizedData[i].AddData(val);
                                    break;
                                case 0:
                                    double ZeroOrOneOption;
                                    try
                                    {
                                        ZeroOrOneOption = double.Parse(rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("ZeroOrOneOption", i);
                                        ZeroOrOneOption = -1;
                                    }
                                    discretizedData[i].AddData(ZeroOrOneOption); //byte type 0/1/miss
                                    break;
                                default:
                                    Print("Default", "LetterRecognitionA[switch by columnType]");
                                    discretizedData[i].AddData(-1);
                                    break;
                            }
                        }
                    }
                    break;
                #endregion
                #region creditRisk
                case EnumDataTypes.CreditRisk:
                    Print("ZScoreDiscretize.Discretize", "case CreditRisk");
                    for (int j = 0; j < rawData[0].GetNum(); j++)
                    {
                        for (int i = 0; i < columnType.Length; i++)
                        {
                            switch (columnType[i])
                            {
                                case (int)EnumCreditRisk.CheckingAcct:
                                    EnumCheckingAcct checkAcc;
                                    try
                                    {
                                        checkAcc = (EnumCheckingAcct)Enum.
                                            Parse(typeof(EnumCheckingAcct), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("checkAcc", i, j);
                                        checkAcc = EnumCheckingAcct.unknown;
                                    }
                                    discretizedData[i].AddData(((int)checkAcc));
                                    break;

                                case (int)EnumCreditRisk.CreditHist:
                                    EnumCreditHist creditHist;
                                    try
                                    {
                                        creditHist = (EnumCreditHist)Enum.
                                            Parse(typeof(EnumCreditHist), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("creditHist", i, j);
                                        creditHist = EnumCreditHist.unknown;
                                    }
                                    discretizedData[i].AddData(((int)creditHist));
                                    break;

                                case (int)EnumCreditRisk.Purpose:
                                    EnumPurpose purpose;
                                    try
                                    {
                                        purpose = (EnumPurpose)Enum.
                                            Parse(typeof(EnumPurpose), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("purpose", i, j);
                                        purpose = EnumPurpose.unknown;
                                    }
                                    discretizedData[i].AddData(((int)purpose));
                                    break;

                                case (int)EnumCreditRisk.SavingsAcct:
                                    EnumSavingsAcct savingAcc;
                                    try
                                    {
                                        savingAcc = (EnumSavingsAcct)Enum.
                                            Parse(typeof(EnumSavingsAcct), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("savingAcc", i, j);
                                        savingAcc = EnumSavingsAcct.unknown;
                                    }
                                    discretizedData[i].AddData(((int)savingAcc));
                                    break;

                                case (int)EnumCreditRisk.Employment:
                                    EnumEmployment employ;
                                    try
                                    {
                                        employ = (EnumEmployment)Enum.
                                            Parse(typeof(EnumEmployment), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("employ", i, j);
                                        employ = EnumEmployment.unknown;
                                    }
                                    discretizedData[i].AddData(((int)employ));
                                    break;

                                case (int)EnumCreditRisk.Gender:
                                    EnumGender gender;
                                    try
                                    {
                                        gender = (EnumGender)Enum.
                                            Parse(typeof(EnumGender), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("gender", i, j);
                                        gender = EnumGender.unknown;
                                    }
                                    discretizedData[i].AddData(((int)gender));
                                    break;

                                case (int)EnumCreditRisk.PersonalStatus:
                                    EnumPersonalStatus pStatus;
                                    try
                                    {
                                        pStatus = (EnumPersonalStatus)Enum.
                                            Parse(typeof(EnumPersonalStatus), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("pStatus", i, j);
                                        pStatus = EnumPersonalStatus.unknown;
                                    }
                                    discretizedData[i].AddData(((int)pStatus));
                                    break;

                                case (int)EnumCreditRisk.Housing:
                                    EnumHousing housing;
                                    try
                                    {
                                        housing = (EnumHousing)Enum.
                                            Parse(typeof(EnumHousing), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("housing", i, j);
                                        housing = EnumHousing.unknown;
                                    }
                                    discretizedData[i].AddData(((int)housing));
                                    break;

                                case (int)EnumCreditRisk.Job:
                                    EnumJob job;
                                    try
                                    {
                                        job = (EnumJob)Enum.
                                            Parse(typeof(EnumJob), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("job", i, j);
                                        Print(rawData[i].Get(j));
                                        job = EnumJob.unknown;
                                    }
                                    discretizedData[i].AddData(((int)job));
                                    break;

                                case (int)EnumCreditRisk.Telephone:
                                case (int)EnumCreditRisk.Foreign:
                                    EnumYesNo yesno;
                                    try
                                    {
                                        yesno = (EnumYesNo)Enum.
                                            Parse(typeof(EnumYesNo), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("yesno", i, j);
                                        yesno = EnumYesNo.unknown;
                                    }
                                    discretizedData[i].AddData(((int)yesno));
                                    break;

                                case (int)EnumCreditRisk.MonthsAcct:
                                case (int)EnumCreditRisk.ResidenceTime:
                                case (int)EnumCreditRisk.Age:
                                    double val;
                                    try
                                    {
                                        val = double.Parse(rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("value", i);
                                        Console.WriteLine(rawData[i].Get(j));
                                        val = -1;
                                    }
                                    discretizedData[i].AddData(val);
                                    break;

                                case (int)EnumCreditRisk.CreditStanding:
                                    EnumGoodBad goodbad;
                                    try
                                    {
                                        goodbad = (EnumGoodBad)Enum.
                                            Parse(typeof(EnumGoodBad), rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("arOption", i, j);
                                        goodbad = EnumGoodBad.unknown;
                                    }
                                    discretizedData[i].AddData(((int)goodbad));
                                    break;

                                default:
                                    Print("Default", "LetterRecognitionA[switch by columnType]");
                                    discretizedData[i].AddData(-1);
                                    break;
                            }
                        }
                    }
                    break;
                #endregion
                default:
                    break;
            }

            return discretizedData;
        }

        private static void failParseInfo(string where)
        {
            Console.WriteLine(">> Failed at ParseToEnum::{0} set to unknown", where);
        }

        private static void failParseInfo(string where, int no)
        {
            Console.WriteLine(">> Failed at ParseToEnum::{0} set to unknown [{1}]", where, no);
        }

        private static void failParseInfo(string where, int no, int col_num)
        {
            Console.WriteLine(">> Failed at ParseToEnum::{0} set to unknown [{1},{2}]", where, no, col_num);
        }

        public static void PrintInfo(string what, double val)
        {
            Console.Write(">> {0}::[{1}]\t", what, val);
        }
    }
}
