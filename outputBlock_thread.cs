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

            }
        }
        public static void wake()
        {
            Program.outputLock.Set();
        }
    }
}
