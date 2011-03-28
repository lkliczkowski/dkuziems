using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    partial class ZScore
    {
        public static float StdDevContinuous(float[] floatList)
        {
            float average = floatList.Average();
            float sumOfDerivation = 0;
            foreach (float value in floatList)
            {
                sumOfDerivation += (value) * (value);
            }
            float sumOfDerivationAverage = sumOfDerivation / floatList.Count();
            return (float)Math.Sqrt(sumOfDerivationAverage - (average * average));
        }

        public static float[] ProbabilityDiscrete(float[] discretizedList,
            EnumHeartDisease discreteType)
        {
            List<float> probabilityList = new List<float>();
            switch (discreteType)
            {
                case EnumHeartDisease.LowMediumHigh:
                    AddZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumLowMediumHigh)).Length - 1);
                    foreach (float cell in discretizedList)
                    {
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumHeartDisease.Obesity:
                    AddZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumObesity)).Length - 1);
                    foreach (float cell in discretizedList)
                    {
                        probabilityList[(int)cell]++;
                    }
                    break;
                case EnumHeartDisease.AgeRange:
                    AddZero(ref probabilityList,
                        Enum.GetValues(typeof(EnumAgeRange)).Length - 1);
                    foreach (float cell in discretizedList)
                    {
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

        public static float StdDevDiscrete(float probability)
        {
            return (float)Math.Sqrt(probability * (1 - probability));
        }


        public static void AddZero(ref List<float> list, int times)
        {
            for (int i = 0; i < times; i++)
                list.Add(0);
        }


    }
}
