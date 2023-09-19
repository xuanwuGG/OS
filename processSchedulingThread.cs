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
        public static List<List<process>>readyJob=new List<List<process>>();

        public static int timeslice = 3;
        public Thread ProcessScheduling_thread=null;
        public static int rub = 0;
        public static bool algorithm = true;
        public processSchedulingThread()
        {
            for(int i=0;i<4;i++)
            {
                List<process> tmp=new List<process>();
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
                if (Program.BackUpJob.Count != 0) { push(Program.BackUpJob[0]);}
                ProcessScheduling(algorithm);
                rub = 0;
                Program.inputLock.Set();
            }
        }
        public static void ProcessScheduling(bool j)
        {
            if (j) //算法判断
            {
                if(rub==1)
                {
                    for(int i=1;i<4;i++)
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
                    for(int i=0;i<4;i++)
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
        public static void push(process t)
        {
            if (Program.manager.allocate(t))
            {
                Program.BackUpJob.Remove(t);
                t.PSW = "Ready";
                if (algorithm && readyJob[0].Count == 0) { rub = 1; }
                processSchedulingThread.readyJob[0].Add(t);
            }
        }
        public static void SetSchedulingTime(int num)
        {
            timeslice = num;
        }
    }
}
