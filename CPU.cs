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
        public static List<work> runningJobs=new List<work>();
        public int ProgramCounter = 0;
        public CPU() { }
        public static void usingCpu(work t)
        {
            t.jobStatus = "Running";
            runningJobs.Add(t);
            if(t.instruct.Count==0)
            {
                Console.WriteLine("{0}号作业已经完成！", t.jobsId);
                processSchedulingThread.readyJob.RemoveAt(0);
                return;
            }
            if(t.TIMES==1)
            {
                if (t.instruct.First() ==1)
                {
                    timeUp(t);
                    return;
                }
                else if(t.instruct.First() ==2)
                {
                    timeUp(t);
                    return;
                }
            }
            else if(t.TIMES==0)
            {
                timeUp(t);
            }
            else
            {
                if(t.instruct.First()==0)
                {
                    t.TIMES -= 1;
                    t.instruct.RemoveAt(0);
                }
                else if(t.instruct.First()==1)
                {
                    t.TIMES -= 2;
                    t.instruct.RemoveAt(0);
                }
                else if( t.instruct.First()==2)
                {
                    CPU_PRO(t);
                    inputBlock_thread.wake();
                    CPU_REC(t);
                }
            }
            runningJobs.RemoveAt(0);
            t.jobStatus = "Ready";
        }
        public static void CPU_PRO(work t)
        {
            t.jobStatus = "Block";
        }
        public static void CPU_REC(work t)
        {
            t.jobStatus = "Running";
        }
        public static void timeUp(work t)
        {
            Console.WriteLine("{0}时间片结束", t.jobsId);
            Thread.Sleep(500);
            work tmpWork = t;
            processSchedulingThread.readyJob.RemoveAt(0);
            t.TIMES = processSchedulingThread.timeslice;
            processSchedulingThread.readyJob.Add(tmpWork);
            t.jobStatus = "Ready";
            Console.WriteLine("{0}时间片开始", processSchedulingThread.readyJob[0].jobsId);
            usingCpu(t);
        }
    }
}
