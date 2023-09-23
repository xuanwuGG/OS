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
        public static List<List<process>> readyJob = new List<List<process>>();

        public static int timeslice = 3;
        public Thread ProcessScheduling_thread = null;
        public static int rub = 0;
        public static bool algorithm = true;
        public processSchedulingThread()
        {
            for (int i = 0; i < 4; i++)
            {
                List<process> tmp = new List<process>();
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
                ProcessScheduling(algorithm);
                rub = 0;
                Program.inputLock.Set();
                Program.outputLock.Set();
            }
        }
        public static void ProcessScheduling(bool j)
        {
            if (j) //算法判断
            {
                if (rub == 1)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        if (readyJob[i].Count != 0)
                        {
                            if (readyJob[i][0].instructionRegister[0] == 1)
                            {
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
                    for (int i = 0; i < 4; i++)
                    {
                        if (readyJob[i].Count != 0)
                        {
                            CPU.usingCpu(readyJob[i][0]);
                            return;
                        }
                    }
                    Console.WriteLine("[CPU 空闲]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[CPU 空闲]");
                }
            }
            else
            {
                if (readyJob[0].Count != 0) { CPU.usingCpu(readyJob[0][0]); }
                else
                {
                    Console.WriteLine("[CPU 空闲]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[CPU 空闲]");
                }
            }
        }
        public static void push(process t)
        {
            if (Program.manager.allocate(t))
            {
                Program.BackUpJob.Remove(t);
                t.PSW = "Ready";
                Console.WriteLine("[创建进程:进程 ID:{0},内存块始地址:{1},内存分配方式:First Fit]", t.jobsId, t.sAddress);

                t.inTime = clockThread.COUNTTIME;
                clockThread.content.Add(clockThread.COUNTTIME + ":[创建进程:进程 ID:" + t.jobsId + ",内存块始地址:" + t.sAddress + ",内存分配方式:First Fit]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[进入就绪队列:进程 ID:" + t.jobsId + ",待执行的指令数:" + t.instructionRegister.Count() + "]");
                if (algorithm && readyJob[0].Count == 0 && (readyJob[1].Count + readyJob[2].Count + readyJob[3].Count)!=0) { rub = 1; }
                processSchedulingThread.readyJob[0].Add(t);
                Console.WriteLine("[进入就绪队列:进程 ID:{0},待执行的指令数:{1}]", t.jobsId, t.instructionRegister.Count());
            }
        }
        public static void SetSchedulingTime(int num)
        {
            timeslice = num;
        }
    }
}
