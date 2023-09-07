using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace project
{
    internal class Program
    {
        public static AutoResetEvent clevent = new AutoResetEvent(false);
        public static AutoResetEvent jievent = new AutoResetEvent(false);
        public static AutoResetEvent psevent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            //clockThread t1=new clockThread();
            //jobInThread t2=new jobInThread();
            //processSchedulingThread t3=new processSchedulingThread();
            List<work> worklist = new List<work>();
            worklist.Add(new work(2,9,3));
            worklist.Add(new work(3,8,1));
            worklist.Add(new work(3,7,1));
            worklist.Add(new work(3,5,1));
            worklist.Add(new work(3,3,1));
            worklist.Sort();
            foreach (work item in worklist)
            {
                Console.WriteLine(item.inTime.ToString());
            }
        }
    }
}
