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
        public static List<List<work>>readyJob=new List<List<work>>();

        public static int timeslice = 3;
        public Thread ProcessScheduling_thread=null;
        public static int rub = 0;
        public static bool algorithm = false;
        public processSchedulingThread()
        {
            for(int i=0;i<4;i++)
            {
                List<work> tmp=new List<work>();
                readyJob.Add(tmp);
            }
            ProcessScheduling_thread = new Thread(schedule);
            ProcessScheduling_thread.Start();
        }
        public static void schedule()
        {
            while (true) 
            {
                Program.psevent.WaitOne();
                update(algorithm);
                ProcessScheduling(algorithm);
                Program.clevent.Set();
                rub = 0;
            }
        }
        public static void ProcessScheduling(bool j)
        {
            if (j) 
            {
                if(rub==1)
                {
                    for(int i=1;i<3;i++)
                    {
                        if (readyJob[i].Count!=0)
                        {
                            if (readyJob[i][0].instruct[0] ==1)
                            {
                                Console.WriteLine("{0}将执行2指令，应优先执行");
                                Thread.Sleep(1000);
                                CPU.usingCpu(readyJob[i][0]);
                            }
                            else
                            {
                                CPU.usingCpu(readyJob[0][0]);
                            }
                            return;
                        }
                    }
                }
                else
                {
                    for(int i=0;i<3;i++)
                    {
                        if (readyJob[i].Count != 0)
                        {
                            CPU.usingCpu(readyJob[i][0]);
                            return;
                        }
                    }
                }
            }
            else
            {
                if (readyJob[0].Count != 0) { CPU.usingCpu(readyJob[0][0]); }
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
                Thread.Sleep(200);
                Program.BackUpJob.Remove(wo);
                wo.jobStatus = "Ready";
                if (j) { wo.isReflect=true; }
                if (readyJob[0].Count == 0) { rub = 1; }
                readyJob[0].Add(wo);
                Thread.Sleep(1000);
            }
        }
    }
}
