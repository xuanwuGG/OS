using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OS
{
    internal class physicalManager
    {
        public int[] physicalSpace;
        public physicalManager() { physicalSpace = new int[16]; }

        public void allocate(int blockNum)
        {
            physicalSpace[blockNum] = 1;
        }
        public void free(int blockNum)
        {
            physicalSpace[blockNum] = 0;

        }
    }
}
