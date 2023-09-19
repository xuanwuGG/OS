using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace project
{
    internal class jobInThread
    {
        public Thread JobIn_Thread=null;
        public static DateTime lastWriteTime=DateTime.MinValue;
        public static void arrangement()
        {
            DateTime lastwritetime = File.GetLastWriteTime(Program.filePath+"jobs-input.txt");
            if (lastwritetime != lastWriteTime)//判断文件是否更新,更新就进tmp队列
            {
                Console.WriteLine("作业请求更新，开始安排-----");
                StreamReader input = new StreamReader(Program.filePath + "jobs-input.txt");
                while (input.Peek() >= 0)
                {
                    string[] eachline = input.ReadLine().Split(',');
                    Program.tmpBackUpJob.Add(new process(int.Parse(eachline[0]), int.Parse(eachline[1]), int.Parse(eachline[2])));
                }
                lastWriteTime = lastwritetime;
                input.Close();

                Console.WriteLine("作业请求更新结束，结束安排-----");
            }
            else
            {
                Thread.Sleep(100);
                Console.WriteLine("作业请求未更新，结束安排-----");
            }

            try
            {
                clockThread.countlock.EnterReadLock();
                for (int i = 0; i < Program.tmpBackUpJob.Count; i++)
                {
                    process tmp = Program.tmpBackUpJob[i];
                    if (tmp.inTime <= clockThread.COUNTTIME)
                    {
                        Console.WriteLine("{0}号作业已经到达申请时间，进入后备队列!", tmp.jobsId);
                        Program.BackUpJob.Add(tmp);
                        Program.tmpBackUpJob.RemoveAt(i);
                        i = -1;
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Console.WriteLine("{0}号作业未到达申请时间，不能进入后备队列!", tmp.jobsId);
                        Thread.Sleep(100);
                    }
                }
                //读指令
                for (int i = 0; i < Program.BackUpJob.Count;i++)
                {
                    if (Program.BackUpJob[i].PSW == null)
                    {
                        process wo = Program.BackUpJob[i];
                        wo.PSW = "New";
                        StreamReader strucStream = new StreamReader(Program.filePath + wo.jobsId + ".txt");
                        List<int> p = new List<int>();
                        while (strucStream.Peek() >= 0)
                        {
                            var eachline = strucStream.ReadLine().Split(',');
                            int intTmp = int.Parse(eachline[1]);
                            p.Add(intTmp);
                        }
                        wo.instruct = p;
                        Console.WriteLine("成功读入{0}作业指令内容------", wo.jobsId);//当作业进入就绪队列后，获得作业指令
                    }
                }
            }
            finally
            {
                clockThread.countlock.ExitReadLock();
            }

            Thread.Sleep(100);
        }
        public jobInThread()
        {
            JobIn_Thread = new Thread(Check);
            JobIn_Thread.Start();
        }   
        public static void Check()
        {
            while(true)
            {
                Program.jievent.WaitOne();
                arrangement();
                Program.clevent.Set();
            }
        }
        public static void CheckJob()
        {
            Program.jievent.Set();
        }
    }
}
