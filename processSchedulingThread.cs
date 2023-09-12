using OS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace project
{
    internal class processSchedulingThread
    {
        public static List<work> readyJob=new List<work>();
        public static List<List<work>>react=new List<List<work>>();

        public static int timeslice = 3;
        public Thread ProcessScheduling_thread=null;
        public static int rub = 0;
        public static work rubWork;
        public processSchedulingThread()
        {
            for(int i=0;i<3;i++)
            {
                List<work> tmp=new List<work>();
                react.Add(tmp);
            }
            ProcessScheduling_thread = new Thread(schedule);
            ProcessScheduling_thread.Start();
        }
        public static void schedule()
        {
            while (true) 
            {
                Program.psevent.WaitOne();
                try
                {
                    clockThread.countlock.EnterReadLock();
                    if(clockThread.COUNTTIME%10==0)
                    {
                        update(true);
                    }
                }
                finally
                {
                    clockThread.countlock.ExitReadLock();
                }
                ProcessScheduling(true);
                Program.clevent.Set();
                rub = 0;
            }
        }
        public static void ProcessScheduling(bool j)
        {
            if (j) 
            {
                if (react[0].Count + react[1].Count + react[2].Count==0)
                {
                    return;
                }
                if (readyJob.Count > 0 && readyJob[0].instruct[0]==1)//优先执行进行2指令的进程
                {
                    CPU.usingCpu(readyJob[0]);
                }
                else
                {
                    if (rub == 1&&readyJob.Count>0)
                    {
                        readyJob.RemoveRange(1, readyJob.Count-1);
                    }
                    else if(rub == 2)
                    {
                        readyJob[0].TIMES = timeslice;
                        readyJob.RemoveRange(0, readyJob.Count);
                    }
                    for(int a=0; a<3;a++)
                    {
                        for(int b=0; b<react[a].Count;b++)
                        {
                            readyJob.Add(react[a][b]);
                        }
                    }
                    CPU.usingCpu(readyJob[0]);
                }
                    
            }
            else
            {
                if (readyJob.Count != 0) { CPU.usingCpu(readyJob[0]); }
            }
        }
        public static void SetSchedulingTime(int num)
        {
            timeslice = num;
        }

        public static void update(bool j)//将处于后备队列作业进入就绪队列
        {
            for(int i=0;i<Program.BackUpJob.Count;)
            {
                work wo = Program.BackUpJob[i];
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
                Program.BackUpJob.Remove(wo);
                wo.jobStatus = "Ready";
                if (j)
                {
                    if (readyJob.Count > 0 && readyJob[0].priority > wo.priority) { rub = 2; }
                    else { rub = 1; }
                    react[wo.priority-1].Add(wo);
                }
                else
                {
                    readyJob.Add(wo);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
