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

        public outputBlock_thread() 
        {
            outBlock = new Thread(outblock);
            outBlock.Start();
        }
        public static void outblock()
        {
            while(true)
            {
                Program.outputLock.WaitOne();
                try
                {
                    inputBlock_thread.bufferLock.EnterReadLock();
                    Console.WriteLine(Program.buffer);
                    Program.buffer = "";
                    Console.WriteLine();
                }
                finally
                {
                    inputBlock_thread.bufferLock.ExitReadLock();
                }

                work tmpWork = processSchedulingThread.readyJob[0];
                tmpWork.jobStatus = "Block";
                processSchedulingThread.readyJob.RemoveAt(0);
                blockJobs2.Add(tmpWork);

                try
                {
                    clockThread.countlock.EnterUpgradeableReadLock();
                    try
                    {
                        clockThread.countlock.EnterWriteLock();
                        clockThread.COUNTTIME++;
                        Console.WriteLine("Tick tok:" + clockThread.COUNTTIME.ToString());
                        clockThread.COUNTTIME++;
                        Console.WriteLine("Tick tok:" + clockThread.COUNTTIME.ToString());
                    }
                    finally
                    {
                        clockThread.countlock.ExitWriteLock();
                    }
                }
                finally
                {
                    clockThread.countlock.ExitUpgradeableReadLock();
                }
                tmpWork = blockJobs2[0];
                blockJobs2.RemoveAt(0);
                tmpWork.jobStatus = "Ready";
                processSchedulingThread.readyJob.Add(tmpWork);

            }
        }
        public static void wake()
        {
            Program.outputLock.Set();
        }
    }
}
