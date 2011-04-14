using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    public enum EnumDataTypes
    {
        unknown = 0,
        HeartDisease = 1
    }
    public enum EnumHeartDisease
    {
        Value = 0, //continuous
        LowMediumHigh = 1,
        AbsentPresent = 2,
        Obesity = 3,
        AgeRange = 4

    }
    public enum EnumLowMediumHigh
    {
        unknown = -1,
        low = 0,
        medium = 1,
        high = 2
    }

    public enum EnumAbsentPresent
    {
        unknown = -1,
        Absent = 0,
        Present = 1
    }

    public enum EnumObesity
    {
        unknown = -1,
        Healthy = 0,
        overweight = 1,
        clinicallyobese = 2
    }

    public enum EnumAgeRange
    {
        unknown = -1,
        young = 0,
        middleaged = 1,
        old = 2
    }
}
