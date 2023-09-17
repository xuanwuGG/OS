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
        public static List<process> blockJobs1=new List<process>();
        public static int count = 3;

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
                        process tmpWork = blockJobs1[0];
                        blockJobs1.RemoveAt(0);
                        Console.WriteLine("2秒时间到，{0}进程回归{1}队列", tmpWork.jobsId, tmpWork.queueNum);
                        processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                        count = 3;
                    }
                }
                Program.outputLock.Set();
            }
        }
    }
}
