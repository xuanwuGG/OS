using OS;
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
        public static List<string> content = new List<string>();
        public static List<string> content1 = new List<string>();
        public static List<string> content2 = new List<string>();
        public static AutoResetEvent clevent = new AutoResetEvent(false);
        public static ManualResetEvent button=new ManualResetEvent(true);
        public static int endCount = 0;
        public static bool signal = false;
        public static int addNum;
        public static List<process> endJobs=new List<process>();
        public static int getTime() { return COUNTTIME; }
        public clockThread()
        {
            Clock_Thread = new Thread(TIME_COUNT);
            Clock_Thread.Start();
        }
        public static void TIME_COUNT()
        {
            FileStream fs = new FileStream("D:/output2/tmp.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            clevent.WaitOne();
            while (true)
            {
                button.WaitOne();
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
                if (COUNTTIME > 20 && Program.tmpBackUpJob.Count == 0 && inputBlock_thread.blockJobs1.Count == 0 && outputBlock_thread.blockJobs2.Count == 0 && processSchedulingThread.readyJob[0].Count == 0 && processSchedulingThread.readyJob[1].Count == 0 && processSchedulingThread.readyJob[2].Count == 0 && processSchedulingThread.readyJob[3].Count == 0)
                {
                    if (endCount++ == 0)
                    {
                        string s = "\n";
                        writer.Write(s);
                        foreach(var i in endJobs)
                        {
                            s = i.endTime + ":["+i.jobsId+":"+i.inTime+","+i.enterTime+","+i.runTime+"]"+"\n";
                            writer.Write(s);
                        }
                        if (CPU.bb1.EndsWith("/")) { CPU.bb1=CPU.bb1.Substring(0,CPU.bb1.Length-1); }
                        if (CPU.bb2.EndsWith("/")) { CPU.bb2 = CPU.bb2.Substring(0, CPU.bb2.Length - 1); }
                        CPU.bb1 += "]"+"\n";
                        CPU.bb2 += "]"+"\n";
                        writer.Write(CPU.bb1);
                        writer.Write(CPU.bb2);
                        writer.Close();
                        fs.Close();
                        string oldFilePath = "D:/output2/tmp.txt";
                        string newFilePath = "D:/output2/ProcessResults-" + COUNTTIME + "-DJFK.txt";
                        File.Move(oldFilePath, newFilePath);
                    }
                    else if (endCount == 5) { button.Reset(); button.WaitOne(); }
                }
                else
                {
                    foreach (string s in content)
                    {
                        writer.WriteLine(s);
                    }
                    content.Clear();
                }
                if (signal)
                {
                    addJobs.add(addNum);
                    signal = false;
                }
                Thread.Sleep(1000);//时钟间隔
                content1.Clear();
                content2.Clear();
            }
        }
    }
}
