using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ZScore
{
    /// <summary>
    /// Scoring Method, discretize + z-score
    /// </summary>
    partial class ZScore
    {
        private readonly string DATAFILE, OUTPUTFILE;
        public EnumDataTypes DataType;
        private int[] columnType;
        private Column<double>[] normalizedData;
        public Column<double>[] Data
        {
            get { return normalizedData; }
        }

        //TODO: example data
        public ZScore()
            : this("ExampleData.csv", "normalizedExample.csv", EnumDataTypes.HeartDisease)
        {}

        public ZScore(string f1, EnumDataTypes dt)
        {
            DATAFILE = f1;
            OUTPUTFILE = null;
            DataType = dt;
            InitData();
        }

        public ZScore(string f1, string f2, EnumDataTypes dt)
        {
            DATAFILE = f1;
            OUTPUTFILE = f2;
            DataType = dt;
            InitData();
        }

        private void InitData()
        {
            switch (DataType)
            {
                case EnumDataTypes.unknown:
                    break;
                case EnumDataTypes.HeartDisease:
                    columnType = ColumnTypes.HeartDisease;
                    break;
                case EnumDataTypes.LetterRecognitionA:
                    columnType = ColumnTypes.LetterRecognition;
                    break;
            }
        }

        public bool NormalizeRun()
        {
            Column<string>[] rawData = new Column<string>[columnType.Length];
            for (int i = 0; i < columnType.Length; i++)
                rawData[i] = new Column<string>();

            if (ZScore.CSVread(DATAFILE, ref rawData))
            {
                if(DataType == EnumDataTypes.HeartDisease)
                    RemoveFromRecords(ref rawData, 0, 2);

                Print(String.Format("DataSetSize:ColumnNumber\t{0}:{1}", rawData[0].GetNum(), rawData.Length));


                Column<double>[] discretizedData = Discretize
                    (rawData, DataType, columnType);

                //PrintList(discretizedData);

                normalizedData = new Column<double>
                    [GetNormalizeLength(columnType, DataType)];
                normalize(ref normalizedData, discretizedData, DataType, columnType);

                Print(String.Format("ZScore on " + DATAFILE), "zakończona!");

                Print(String.Format("DataSetSize \t{0}:{1}", rawData[0].GetNum(), rawData.Length));
                Print(String.Format("NormalizedSetSize\t{0}:{1}", normalizedData[0].GetNum(), normalizedData.Length));

                //PrintList(normalizedData);
                if(OUTPUTFILE != null)
                    CSVwrite(OUTPUTFILE, normalizedData);
            }
            else
            {
                Print("Fail ReadingCVSFile", DATAFILE);
                Print(String.Format("Dalsze operacje na " + DATAFILE), "nie będą kontynuowane!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Funkcja pozwalajaca "wydobyc" pojedynczy rekord z danych
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public double[] sample(int f)
        {
            int l;
            switch (DataType)
            {
                case EnumDataTypes.HeartDisease:
                    l = 2; break;
                case EnumDataTypes.LetterRecognitionA:
                    l = 1; break;
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

        public double target(int f)
        {
            int l;
            switch (DataType)
            {
                case EnumDataTypes.HeartDisease:
                    l = 2; break;
                case EnumDataTypes.LetterRecognitionA:
                    l = 1; break;
                default:
                    Console.WriteLine("No pattern from set chosen! TODO in ZScore.class if you want to use it!");
                    l = 1; break;
            }

           
            return normalizedData[normalizedData.Length - l].GetByIndex(f);
        }

        /// <summary>
        /// Funkcja zwracajaca dane
        /// </summary>
        /// <returns>znormalizowana tablica danych</returns>
        public Column<double>[] NormalizedData()
        {
            return normalizedData;
        }
    }
}
