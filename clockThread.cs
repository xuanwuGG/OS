using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
        public static List<string> content = new List<string>();
        public clockThread()
        {
            Clock_Thread = new Thread(TIME_COUNT);
            Clock_Thread.Start();
        }
        public static void TIME_COUNT()
        {
            FileStream fs = new FileStream(Program.filePath + "output.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            while (true)
            {
                Thread.Sleep(200);//时钟间隔
                try
                {
                    countlock.EnterUpgradeableReadLock();
                    try
                    {
                        countlock.EnterWriteLock();
                        COUNTTIME++;
                        Console.WriteLine("Tick tok:" + COUNTTIME.ToString());
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
                }
                finally
                {
                    countlock.ExitUpgradeableReadLock();
                }
                Program.psevent.Set();
                Program.clevent.WaitOne();
                if (COUNTTIME == 300)
                {
                    writer.Close();
                    fs.Close();
                }
                else if (COUNTTIME < 300)
                {
                    foreach (string s in content)
                    {
                        writer.WriteLine(s);
                    }
                    content.Clear();
                }
                else
                {
                    Console.ReadKey();
                }
            }
        }
    }
}
