using Microsoft.Win32;
using project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OS
{
    internal class partnerManager
    {
        public static List<LinkedList<List<int>>> freeAreas = new List<LinkedList<List<int>>>(5);
        public static List<LinkedList<List<int>>> allocatedAreas = new List<LinkedList<List<int>>>(5);
        public partnerManager()
        {
            freeAreas[4].AddFirst(new List<int>() { 1,16});
        }  
        public static void split(int level)
        {
            if (freeAreas[level].Count==0)
            {
                if (level==freeAreas.Count()) { Console.WriteLine("作业大小越界！"); throw new NotImplementedException();}
                else
                {
                    split(level + 1);
                }
            }
            List<int> tmp = freeAreas[level].First.Value;
            int len = (tmp[1] - tmp[0])/2;
            List<int> first = new List<int>() { { tmp[0] },{ (tmp[0] + len) } };
            List<int>second=new List<int>() { { (tmp[1] - len) }, (tmp[1]) };
            freeAreas[level-1].AddLast(first);
            freeAreas[level - 1].AddLast(second);
        }

        public static bool allocate(process t)
        {
            int i = 0;
            while (Math.Pow(2,i++)<t.requiredBlocks) { }
            i--;
            if (freeAreas[i].Count == 0) { split(i); }
            if (freeAreas[i].Count == 0) { return false; }
            allocatedAreas[i].AddLast(freeAreas[i].First.Value);
            freeAreas[i].RemoveFirst();
            return true;
        }
        public static void free(int startAddress)
        {
            int i = 10;
            foreach(var link in allocatedAreas)
            {
                if(link.FirstOrDefault)
            }
        }
    }
}
