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
        public static List<List<List<int>>> freeAreas = new List<List<List<int>>>();
        public static List<List<List<int>>> allocatedAreas = new List<List<List<int>>>();
        static int[] painting;
        public static int signal = 0;
        public partnerManager()
        {
            for(int i = 0; i < 5;i++)
            {
                List<List<int>>tmp1= new List<List<int>>();
                freeAreas.Add(tmp1);
                List<List<int>> tmp2 = new List<List<int>>();
                allocatedAreas.Add(tmp2);
            }
            freeAreas[4].Add(new List<int>() { 1,16});
        }
        public static void draw()
        {
            painting = new int[16];
            for (int a = 0; a < allocatedAreas.Count; a++)
            {
                for (int b = 0; b < allocatedAreas[a].Count; b++)
                {
                    for (int c = allocatedAreas[a][b][0] - 1; c < allocatedAreas[a][b][1]; c++)
                    {
                        painting[c] = 1;
                    }
                }
            }
        }

        public bool split(int level)
        {
            if(level>=5) { Console.WriteLine("暂时无可用空间！");return false; }
            else if (freeAreas[level].Count == 0) { split(level+1); }
            if(freeAreas[level].Count == 0) { return false; }
            else if (signal == level) { return true; }
            int len = (freeAreas[level][0][1] - freeAreas[level][0][0])/2;
            List<int>left= new List<int>() { freeAreas[level][0][0], freeAreas[level][0][0]+len };
            List<int> right = new List<int>() { freeAreas[level][0][1] -len, freeAreas[level][0][1]  };
            freeAreas[level-1].Add(left);
            freeAreas[level-1].Add(right);
            freeAreas[level].RemoveAt(0);
            return true;
        }
        public bool allocate(process t)
        {
            int i = 0;
            while (Math.Pow(2,i++)<t.requiredBlocks) { }
            i--;
            signal = i;
            if (freeAreas[i].Count == 0) { if (!split(i)) { return false; } }
            allocatedAreas[i].Add(freeAreas[i][0]);
            t.sAddress = freeAreas[i][0][0];
            freeAreas[i].RemoveAt(0);
            allocatedAreas[i]=allocatedAreas[i].OrderBy(a => a[0]).ToList();
            freeAreas[i]=freeAreas[i].OrderBy(a => a[0]).ToList();
            draw();
            return true;
        }
        public void merge(int level)
        {
            freeAreas[level] = freeAreas[level].OrderBy(a => a[0]).ToList();
            if (level >= 4) { return; }
            int sig = 0;
            for(int n = 0; n < freeAreas[level].Count-1;n++)
            {
                int leftsig = (int)Math.Ceiling((freeAreas[level][n][0] / Math.Pow(2, level + 1)));
                int rightsig = (int)Math.Ceiling((freeAreas[level][n+1][0] / Math.Pow(2, level + 1)));
                if (leftsig == rightsig) 
                { 
                    sig = 1;
                    List<int> tmp = new List<int>() { freeAreas[level][n][0], freeAreas[level][n + 1][1] };
                    freeAreas[level+1].Add(tmp);
                    freeAreas[level].RemoveAt(n);
                    freeAreas[level].RemoveAt(n);//删除后位置会前移一位
                    n = -1;
                }
            }
            if (sig == 1) { merge(level+1); }
        }
        public void free(process t)
        {
            int i = 0;
            while (Math.Pow(2, i++) < t.requiredBlocks) { }
            i--;
            var tmpblock = allocatedAreas[i].FirstOrDefault(a => a[0] == t.sAddress);
            allocatedAreas[i].Remove(tmpblock);
            freeAreas[i].Add(tmpblock);
            allocatedAreas[i]=allocatedAreas[i].OrderBy(a => a[0]).ToList();
            if(freeAreas[i].Count>1) { freeAreas[i] = freeAreas[i].OrderBy(a => a[0]).ToList(); }
            merge(i);
            draw();
        }
    }
}
