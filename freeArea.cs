using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS
{
    internal class freeArea
    {
        public int blockNum = 0;
        public LinkedList<List<int>> blockHead=new LinkedList<List<int>>();
        public freeArea(int x){blockNum = x;}
        public freeArea() { }
        public bool isEmpty()
        {
            IEnumerator<List<int>> e=blockHead.GetEnumerator();
            if(e.MoveNext()) { return true; }
            else { return false; }
        }
        public void move()
        {

        }
    }
}
