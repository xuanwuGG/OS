using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OS
{
    internal class partnerSystem
    {
        public static List<freeArea> freeAreas=new List<freeArea>();
        public partnerSystem()
        {
            for (int i = 0; i < 5; i++)
            {
                freeArea tmpArea;
                if (i==4){ tmpArea = new freeArea();tmpArea.blockHead.AddFirst(new List<int>() { 1,16}); }
                tmpArea=new freeArea();
                freeAreas.Add(tmpArea);
            }
        }  
        public static void allocate(int size)
        {
            int i = 0;
            while ((Math.Pow(2, i++) < (size + 9) / 10)){ }//计算得出分配所需最小的块
            int minSize = i;
            if (freeAreas[4].blockHead.Count==1)//两种情况，第一个是整块内存空闲，则需要从最大内存块开始撕
            {
                i = 4;
                while (minSize != i--)
                {

                }
            }
            else//第二种情况则是需要从最小空闲块开始找
            {

            }
        }
    }
}
