using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS
{
    internal class freeArea
    {
<<<<<<< HEAD
        public LinkedList<List<int>> blockHead=new LinkedList<List<int>>();
        public freeArea() { }
        public bool isEmpty()
        {
            IEnumerator<List<int>> e=blockHead.GetEnumerator();
            if(e.MoveNext()) { return true; }
            else { return false; }
        }
        public void move()
        {

=======
        public int num = 0;
        public LinkedList<int> page=new LinkedList<int>();
        public void addBlock()
        {
>>>>>>> f6001d6 (temp)
        }
    }
}
