using System.Collections.Generic;

namespace MyData
{
	public class DataSet
	{
		private List<double[]> data;
		
		private int count;
		public int Count { get { return count; } }
		
		private int lengthOfPattern;
		public int LengthOfPattern { get { return lengthOfPattern; } }
		
		public DataSet (string path)
		{
			data = DataRead.ReadData(path);
			count = data.Count;
			
			//nie ma w .Net2 data.First().Length
			lengthOfPattern = data[0].Length - 1;
		}
		
        /// <summary>
        /// zwraca pojedynczą próbkę danych
        /// </summary>
        /// <param name="index">nr, indeks próbki</param>
        /// <returns>pattern[]</returns>
		public double[] Pattern(int index)
		{
			double[] record = new double[data[index].Length - 1];

            for (int i = 0; i < record.Length; i++)
            {
                record[i] = data[index][i];
            }

            return record;
		}
		
        /// <summary>
        /// zwraca wartość docelową dla wskazanej próbki danych
        /// </summary>
        /// <param name="index">nr, indeks próbki</param>
        /// <returns>target</returns>
		public double Target(int index)
		{
			//return data[index].Last()	->	w .Net2.0 nie było jeszcze Last()
			return data[index][data[index].Length - 1];
		}
		
	}
}

