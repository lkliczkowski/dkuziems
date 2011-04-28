using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static int[] LetterRecognition =
        {
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0
        };
    }
}
