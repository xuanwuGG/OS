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
        public partnerManager()
        {
            freeAreas[4].AddFirst(new List<int>() { 1,16});
        }  
        public void split(int point)
        {
            if (freeAreas[point].Count==0)
            {
                split(point+1);
            }
        }
        public static void allocate(process t)
        {
            int i = 0;
            while (Math.Pow(2,i++)<t.requiredBlocks) { }
            i--;
            for (; i < 5; i++)
            {

            }
        }
    }
}
