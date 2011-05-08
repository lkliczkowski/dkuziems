﻿
namespace ZScore
{
    public enum EnumDataTypes
    {
        unknown = 0,
        HeartDisease = 1,
        LetterRecognitionA = 2,
        CreditRisk = 3
    }

    public enum EnumHeartDisease
    {
        Value = 0, //continuous
        LowMediumHigh = 1,
        AbsentPresent = 2,
        Obesity = 3,
        AgeRange = 4

    }

    public enum EnumCreditRisk
    {
        /// <summary>
        /// EnumCheckingAcct :: 
        /// unknown = -1,
        /// Low = 0,
        /// NoAcct = 1,
        /// Balance = 2,
        /// High = 3
        /// </summary>
        CheckingAcct = 1,

        /// <summary>
        /// EnumCreditHist :: 
        /// unknown = -1,
        /// Critical = 0,
        /// Delay = 1,
        /// BankPaid = 2,
        /// Current = 3,
        /// AllPaid = 4
        /// </summary>
        CreditHist = 2,

        /// <summary>
        /// EnumPurpose :: 
        /// unknown = -1,
        /// SmallAppliance = 0,
        /// Furniture = 1,
        /// LargeAppliance = 2,
        /// Repairs = 3,
        /// Other = 4,
        /// CarUsed = 5,
        /// Retraining = 6,
        /// Education = 7,
        /// CarNew = 8,
        /// Business = 9
        /// </summary>
        Purpose = 3,

        /// <summary>
        /// EnumSavingsAcct :: 
        /// unknown = -1,
        /// Low = 0,
        /// MedLow = 1,
        /// NoAcct = 2,
        /// MedHigh = 3,
        /// High = 4
        /// </summary>
        SavingsAcct = 4,

        /// <summary>
        /// EnumEmployment :: 
        /// unknown = -1,
        /// Unemployed = 0,
        /// VeryShort = 1,
        /// Short = 2,
        /// Medium = 3,
        /// Long = 4,
        /// </summary>
        Employment = 5,

        /// <summary>
        /// EnumGender :: 
        /// unknown = -1,
        /// F = 0,
        /// M = 1
        /// </summary>
        Gender = 6,

        /// <summary>
        /// EnumPersonalStatus :: 
        /// unknown = -1,
        /// Single = 0,
        /// Divorced = 1,
        /// Married = 2
        /// </summary>
        PersonalStatus = 7,

        /// <summary>
        /// EnumHousing :: 
        /// unknown = -1,
        /// Other = 0,
        /// Rent = 1,
        /// Own = 2
        /// </summary>
        Housing = 8,

        /// <summary>
        /// EnumJob :: 
        /// unknown = -1,
        /// Unskilled = 0,
        /// Unemployed = 1,
        /// Skilled = 2,
        /// Management = 3
        /// </summary>
        Job = 9,

        /// <summary>
        /// EnumYesNo :: 
        /// unknown = -1,
        /// No = 0,
        /// Yes = 1
        /// </summary>
        Telephone = 10,

        /// <summary>
        /// EnumYesNo :: 
        /// unknown = -1,
        /// No = 0,
        /// Yes = 1
        /// </summary>
        Foreign = 11,

        /// <summary>
        /// wartosc ciagla (value)
        /// </summary>
        MonthsAcct = 12,

        /// <summary>
        /// wartosc ciagla (value)
        /// </summary>
        ResidenceTime = 13,

        /// <summary>
        /// wartosc ciagla (value)
        /// </summary>
        Age = 14,

        /// <summary>
        /// EnumGoodBad :: 
        /// unknown = -1,
        /// Bad = 0,
        /// Good = 1
        /// </summary>
        CreditStanding = 15
    }

    public enum EnumGoodBad
    {
        unknown = -1,
        Bad = 0,
        Good = 1
    }

    public enum EnumYesNo
    {
        unknown = -1,
        No = 0,
        Yes = 1
    }

    public enum EnumJob
    {
        unknown = -1,
        Unskilled = 0,
        Unemployed = 1,
        Skilled = 2,
        Management = 3
    }

    public enum EnumHousing
    {
        unknown = -1,
        Other = 0,
        Rent = 1,
        Own = 2
    }

    public enum EnumPersonalStatus
    {
        unknown = -1,
        Single = 0,
        Divorced = 1,
        Married = 2
    }

    public enum EnumGender
    {
        unknown = -1,
        F = 0,
        M = 1
    }

    public enum EnumEmployment
    {
        unknown = -1,
        Unemployed = 0,
        VeryShort = 1,
        Short = 2,
        Medium = 3,
        Long = 4,
    }

    public enum EnumSavingsAcct
    {
        unknown = -1,
        Low = 0,
        MedLow = 1,
        NoAcct = 2,
        MedHigh = 3,
        High = 4
    }

    public enum EnumCheckingAcct
    {
        unknown = -1,
        Low = 0,
        NoAcct = 1,
        Balance = 2,
        High = 3

    }

    public enum EnumCreditHist
    {
        unknown = -1,
        Critical = 0,
        Delay = 1,
        BankPaid = 2,
        Current = 3,
        AllPaid = 4
    }
    
    public enum EnumPurpose
    {
        unknown = -1,
        SmallAppliance = 0,
        Furniture = 1,
        LargeAppliance = 2,
        Repairs = 3,
        Other = 4,
        CarUsed = 5,
        Retraining = 6,
        Education = 7,
        CarNew = 8,
        Business = 9
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
        underweight = 0,
        Healthy = 1,
        overweight = 2,
        clinicallyobese = 3
    }

    public enum EnumAgeRange
    {
        unknown = -1,
        young = 0,
        middleaged = 1,
        old = 2,
    }
}
