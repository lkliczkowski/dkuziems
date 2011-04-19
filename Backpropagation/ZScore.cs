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
            :this("exampleData.cvs", "Normalized.cvs", EnumDataTypes.HeartDisease)
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

                Column<float>[] discretizedData = Discretize
                    (rawData, dataType, columnType);

                //PrintList(discretizedData);

                normalizedData = new Column<float>
                    [GetNormalizeLength(columnType, dataType)];
                Normalize(ref normalizedData, discretizedData, dataType, columnType);

                Print(String.Format("ZScore on " + DATAFILE), "completed!");
                //PrintList(normalizedData);
                CSVwrite(OUTPUTFILE, normalizedData);
            }
            else
            {
                Print("Fail ReadingCVSFile", DATAFILE);
                Print(String.Format("Working on " + DATAFILE), "will not continue!");
            }
        }

        public Column<float>[] NormalizedData()
        {
            return normalizedData;
        }

        public float[] SingleRecord(int f)
        {
            float[] record = new float[normalizedData.Length];

            for (int i = 0; i < record.Length; i++)
            {
                record[i] = normalizedData[i].GetByIndex(f);
            }

            return record;
        }
    }
}
