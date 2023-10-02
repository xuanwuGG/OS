using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project
{
    public class clockThread
    {
        public static ReaderWriterLockSlim countlock = new ReaderWriterLockSlim();//对共享区COUNTTIME的互斥锁
        public static int COUNTTIME = 0;//时钟
        public Thread Clock_Thread = null;
        public static List<string> content1 = new List<string>();
        public static List<string> content2 = new List<string>();
        public static AutoResetEvent clevent = new AutoResetEvent(true);
        public static ManualResetEvent button=new ManualResetEvent(true);

        public static int getTime() { return COUNTTIME; }
        public clockThread()
        {
            Clock_Thread = new Thread(TIME_COUNT);
            Clock_Thread.Start();
        }

        public static void TIME_COUNT()
        {
            //FileStream fs = new FileStream(Program.filePath + "output.txt", FileMode.Create);
            //StreamWriter writer = new StreamWriter(fs);
            clevent.WaitOne();
            while (true)
            {
                //button.WaitOne();
                try
                {
                    countlock.EnterUpgradeableReadLock();
                    try
                    {
                        countlock.EnterWriteLock();
                        COUNTTIME++;
                    }
                    finally
                    {
                        countlock.ExitWriteLock();
                    }
                    if (COUNTTIME % 10 == 0)
                    {
                        jobInThread.CheckJob();
                        clevent.WaitOne();
                    }
                }
                finally
                {
                    countlock.ExitUpgradeableReadLock();
                }
                Program.psevent.Set();
                clockThread.clevent.WaitOne();
                Thread.Sleep(1000);//时钟间隔
                content.Clear();
                //if (COUNTTIME == 300)
                //{
                //    writer.Close();
                //    fs.Close();
                //}
                //else if (COUNTTIME < 300)
                //{
                //    foreach (string s in content)
                //    {
                //        writer.WriteLine(s);
                //    }
                //    content.Clear();
                //}
                //else
                //{
                //    Console.ReadKey();
                //}
            }
        }
    }
}
