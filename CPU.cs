using project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS
{
    internal class CPU
    {
        public int ProgramCounter = 0;
        public static string bb1 = "BB1[阻塞队列1,键盘输入:";
        public static string bb2 = "BB2[阻塞队列2,屏幕显示:";
        public static int MMU(process t) { return (t.sAddress-1)*1000+5000+ (t.programCounter) * 100; }
        public CPU() { }
        public static void usingCpu(process t)
        {
            if (t.instructionRegister.Count== t.programCounter)
            {
                clockThread.content1.Add(clockThread.COUNTTIME + "[终止进程:"+ t.jobsId + "]");
                clockThread.content.Add(clockThread.COUNTTIME + "[终止进程:" + t.jobsId + "]");
                t.endTime = clockThread.COUNTTIME;
                t.runTime = t.endTime - t.enterTime;
                if (Program.partnersystem) { Program.partnermanager.free(t); }
                else { Program.manager.free(t); }
                processSchedulingThread.readyJob[t.queueNum].RemoveAt(0);
                clockThread.endJobs.Add(t);
                processSchedulingThread.rub = 0;//刷新rub标志
                if (jobInThread.BackUpJob.Count != 0) { processSchedulingThread.push(jobInThread.BackUpJob[0]); }
                processSchedulingThread.ProcessScheduling(processSchedulingThread.algorithm);
                return;
            }
            if (t.TIMES == 1 && t.instructionRegister[t.programCounter] == 1 && t.instr1Count == 0)
            {
                timeUp(t);
                return;
            }
            else if (t.TIMES <= 0)
            {
                timeUp(t);
                return;
            }
            t.PSW = "Running";
            clockThread.content1.Add(clockThread.COUNTTIME + ":[运行进程:" + t.jobsId + "," + t.instructionRegister[t.programCounter] + ","+ ((t.programCounter)*100)+ "," + MMU(t) + "]");
            clockThread.content.Add(clockThread.COUNTTIME + ":[运行进程:" + t.jobsId + "," + t.instructionRegister[t.programCounter] + ","+ ((t.programCounter)*100)+ "," + MMU(t) + "]");
            if (t.instructionRegister[t.programCounter] == 0)
            {
                t.programCounter++;
                t.TIMES -= 1;
            }
            else if (t.instructionRegister[t.programCounter] == 1)
            {
                t.instr1Count++;
                t.TIMES--;
                if (t.instr1Count == 2)
                {
                    t.programCounter++;
                    t.instr1Count = 0;
                }
            }
            else if (t.instructionRegister[t.programCounter] == 2)
            {
                t.programCounter++;
                bb1 += clockThread.COUNTTIME+","+t.jobsId+"/";
                CPU_PRO(t, 2);
                return;
            }
            else
            {
                t.programCounter++;
                bb2 += clockThread.COUNTTIME + "," + t.jobsId + "/";
                CPU_PRO(t, 3);
                return;
            }
            t.PSW = "Ready";
        }
        public static void CPU_PRO(process t, int sig)
        {
            t.PSW = "Block";
            processSchedulingThread.readyJob[t.queueNum].RemoveAt(0);
            if (sig == 2)
            {
                inputBlock_thread.blockJobs1.Add(t);
                clockThread.content1.Add(clockThread.COUNTTIME + ":[阻塞进程:inputBlock," + t.jobsId + "]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[阻塞进程:inputBlock," + t.jobsId + "]");
            }
            else
            {
                outputBlock_thread.blockJobs2.Add(t);
                clockThread.content1.Add(clockThread.COUNTTIME + ":[阻塞进程:outputBlock," + t.jobsId + "]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[阻塞进程:outputBlock," + t.jobsId + "]");
            }

        }
        public static void CPU_REC(process t)
        {
            t.PSW = "Ready";
            if (t.queueNum == 3)
            {
                t.TIMES = 99;
            }
        }
        public static void timeUp(process t)
        {
            t.PSW = "Ready";
            if (processSchedulingThread.algorithm)
            {
                if (t.queueNum == 3)
                {
                    t.TIMES = 99;
                    usingCpu(t);
                }
                else
                {
                    processSchedulingThread.readyJob[t.queueNum].RemoveAt(0);
                    t.queueNum++;
                    processSchedulingThread.readyJob[t.queueNum].Add(t);
                    t.TIMES = processSchedulingThread.timeslice * (t.queueNum + 1);
                    for (int a = 0; a < 4; a++)
                    {
                        if (processSchedulingThread.readyJob[a].Count != 0)
                        {
                            usingCpu(processSchedulingThread.readyJob[a][0]);
                            return;
                        }
                    }
                }
            }
            else
            {
                processSchedulingThread.readyJob[0].RemoveAt(0);
                processSchedulingThread.readyJob[0].Add(t);
                t.TIMES = processSchedulingThread.timeslice;
                usingCpu(processSchedulingThread.readyJob[0][0]);
            }
        }
    }
}
