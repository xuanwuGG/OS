using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace project
{
    public class jobInThread
    {
        public Thread JobIn_Thread = null;
        public static DateTime lastWriteTime = DateTime.MinValue;
        public static List<process> BackUpJob = new List<process>();
        public static void arrangement()
        {
            DateTime lastwritetime = File.GetLastWriteTime(Program.filePath + "jobs-input.txt");
            if (lastwritetime != lastWriteTime)//判断文件是否更新,更新就进tmp队列
            {
                StreamReader input = new StreamReader(Program.filePath + "jobs-input.txt");
                while (input.Peek() >= 0)
                {
                    string[] eachline = input.ReadLine().Split(',');
                    Program.tmpBackUpJob.Add(new process(int.Parse(eachline[0]), int.Parse(eachline[1]), int.Parse(eachline[2])));
                    clockThread.content1.Add(clockThread.COUNTTIME + ":[新增作业:" + int.Parse(eachline[0]) + "," + int.Parse(eachline[1]) + "," + int.Parse(eachline[2]) + "]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[新增作业:" + int.Parse(eachline[0]) + "," + int.Parse(eachline[1]) + "," + int.Parse(eachline[2]) + "]");
                }
                lastWriteTime = lastwritetime;
                input.Close();

            }

            try
            {
                clockThread.countlock.EnterReadLock();
                for (int i = 0; i < Program.tmpBackUpJob.Count; i++)
                {
                    process tmp = Program.tmpBackUpJob[i];
                    if (tmp.inTime <= clockThread.COUNTTIME)
                    {
                        BackUpJob.Add(tmp);
                        Program.tmpBackUpJob.RemoveAt(i);
                        i = -1;
                    }
                }
                //读指令
                for (int i = 0; i < BackUpJob.Count;)
                {
                    if (BackUpJob[i].PSW == null)
                    {
                        process wo = BackUpJob[i];
                        wo.PSW = "New";
                        StreamReader strucStream = new StreamReader(Program.filePath + wo.jobsId + ".txt");
                        List<int> p = new List<int>();
                        while (strucStream.Peek() >= 0)
                        {
                            var eachline = strucStream.ReadLine().Split(',');
                            int intTmp = int.Parse(eachline[1]);
                            p.Add(intTmp);
                        }
                        wo.instructionRegister = p;
                        processSchedulingThread.push(BackUpJob[0]);
                    }
                }
            }
            finally
            {
                clockThread.countlock.ExitReadLock();
            }

        }
        public jobInThread()
        {
            JobIn_Thread = new Thread(Check);
            JobIn_Thread.Start();
        }
        public static void Check()
        {
            while (true)
            {
                Program.jievent.WaitOne();
                arrangement();
                clockThread.clevent.Set();
            }
        }
        public static void CheckJob()
        {
            Program.jievent.Set();
        }
    }
}
