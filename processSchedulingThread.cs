using OS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace project
{
    public class processSchedulingThread
    {
        public static List<List<process>> readyJob = new List<List<process>>();

        public static int timeslice = 3;
        public Thread ProcessScheduling_thread = null;
        public static int rub = 0;
        public static bool algorithm = true;
        public static int runningjob = 0;
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
                                runningjob = readyJob[i][0].jobsId;
                                CPU.usingCpu(readyJob[i][0]);
                            }
                            else
                            {
                                runningjob = readyJob[0][0].jobsId;
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
                            runningjob = readyJob[i][0].jobsId;
                            CPU.usingCpu(readyJob[i][0]);
                            return;
                        }
                    }
                    clockThread.content1.Add(clockThread.COUNTTIME + ":[CPU 空闲]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[CPU 空闲]");
                }
            }
            else
            {
                if (readyJob[0].Count != 0) { CPU.usingCpu(readyJob[0][0]); runningjob = readyJob[0][0].jobsId;}
                else
                {
                    clockThread.content1.Add(clockThread.COUNTTIME + ":[CPU 空闲]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[CPU 空闲]");
                }
            }
        }
        public static void push(process t)
        {
            if(Program.partnersystem)
            {
                if (Program.partnermanager.allocate(t))
                {
                    jobInThread.BackUpJob.Remove(t);
                    t.PSW = "Ready";
                    t.inTime = clockThread.COUNTTIME;
                    clockThread.content1.Add(clockThread.COUNTTIME + ":[创建进程:" + t.jobsId + "," + ((t.sAddress - 1) * 1000 + 5000) + ",PartnerSystem]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[创建进程:" + t.jobsId + "," + ((t.sAddress - 1) * 1000 + 5000) + ",PartnerSystem]");
                    clockThread.content1.Add(clockThread.COUNTTIME + ":[进入就绪队列:" + t.jobsId + "," + t.instructionRegister.Count() + "]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[进入就绪队列:" + t.jobsId + "," + t.instructionRegister.Count() + "]");
                    if (algorithm && readyJob[0].Count == 0 && (readyJob[1].Count + readyJob[2].Count + readyJob[3].Count) != 0) { rub = 1; }
                    processSchedulingThread.readyJob[0].Add(t);
                }
            }
            else
            {
                if (Program.manager.allocate(t))
                {
                    jobInThread.BackUpJob.Remove(t);
                    t.PSW = "Ready";
                    t.inTime = clockThread.COUNTTIME;
                    clockThread.content1.Add(clockThread.COUNTTIME + ":[创建进程:" + t.jobsId + "," + ((t.sAddress - 1) * 1000 + 5000) + ",First Fit]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[创建进程:" + t.jobsId + "," + ((t.sAddress - 1) * 1000 + 5000) + ",First Fit]");
                    clockThread.content1.Add(clockThread.COUNTTIME + ":[进入就绪队列:" + t.jobsId + "," + t.instructionRegister.Count() + "]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[进入就绪队列:" + t.jobsId + "," + t.instructionRegister.Count() + "]");
                    if (algorithm && readyJob[0].Count == 0 && (readyJob[1].Count + readyJob[2].Count + readyJob[3].Count) != 0) { rub = 1; }
                    processSchedulingThread.readyJob[0].Add(t);
                }
            }
        }
        public static void SetSchedulingTime(int num)
        {
            timeslice = num;
        }
    }
}
