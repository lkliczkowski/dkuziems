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
        private EnumDataTypes dataType;
        private int[] columnType;
        private Column<float>[] normalizedData;
        public Column<float>[] Data
        {
            get { return normalizedData; }
        }

        //TODO: example data
        public ZScore()
            : this("ExampleData.csv", "normalizedExample.csv", EnumDataTypes.HeartDisease)
        {}

        public ZScore(string f1, string f2, EnumDataTypes dt)
        {
            DATAFILE = f1;
            OUTPUTFILE = f2;
            dataType = dt;
            InitData();
        }

        private void InitData()
        {
            switch (dataType)
            {
                case EnumDataTypes.unknown:
                    break;
                case EnumDataTypes.HeartDisease:
                    columnType = ColumnTypes.HeartDisease;
                    break;
            }
        }

        public void NormalizeHeartDisease()
        {
            Column<string>[] rawData = new Column<string>[columnType.Length];
            for (int i = 0; i < columnType.Length; i++)
                rawData[i] = new Column<string>();

            if (ZScore.CSVread(DATAFILE, ref rawData))
            {
                //Console.WriteLine(rawData.Length.ToString());
                RemoveFromRecords(ref rawData, 0, 2);

                Console.WriteLine(">> DataSetSize {0}:{1}", rawData.Length, rawData[0].GetNum());


                Column<float>[] discretizedData = Discretize
                    (rawData, dataType, columnType);

                //PrintList(discretizedData);

                normalizedData = new Column<float>
                    [GetNormalizeLength(columnType, dataType)];
                normalize(ref normalizedData, discretizedData, dataType, columnType);

                Print(String.Format("ZScore on " + DATAFILE), "completed!");

                Print(String.Format("DataSetSize \t{0}:{1}", rawData.Length, rawData[0].GetNum()));
                Print(String.Format("NormalizedSetSize\t{0}:{1}", normalizedData.Length, normalizedData[0].GetNum()));

                //PrintList(normalizedData);
                CSVwrite(OUTPUTFILE, normalizedData);
            }
            else
            {
                Print("Fail ReadingCVSFile", DATAFILE);
                Print(String.Format("Working on " + DATAFILE), "will not continue!");
            }
        }

        /// <summary>
        /// Funkcja pozwalajaca "wydobyc" pojedynczy rekord z danych
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public float[] SingleRecord(int f)
        {
            float[] record = new float[normalizedData.Length];

            for (int i = 0; i < record.Length; i++)
            {
                record[i] = normalizedData[i].GetByIndex(f);
            }

            return record;
        }

        /// <summary>
        /// Funkcja zwracajaca dane
        /// </summary>
        /// <returns>znormalizowana tablica danych</returns>
        public Column<float>[] NormalizedData()
        {
            return normalizedData;
        }
    }
}
