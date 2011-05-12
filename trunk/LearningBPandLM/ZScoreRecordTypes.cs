
namespace ZScore
{
    public static class ColumnTypes
    {
        public static int[] HeartDisease =
        {
            (int)EnumHeartDisease.LowMediumHigh,//Blood Pressure Level;
            (int)EnumHeartDisease.Value,//Systolic Blood Pressure;
            (int)EnumHeartDisease.LowMediumHigh,//Tobacco Intake Level;
            (int)EnumHeartDisease.Value,//Tobacco Intake (kg);
            (int)EnumHeartDisease.LowMediumHigh,//Cholesterol Level;
            (int)EnumHeartDisease.Value,//LDL Cholesterol;
            (int)EnumHeartDisease.Value,//Adiposity;
            (int)EnumHeartDisease.AbsentPresent,//Family History;
            (int)EnumHeartDisease.LowMediumHigh,//Stress Level Type A;
            (int)EnumHeartDisease.Value, //Stress Type-A behavior;
            (int)EnumHeartDisease.Obesity,//Obesity Level;
            (int)EnumHeartDisease.Value,//Obesity Body Mass Index (BMI);
            (int)EnumHeartDisease.LowMediumHigh,//Alcohol Intake Level;
            (int)EnumHeartDisease.Value,//Alcohol Consumption;
            (int)EnumHeartDisease.AgeRange,//Age Range;
            (int)EnumHeartDisease.Value,//Age Disease;
            (int)EnumHeartDisease.AbsentPresent//Coronary Heart Disease
        };

        public static int[] GermanCreditData =
        {
            (int)EnumGermanCreditData.A1, //Status of existing checking account
            (int)EnumGermanCreditData.Numerical, //Duration in month
            (int)EnumGermanCreditData.A3, //Credit history
            (int)EnumGermanCreditData.A4, //Purpose
            (int)EnumGermanCreditData.Numerical, //Credit amount
            (int)EnumGermanCreditData.A6, //Savings account/bonds
            (int)EnumGermanCreditData.A7, //Present employment since
            (int)EnumGermanCreditData.Numerical, //Installment rate in percentage of disposable income
            (int)EnumGermanCreditData.A9, //Personal status and sex
            (int)EnumGermanCreditData.A10, //Other debtors / guarantors
            (int)EnumGermanCreditData.Numerical, //Present residence since
            (int)EnumGermanCreditData.A12, //Property
            (int)EnumGermanCreditData.Numerical, //Age in years
            (int)EnumGermanCreditData.A14, //Other installment plans
            (int)EnumGermanCreditData.A15, //Housing
            (int)EnumGermanCreditData.Numerical, //Number of existing credits at this bank
            (int)EnumGermanCreditData.A17, //Job
            (int)EnumGermanCreditData.Numerical, //Number of people being liable to provide maintenance for
            (int)EnumGermanCreditData.A19, //Telephone (binary)
            (int)EnumGermanCreditData.A20, //foreign worker (binary)
            (int)EnumGermanCreditData.Classification //binary
        };

        public static int[] LetterRecognition =
        {
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0
        };

        public static int[] CreditRisk = 
        {
            (int)EnumCreditRisk.CheckingAcct,
            (int)EnumCreditRisk.CreditHist,
            (int)EnumCreditRisk.Purpose,
            (int)EnumCreditRisk.SavingsAcct,
            (int)EnumCreditRisk.Employment,
            (int)EnumCreditRisk.Gender,
            (int)EnumCreditRisk.PersonalStatus,
            (int)EnumCreditRisk.Housing,
            (int)EnumCreditRisk.Job,
            (int)EnumCreditRisk.Telephone,
            (int)EnumCreditRisk.Foreign,
            (int)EnumCreditRisk.MonthsAcct,
            (int)EnumCreditRisk.ResidenceTime,
            (int)EnumCreditRisk.Age,
            (int)EnumCreditRisk.CreditStanding
        };
    }
}
