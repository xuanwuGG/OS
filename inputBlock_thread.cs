using project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS
{
    internal class inputBlock_thread
    {
        public static ReaderWriterLockSlim bufferLock=new ReaderWriterLockSlim();
        public static List<work> blockJobs1=new List<work>();
        public static int count = 2;

        Thread inputBlock;
        public inputBlock_thread() 
        {
            inputBlock = new Thread(inputlock);
            inputBlock.Start();
        }
        public static void inputlock()
        {
            while(true)
            {
                Program.inputLock.WaitOne();
                Thread.Sleep(1000);
                if (blockJobs1.Count!=0) 
                {
                    count--;
                    if(count==0)
                    {
                        try
                        {
                            bufferLock.EnterUpgradeableReadLock();
                            try
                            {
                                bufferLock.EnterWriteLock();
                                Program.buffer = "This is a massage from buffer";
                            }
                            finally{bufferLock.ExitWriteLock();}
                        }
                        finally { bufferLock.ExitUpgradeableReadLock();}
                        work tmpWork = blockJobs1[0];
                        blockJobs1.RemoveAt(0);
                        processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                    }
                }
                else{count = 2;}
                Console.WriteLine("input结束，clock进程解锁");
                Thread.Sleep(300);
                Program.clevent.Set();
            }
        }
        public static void wake()
        {
            Program.inputLock.Set();
        }
    }
}
