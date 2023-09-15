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
                Program.clevent.Set();
                Program.psevent.WaitOne();
                Program.clevent.Set();
                Program.psevent.WaitOne();
            }
        }
        public static void wake()
        {
            Program.inputLock.Set();
        }
    }
}
