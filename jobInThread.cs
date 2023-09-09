using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                    Program.tmpBackUpJob.Add(new work(int.Parse(eachline[0]), int.Parse(eachline[1]), int.Parse(eachline[2]), int.Parse(eachline[3])));
                }
                lastWriteTime = lastwritetime;
                input.Close();
                Thread.Sleep(100);
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
                    work tmp = Program.tmpBackUpJob[i];
                    if (tmp.inTime <= clockThread.COUNTTIME)
                    {
                        Console.WriteLine("{0}号作业已经到达申请时间，进入后备队列!", tmp.jobsId);
                        tmp.jobStatus = "New";
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
            }
            finally
            {
                clockThread.countlock.ExitReadLock();
            }


            //Console.WriteLine("当前作业时间到达时间顺序为-----");
            //foreach (var tmp in Program.BackUpJob)
            //{
            //    Console.WriteLine(tmp.inTime.ToString());
            //}
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
