﻿using System;

namespace ZScore
{
    /// <summary>
    /// Scoring Method, discretize + z-score
    /// </summary>
    partial class ZScoreData
    {
        private readonly string DATAFILE, OUTPUTFILE;
        public EnumDataTypes DataType;
        private int[] columnType;
        private Column<double>[] normalizedData;

        /// <summary>
        /// Zwraca znormalizowane dane
        /// </summary>
        public Column<double>[] NormalizedData
        {
            get { return normalizedData; }
        }

        private ZScoreData()
            : this("ExampleData.csv", "normalizedExample.csv", EnumDataTypes.HeartDisease)
        {}

        public ZScoreData(string f1, EnumDataTypes dt)
            :this (f1, null, dt)
        { }

        public ZScoreData(string f1, string f2, EnumDataTypes dt)
        {
            DATAFILE = f1;
            OUTPUTFILE = f2;
            DataType = dt;
            InitDataColumnType();
        }

        private void InitDataColumnType()
        {
            switch (DataType)
            {
                case EnumDataTypes.unknown:
                    break;
                case EnumDataTypes.HeartDisease:
                    columnType = ColumnTypes.HeartDisease;
                    break;
                case EnumDataTypes.GermanCreditData:
                    columnType = ColumnTypes.GermanCreditData;
                    break;
                case EnumDataTypes.LetterRecognitionA:
                    columnType = ColumnTypes.LetterRecognition;
                    break;
                case EnumDataTypes.CreditRisk:
                    columnType = ColumnTypes.CreditRisk;
                    break;
            }
        }

        public bool NormalizeRun()
        {
            Column<string>[] rawData = new Column<string>[columnType.Length];
            for (int i = 0; i < columnType.Length; i++)
                rawData[i] = new Column<string>();

            if (ZScoreData.CSVread(DATAFILE, ref rawData))
            {
                if (DataType == EnumDataTypes.HeartDisease)
                    RemoveFromRecords(ref rawData, 0, 2);
                else if (DataType == EnumDataTypes.CreditRisk)
                    RemoveFromRecords(ref rawData, 0, 1);

                Print(String.Format("DataSetSize:ColumnNumber\t{0}:{1}", rawData[0].GetNum(), rawData.Length));


                Column<double>[] numericData = MapToNumeric
                    (rawData, DataType, columnType);

                //PrintList(discretizedData);

                normalizedData = new Column<double>
                    [GetNormalizeLength(columnType, DataType)];
                normalize(ref normalizedData, numericData, DataType, columnType);

                Print(String.Format("ZScore on " + DATAFILE), "zakończona!");

                Print(String.Format("DataSetSize \t{0}:{1}", rawData[0].GetNum(), rawData.Length));
                Print(String.Format("NormalizedSetSize\t{0}:{1}", normalizedData[0].GetNum(), normalizedData.Length));

                //PrintList(normalizedData);
                if(OUTPUTFILE != null)
                    CSVwrite(OUTPUTFILE, normalizedData);
            }
            else
            {
                Print("Nieudane ReadingCVSFile", DATAFILE);
                Print(String.Format("Dalsze operacje na " + DATAFILE), "nie będą kontynuowane!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Funkcja pozwalajaca "wydobyc" pojedynczy rekord z danych dla wskazanego indeksu
        /// </summary>
        /// <param name="f">index w danych</param>
        /// <returns>lista danych bez "target"</returns>
        public double[] sample(int f)
        {
            int l;
            switch (DataType)
            {
                case EnumDataTypes.HeartDisease:
                case EnumDataTypes.GermanCreditData:
                case EnumDataTypes.CreditRisk:
                case EnumDataTypes.LetterRecognitionA:
                    l = 2; break;
                default:
                    l = 1; break;
            }
            double[] record = new double[normalizedData.Length - l];

            for (int i = 0; i < record.Length; i++)
            {
                record[i] = normalizedData[i].GetByIndex(f);
            }

            return record;
        }

        /// <summary>
        /// Funkcja pozwalajaca "wydobyc" WY dla wskazanego indeksu
        /// </summary>
        /// <param name="f">index w danych</param>
        /// <returns>wartosc "desired output"</returns>
        public double[] target(int f)
        {
            int l;
            switch (DataType)
            {
                case EnumDataTypes.HeartDisease:
                case EnumDataTypes.GermanCreditData:
                case EnumDataTypes.CreditRisk:
                case EnumDataTypes.LetterRecognitionA:
                    l = 1; break;   //ile OutputNeurons
                    //l = 2; break; //0/1(-2,-1)
                default:
                    l = 1; break;
            }

            double[] t = new double[l];
            for (int i = 0; i < l; i++)
            {
                t[i] = normalizedData[normalizedData.Length - 2 + i].GetByIndex(f);
            }

            return t;
        }
    }
}
