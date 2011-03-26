using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZScore
{
    class Records<T>
    {
        private List<T> record;

        public Records()
        {
            record = new List<T>();
        }

        public void AddRecord(T val)
        {
            record.Add(val);
        }

        public List<T> GetRecord()
        {
            return record;
        }

        public T[] RecordTable()
        {
            return record.ToArray();
        }

        public T GetByIndex (int i)
        {
            return record[i];
        }

        public T Get(int i)
        {
            return this.GetByIndex(i);
        }

        public int GetNum()
        {
            return record.Count();
        }

        public void RemoveRange(int from, int to)
        {
            if(from <= to)
                record.RemoveRange(from, to);
        }
    }

}
