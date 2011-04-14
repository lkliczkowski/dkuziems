using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    partial class ZScore
    {
        public static Column<float>[] Discretize(
            Column<string>[] rawData,
            EnumDataTypes switchType,
            int[] columnType)
        {
            Column<float>[] discretizedData = new Column<float>[columnType.Length];
            for (int i = 0; i < columnType.Length; i++)
                discretizedData[i] = new Column<float>();

            switch (switchType)
            {
                case EnumDataTypes.HeartDisease:
                    Print("ZScoreDiscretize.Discretize", "case EnumDataTypes.HeartDisease");
                    for (int j = 0; j < rawData[0].GetNum(); j++)
                    {
                        for (int i = 0; i < columnType.Length; i++)
                        {
                            switch (columnType[i])
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
                                    discretizedData[i].AddData((float)((int)lmhOption));
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
                                    discretizedData[i].AddData((float)((int)apOption)); //byte type 0/1/miss
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
                                    discretizedData[i].AddData((float)((int)obeOption));
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
                                    discretizedData[i].AddData((float)((int)arOption));
                                    break;

                                default:
                                    Print("Unrecognized", "HeartDisease[switch by columnType]");
                                    discretizedData[i].AddData(-1);
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
    }
}
