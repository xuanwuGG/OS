using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS
{
    internal class bitMap
    {
        public Dictionary<int,int> allocatedTable;
        public void addAllocatedTable(int start,int end)
        {
            if (!allocatedTable.ContainsKey(start))
            {
                allocatedTable.Add(start, end);
            }
            else
            {
                Console.WriteLine("Do not have enough space");
            }
        }
        public void removeAllocatedTable(int start) 
        {
            allocatedTable.Remove(start);
        }
    }
}
