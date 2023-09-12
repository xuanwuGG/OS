﻿using project;
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
                try
                {
                    bufferLock.EnterUpgradeableReadLock();
                    try
                    {
                        bufferLock.EnterWriteLock();
                        Program.buffer = "This is a message from buffer";
                    }
                    finally { bufferLock.ExitWriteLock(); }
                }
                finally { bufferLock.ExitUpgradeableReadLock(); }


                work tmpWork = processSchedulingThread.readyJob[0];
                tmpWork.jobStatus = "Block";
                blockJobs1.Add(tmpWork);
                processSchedulingThread.readyJob.RemoveAt(0);

                try
                {
                    clockThread.countlock.EnterUpgradeableReadLock();
                    try
                    {
                        clockThread.countlock.EnterWriteLock();
                        Thread.Sleep(1000);
                        clockThread.COUNTTIME++;
                        Console.WriteLine("Tick tok:" + clockThread.COUNTTIME.ToString());
                        Thread.Sleep(1000);
                        clockThread.COUNTTIME++;
                        Console.WriteLine("Tick tok:" + clockThread.COUNTTIME.ToString());
                    }
                    finally{clockThread.countlock.ExitWriteLock();}
                }
                finally{clockThread.countlock.ExitUpgradeableReadLock();}

                tmpWork = blockJobs1[0];
                blockJobs1.RemoveAt(0);
                tmpWork.jobStatus = "Ready";
                processSchedulingThread.readyJob.Add(tmpWork);
            }
        }
        public static void wake()
        {
            Program.inputLock.Set();
        }
    }
}
