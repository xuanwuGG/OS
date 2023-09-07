using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace project
{
    internal class jobInThread
    {
        public Thread JobIn_Thread=null;
        public jobInThread()
        {
            JobIn_Thread = new Thread(Check);
            JobIn_Thread.Start();
        }   
        public static void Check()
        {
            while(true)
            {
                Program.jievent.WaitOne();
                Console.WriteLine("jobin线程正在判断是否有新的作业------");
                Thread.Sleep(1000);
                Console.WriteLine("好像莫得，继续！");
                Program.clevent.Set();
            }
        }
        public static void CheckJob()
        {
            Program.jievent.Set();
        }
    }
}
