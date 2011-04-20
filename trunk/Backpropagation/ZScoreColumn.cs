using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    class Column<T>
    {
        private List<T> cell;

        public Column()
        {
            cell = new List<T>();
        }

        public void AddData(T val)
        {
            cell.Add(val);
        }

        public List<T> GetColumn()
        {
            return cell;
        }

        public T[] ColumnToArray()
        {
            return cell.ToArray();
        }

        public T GetByIndex (int i)
        {
            return cell[i];
        }

        public T Get(int i)
        {
            return this.GetByIndex(i);
        }

        public int GetNum()
        {
            return cell.Count();
        }

        public void RemoveRange(int from, int to)
        {
            try
            {
                if (from <= to)
                    cell.RemoveRange(from, to);
            }
            catch (Exception ex)
            {
                Console.WriteLine("In RemoveRange: {0}", ex.Message);
            }
        }
    }

}
