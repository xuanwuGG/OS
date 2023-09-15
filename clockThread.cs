using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace project
{
    internal class clockThread
    {
        public static ReaderWriterLockSlim countlock = new ReaderWriterLockSlim();//对共享区COUNTTIME的互斥锁
        public static int COUNTTIME = 0;//时钟
        public Thread Clock_Thread = null;
        public clockThread()
        {
            Clock_Thread =new Thread(TIME_COUNT);
            Clock_Thread.Start();
        }
        public static void TIME_COUNT()
        {
            while(true)
            {
                Thread.Sleep(100);//时钟间隔
                try
                {
                    countlock.EnterUpgradeableReadLock();
                    try
                    {
                        countlock.EnterWriteLock();
                        COUNTTIME++;
                        Console.WriteLine("Tick tok:"+COUNTTIME.ToString());    
                    }
                    finally
                    {
                        countlock.ExitWriteLock();
                    }
                    if (COUNTTIME % 10 == 0)
                    {
                        jobInThread.CheckJob();
                        Program.clevent.WaitOne();
                    }
                    Console.WriteLine("ps进程解锁");
                    Thread.Sleep(300);
                    Program.psevent.Set();
                    Console.WriteLine("clock进程加锁");
                    Thread.Sleep(300);
                    Program.clevent.WaitOne();
                    Console.WriteLine("input进程解锁");
                    Thread.Sleep(300);
                    Program.inputLock.Set();
                    Console.WriteLine("clock进程加锁");
                    Thread.Sleep(300);
                    Program.clevent.WaitOne();
                    Console.WriteLine("output进程解锁");
                    Thread.Sleep(300);
                    Program.outputLock.Set();
                    Console.WriteLine("clock进程加锁");
                    Thread.Sleep(300);
                    Program.clevent.WaitOne();
                }
                finally
                {
                    countlock.ExitUpgradeableReadLock();
                }
            }
        }
    }
}
