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
        public CPU() { }
        public static void usingCpu(work t)
        {
            if(t.instruct.Count==0)
            {
                Console.WriteLine("{0}号作业已经完成！", t.jobsId);
                Thread.Sleep(200);
                processSchedulingThread.readyJob[0].RemoveAt(0);
                for (int a = 0; a < 3; a++)
                {
                    if (processSchedulingThread.readyJob[a].Count!=0)
                    {
                        usingCpu(processSchedulingThread.readyJob[a][0]);
                    }
                }
                return;
            }
            if(t.TIMES==1)
            {
                if (t.instruct.First() ==1)
                {
                    timeUp(t);
                    return;
                }
                else
                {
                    t.jobStatus = "Running";
                    t.TIMES -= 1;
                    t.instruct.RemoveAt(0);
                    return;
                }
            }
            else if(t.TIMES<=0)
            {
                timeUp(t);
            }
            else
            {
                t.jobStatus = "Running";
                if (t.instruct.First()==0)
                {
                    t.TIMES -= 1;
                }
                else if(t.instruct.First()==1)
                {
                    t.TIMES -= 2;
                }
                else if( t.instruct.First()==2)
                {
                    CPU_PRO(t,2);
                }
                else
                {
                    CPU_PRO(t, 3);
                }
                t.instruct.RemoveAt(0);
                t.jobStatus = "Ready";
            }
        }
        public static void CPU_PRO(work t,int sig)
        {
            t.jobStatus = "Block";
            processSchedulingThread.readyJob[t.queueNum].RemoveAt(0);
            if (sig == 2) 
            {
                inputBlock_thread.blockJobs1.Add(t);
                inputBlock_thread.wake();
            }
            else 
            { 
                outputBlock_thread.blockJobs2.Add(t);
                outputBlock_thread.wake();
            }

        }
        public static void CPU_REC(work t)
        {
            t.jobStatus = "Ready";
            if(t.queueNum==3)
            {
                t.TIMES = 99;
            }
            else
            {
                t.TIMES = processSchedulingThread.timeslice * (t.queueNum + 1);
            }
        }
        public static void timeUp(work t)
        {
            Console.WriteLine("第{0}优先级队列进程{1}时间片结束",t.queueNum, t.jobsId);
            Thread.Sleep(200);
            t.jobStatus = "Ready";
            if (t.isReflect)
            {
                if (t.queueNum == 3)
                {
                    Console.WriteLine("{0}已处于优先级队列，继续执行", t.jobsId);
                    Thread.Sleep(200);
                    t.TIMES = 99;
                    usingCpu(t);
                }
                else
                {
                    processSchedulingThread.readyJob[t.queueNum].RemoveAt(0);
                    t.queueNum++;
                    processSchedulingThread.readyJob[t.queueNum].Add(t);
                    t.TIMES = processSchedulingThread.timeslice * (t.queueNum + 1);
                    for (int a = 0; a < 3; a++)
                    {
                        if (processSchedulingThread.readyJob[a].Count != 0)
                        {
                            Console.WriteLine("第{0}优先级队列进程{1}时间片开始", processSchedulingThread.readyJob[a][0].queueNum, processSchedulingThread.readyJob[a][0].jobsId);
                            Thread.Sleep(200);
                            usingCpu(processSchedulingThread.readyJob[a][0]);
                        }
                    }
                }
            }
            else
            {
                processSchedulingThread.readyJob[0].RemoveAt(0);
                processSchedulingThread.readyJob[0].Add(t);
                t.TIMES = processSchedulingThread.timeslice;
                Console.WriteLine("进程{0}时间片开始",processSchedulingThread.readyJob[0][0].jobsId);
                Thread.Sleep (200);
                usingCpu(processSchedulingThread.readyJob[0][0]);
            }
        }
    }
}
