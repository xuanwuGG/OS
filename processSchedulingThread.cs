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

        public static int timeslice = 3;
        public Thread ProcessScheduling_thread=null;
        public processSchedulingThread()
        {
            ProcessScheduling_thread = new Thread(schedule);
            ProcessScheduling_thread.Start();
        }
        public static void schedule()
        {
            while (true) 
            {
                Program.psevent.WaitOne();
                update();
                if(readyJob.Count != 0) { CPU.usingCpu(readyJob[0]);}
                Program.clevent.Set();
            }
        }
        public static void ProcessScheduling(bool j)
        {
            if(j) 
            {
                
            }
            else
            {
                readyJob.Sort();
                Console.WriteLine("当前就绪队列如下：");
                foreach (var work in readyJob) { 
                    Console.Write(work.jobsId+" ");
                }
                Console.WriteLine();
            }
        }
        public static void SetSchedulingTime(int num)
        {
            timeslice = num;
        }

        public static void update()
        {
            foreach(var wo in Program.BackUpJob)
            {
                if(wo!=null&&wo.jobStatus.Equals("New"))
                {
                    StreamReader strucStream = new StreamReader(Program.filePath + wo.jobsId + ".txt");
                    List<int> p=new List<int> ();
                    while (strucStream.Peek() >= 0)
                    {
                        var eachline = strucStream.ReadLine().Split(',');
                        int intTmp = int.Parse(eachline[1]);
                        p.Add(intTmp);
                    }
                    wo.instruct = p;
                    Console.WriteLine("成功读入{0}作业指令内容------", wo.jobsId);//当作业进入就绪队列后，获得作业指令
                    wo.jobStatus = "Ready";
                    readyJob.Add(wo);
                    foreach(var i in wo.instruct) { Console.Write(i+" "); Thread.Sleep(200); }
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
