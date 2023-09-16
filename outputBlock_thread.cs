using project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS
{
    internal class outputBlock_thread
    {
        Thread outBlock;
        public static List<work> blockJobs2 = new List<work>();
        public static int count = 2;
        public outputBlock_thread() 
        {
            outBlock = new Thread(outblock);
            outBlock.Start();
        }
        public static void outblock()
        {
            while (true)
            {
                Program.outputLock.WaitOne();
                if (blockJobs2.Count != 0)
                {
                    count--;
                    if (count == 0)
                    {
                        try
                        {
                            inputBlock_thread.bufferLock.EnterUpgradeableReadLock();
                            try
                            {
                                inputBlock_thread.bufferLock.EnterWriteLock();
                                Console.WriteLine(Program.buffer);
                                Program.buffer = "";
                            }
                            finally { inputBlock_thread.bufferLock.ExitWriteLock(); }
                        }
                        finally {inputBlock_thread.bufferLock.ExitUpgradeableReadLock(); }
                        work tmpWork = blockJobs2[0];
                        blockJobs2.RemoveAt(0);
                        processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                        CPU.CPU_REC(tmpWork);
                        count = 2;
                    }
                }
                Program.clevent.Set();
            }
        }
    }
}
