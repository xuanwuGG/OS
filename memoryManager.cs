using project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS
{
    internal class memoryManager
    {
        public List<List<int>> allocatedTable;
        public List<List<int>> unallocatedTable;
        public memoryManager()
        {
            allocatedTable = new List<List<int>>();
            unallocatedTable = new List<List<int>>() { new List<int>() { 1,16} };
        }
        public bool allocate(process t)
        {
            for (int i = 0; i < unallocatedTable.Count; i++)
            {
                int tmpLen = unallocatedTable[i][1] - unallocatedTable[i][0];
                if (tmpLen > t.instructNum)
                {
                    t.sAddress = unallocatedTable[i][0];
                    allocatedTable.Add(new List<int>() { unallocatedTable[i][0], unallocatedTable[i][0] + t.instructNum - 1 });
                    unallocatedTable[i][0] = unallocatedTable[i][0]+t.instructNum;
                    unallocatedTable.OrderBy(m => m[0]).ToList();
                    allocatedTable.OrderBy(m => m[0]).ToList();
                    return true;
                }
                else if(tmpLen == t.instructNum)
                {
                    t.sAddress = unallocatedTable[i][0];
                    allocatedTable.Add(new List<int>() { unallocatedTable[i][0], unallocatedTable[i][1] });
                    unallocatedTable.RemoveAt(i);
                    unallocatedTable.OrderBy(m => m[0]).ToList();
                    allocatedTable.OrderBy(m => m[0]).ToList();
                    return true;
                }
            }
            Console.WriteLine("Do not have enough space!");
            return false;
        }
        public void free(int startAddress)
        {
            int leftSig = 0;//0表示该空间左边无作业，1则反之，right同理;
            int rightSig = 0;
            int endAddress=0;
            for (int i = 0;i < allocatedTable.Count; i++)
            {
                if (allocatedTable[i][0] == startAddress)
                {
                    endAddress = allocatedTable[i][1];
                    if (i > 0 && allocatedTable[i - 1][1]==startAddress-1) { leftSig++; }
                    if (i < allocatedTable.Count-1 && allocatedTable[i + 1][0]==endAddress+1) { rightSig++; }
                    allocatedTable.RemoveAt(i);
                    break;
                }
            }
            if (leftSig == 0)
            {
                for (int i = 0; i < unallocatedTable.Count; i++) 
                {
                    if (unallocatedTable[i][1] == startAddress-1) { startAddress = unallocatedTable[i][0]; unallocatedTable.RemoveAt(i); break; }
                }
            }
            if(rightSig == 0)
            {
                for (int i = 0; i < unallocatedTable.Count; i++)
                {
                    if (unallocatedTable[i][0] == endAddress + 1) { endAddress = unallocatedTable[i][1];unallocatedTable.RemoveAt(i); break; }
                }
            }
            unallocatedTable.Add(new List<int> { startAddress, endAddress });
            allocatedTable.OrderBy(t => t[0]).ToList();
            unallocatedTable.OrderBy(t => t[0]).ToList();
        }
    }
}
