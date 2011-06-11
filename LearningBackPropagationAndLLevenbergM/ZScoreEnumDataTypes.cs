﻿
namespace ZScore
{
    public enum EnumDataTypes
    {
        unknown,
        HeartDisease,
        GermanCreditData,
        LetterRecognitionA,
        CreditRisk
    }

    #region HeartDisease
    public enum EnumHeartDisease
    {
        Value, //continuous
        LowMediumHigh,
        AbsentPresent,
        Obesity,
        AgeRange

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

    #endregion

    #region GermanCreditData

    public enum EnumGermanCreditData
    {
        /// <summary>
        /// wartosc ciagla (value)
        /// </summary>
        Numerical, 

        /// <summary>
        /// Status of existing checking account (4)
        /// </summary>
        A1, 

        /// <summary>
        /// Credit history (5)
        /// </summary>
        A3,

        /// <summary>
        /// Purpose (11)
        /// </summary>
        A4,

        /// <summary>
        /// Savings account/bonds (5)
        /// </summary>
        A6,

        /// <summary>
        /// Present employment since (5)
        /// </summary>
        A7,

        /// <summary>
        /// Personal status and sex (5)
        /// </summary>
        A9,

        /// <summary>
        /// Other debtors / guarantors (3)
        /// </summary>
        A10,

        /// <summary>
        /// Property (4)
        /// </summary>
        A12,

        /// <summary>
        /// Other installment plans (3)
        /// </summary>
        A14,

        /// <summary>
        /// Housing (3)
        /// </summary>
        A15,

        /// <summary>
        /// Job (4)
        /// </summary>
        A17,

        /// <summary>
        /// Telephone (2) binary
        /// </summary>
        A19,

        /// <summary>
        /// foreign worker (2) binary
        /// </summary>
        A20,

        /// <summary>
        /// 1 == Good, 2 == Bad
        /// </summary>
        Classification

    }

    public enum EnumA1
    {
        unknown = -1,
        A11 = 0, A12 = 1, A13 = 2, A14 = 3 
    }

    public enum EnumA3
    {
        unknown = -1,
        A30 = 0, A31 = 1, A32 = 2, A33 = 3,
        A34 = 4
    }

    public enum EnumA4
    {
        unknown = -1,
        A40 = 0, A41 = 1, A42 = 2, A43 = 3,
        A44 = 4, A45 = 5, A46 = 6, A47 = 7,
        A48 = 8, A49 = 9, A410 = 10
    }

    public enum EnumA6
    {
        unknown = -1,
        A61 = 0, A62 = 1, A63 = 2, A64 = 3,
        A65 = 4
    }

    public enum EnumA7
    {
        unknown = -1,
        A71 = 0, A72 = 1, A73 = 2, A74 = 3,
        A75 = 4
    }

    public enum EnumA9
    {
        unknown = -1,
        A91 = 0, A92 = 1, A93 = 2, A94 = 3,
        A95 = 4
    }

    public enum EnumA10
    {
        unknown = -1,
        A101 = 0, A102 = 1, A103 = 2
    }

    public enum EnumA12
    {
        unknown = -1,
        A121 = 0, A122 = 1, A123 = 2,
        A124 = 3
    }

    public enum EnumA14
    {
        unknown = -1,
        A141 = 0, A142 = 1, A143 = 2
    }

    public enum EnumA15
    {
        unknown = -1,
        A151 = 0, A152 = 1, A153 = 2
    }

    public enum EnumA17
    {
        unknown = -1,
        A171 = 0, A172 = 1, A173 = 2,
        A174 = 3
    }

    public enum EnumA19
    {
        unknown = -1,
        A191 = 0, //none
        A192 = 1 //yes
    }

    public enum EnumA20
    {
        unknown = -1,
        A201 = 1, //yes
        A202 = 0 //no
    }

    #endregion

    #region CreditRisk

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
        CheckingAcct,

        /// <summary>
        /// EnumCreditHist :: 
        /// unknown = -1,
        /// Critical = 0,
        /// Delay = 1,
        /// BankPaid = 2,
        /// Current = 3,
        /// AllPaid = 4
        /// </summary>
        CreditHist,

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
        Purpose,

        /// <summary>
        /// EnumSavingsAcct :: 
        /// unknown = -1,
        /// Low = 0,
        /// MedLow = 1,
        /// NoAcct = 2,
        /// MedHigh = 3,
        /// High = 4
        /// </summary>
        SavingsAcct,

        /// <summary>
        /// EnumEmployment :: 
        /// unknown = -1,
        /// Unemployed = 0,
        /// VeryShort = 1,
        /// Short = 2,
        /// Medium = 3,
        /// Long = 4,
        /// </summary>
        Employment,

        /// <summary>
        /// EnumGender :: 
        /// unknown = -1,
        /// F = 0,
        /// M = 1
        /// </summary>
        Gender,

        /// <summary>
        /// EnumPersonalStatus :: 
        /// unknown = -1,
        /// Single = 0,
        /// Divorced = 1,
        /// Married = 2
        /// </summary>
        PersonalStatus,

        /// <summary>
        /// EnumHousing :: 
        /// unknown = -1,
        /// Other = 0,
        /// Rent = 1,
        /// Own = 2
        /// </summary>
        Housing,

        /// <summary>
        /// EnumJob :: 
        /// unknown = -1,
        /// Unskilled = 0,
        /// Unemployed = 1,
        /// Skilled = 2,
        /// Management = 3
        /// </summary>
        Job,

        /// <summary>
        /// EnumYesNo :: 
        /// unknown = -1,
        /// No = 0,
        /// Yes = 1
        /// </summary>
        Telephone,

        /// <summary>
        /// EnumYesNo :: 
        /// unknown = -1,
        /// No = 0,
        /// Yes = 1
        /// </summary>
        Foreign,

        /// <summary>
        /// wartosc ciagla (value)
        /// </summary>
        MonthsAcct,

        /// <summary>
        /// wartosc ciagla (value)
        /// </summary>
        ResidenceTime,

        /// <summary>
        /// wartosc ciagla (value)
        /// </summary>
        Age,

        /// <summary>
        /// EnumGoodBad :: 
        /// unknown = -1,
        /// Bad = 0,
        /// Good = 1
        /// </summary>
        CreditStanding
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

    #endregion
}
