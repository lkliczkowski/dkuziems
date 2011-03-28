using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    static class ZScoreDiscretize
    {
        public static Records<float>[] Discretize(
            Records<string>[] rawData,
            EnumDataTypes switchType,
            int[] recordsType)
        {
            Records<float>[] discretizedData = new Records<float>[recordsType.Length];
            for (int i = 0; i < recordsType.Length; i++)
                discretizedData[i] = new Records<float>();

            switch (switchType)
            {
                case EnumDataTypes.HeartDisease:
                    Print("ZScoreDiscretize.Discretize", "case EnumDataTypes.HeartDisease");
                    for (int j = 0; j < rawData[0].GetNum(); j++)
                    {
                        for (int i = 0; i < recordsType.Length; i++)
                        {
                            switch (recordsType[i])
                            {
                                case (int)EnumHeartDisease.Value:
                                    float val;
                                    try
                                    {
                                        val = float.Parse(rawData[i].Get(j));
                                    }
                                    catch
                                    {
                                        failParseInfo("value", i);
                                        Console.WriteLine(rawData[i].Get(j));
                                        val = -1;
                                    }
                                    discretizedData[i].AddRecord(val);
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
                                    discretizedData[i].AddRecord((float)((int)lmhOption));
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
                                    discretizedData[i].AddRecord((float)((int)apOption)); //byte type 0/1/miss
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
                                    discretizedData[i].AddRecord((float)((int)obeOption));
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
                                    discretizedData[i].AddRecord((float)((int)arOption));
                                    break;

                                default:
                                    Print("Unrecognized", "HeartDisease[switch by recordsType]");
                                    discretizedData[i].AddRecord(-1);
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }


            return discretizedData;
        }

        public static void Print(string what, string where)
        {
            Console.WriteLine(">>>> {0}::{1}", what, where);
        }

        public static void Print(string what)
        {
            Print(what, null);
        }

        private static void failParseInfo(string where)
        {
            Console.WriteLine(">> Failed at ParseToEnum::{0} set to unknown", where);
        }

        private static void failParseInfo(string where, int no)
        {
            Console.WriteLine(">> Failed at ParseToEnum::{0} set to unknown [{1}]", where, no);
        }

        private static void failParseInfo(string where, int no, int rec_num)
        {
            Console.WriteLine(">> Failed at ParseToEnum::{0} set to unknown [{1},{2}]", where, no, rec_num);
        }

        public static void PrintInfo(string what, float val)
        {
            Console.Write(">> {0}::[{1}]\t", what, val);
        }

        #region dyskretyzacja zwyczajna (np. dla 5 zm: 0, 0.25 , 0.5, 0.75, 1)
        /*
        static class ZScoreDiscretize
        {
            public static Records<float>[] Discretize(
                Records<string>[] rawData,
                EnumDataTypes switchType,
                int[] recordsType)
            {
                Records<float>[] discretizedData = new Records<float>[recordsType.Length];
                for (int i = 0; i < recordsType.Length; i++)
                    discretizedData[i] = new Records<float>();

                switch (switchType)
                {
                    case EnumDataTypes.HeartDisease:
                        Print("ZScoreDiscretize.Discretize", "case EnumDataTypes.HeartDisease");
                        for (int j = 0; j < rawData[0].GetNum(); j++)
                        {
                            for (int i = 0; i < recordsType.Length; i++)
                            {
                                switch (recordsType[i])
                                {
                                    case (int)EnumHeartDisease.Value:
                                        float val;
                                        try
                                        {
                                            val = float.Parse(rawData[i].Get(j));
                                        }
                                        catch
                                        {
                                            failParseInfo("value", i);
                                            Console.WriteLine(rawData[i].Get(j));
                                            val = -1;
                                        }
                                        discretizedData[i].AddRecord(val);
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
                                        discretizedData[i].AddRecord((float)((int)lmhOption) /
                                            (Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1));
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
                                        discretizedData[i].AddRecord((float)((int)apOption)); //byte type 0/1/miss
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
                                        discretizedData[i].AddRecord((float)((int)obeOption) /
                                            (Enum.GetValues(typeof(EnumObesity)).Length - 1));
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
                                        discretizedData[i].AddRecord((float)((int)arOption) /
                                            (Enum.GetValues(typeof(EnumAgeRange)).Length - 1));
                                        break;

                                    default:
                                        Print("Unrecognized", "HeartDisease[switch by recordsType]");
                                        discretizedData[i].AddRecord(-1);
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }


                return discretizedData;
            }
            */
        #endregion
    }
}
