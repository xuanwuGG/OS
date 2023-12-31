﻿using project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS
{
    public class inputBlock_thread
    {
        public static List<process> blockJobs1 = new List<process>();
        public static process getKeyboardProcess =new process();
        public static int count = 3;//阻塞时间
        public static int sig = 0;//开始阻塞时间
        public static bool dead = false;
        Thread inputBlock;
        public inputBlock_thread()
        {
            inputBlock = new Thread(inputlock);
            inputBlock.Start();
        }
        public static void inputlock()
        {
            while (true)
            {
                Program.inputLock.WaitOne();
                if (!Program.deadlock && blockJobs1.Count != 0)
                {
                    counting();
                }
                else if (Program.deadlock && (blockJobs1.Count != 0 || getKeyboardProcess.jobsId != 0))
                {
                    counting2();
                }
                Program.outputLock.Set();
            }
        }

        public static void counting2()
        {
            if (Monitor.TryEnter(Program.keyboard))
            {
                clockThread.content2.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                if (getKeyboardProcess.jobsId == 0){getKeyboardProcess = blockJobs1[0];blockJobs1.RemoveAt(0);}
            }
            else { return; }
            if (Monitor.TryEnter(Program.buffer))
            {
                if (blockJobs1.Count != 0&&getKeyboardProcess.jobsId!=0&& ((new Random()).Next(0, 10) < 1))//调整死锁概率
                {
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    Monitor.Exit(Program.keyboard);
                    blockJobs1.Insert(1, getKeyboardProcess);
                    getKeyboardProcess = new process();
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                    Monitor.TryEnter(Program.keyboard);
                }
                else if(blockJobs1.Count == 0&& getKeyboardProcess.jobsId != 0) 
                {
                    blockJobs1.Add(getKeyboardProcess);
                    getKeyboardProcess=new process();
                }
                clockThread.content2.Add(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                if (blockJobs1.Count != 0 && getKeyboardProcess.jobsId != 0)//死锁解除
                {
                    dead = true;
                    Program.outputLock.Set();
                    Program.inputLock.WaitOne();
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    Monitor.Exit(Program.keyboard);
                    blockJobs1.Insert(1, getKeyboardProcess);
                    getKeyboardProcess = new process();
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                    Monitor.TryEnter(Program.keyboard);
                    dead = false;
                }
                while (--count != 0)
                {
                    Program.outputLock.Set();
                    Program.inputLock.WaitOne();
                }
                if (count == 0)
                {
                    process tmpWork = blockJobs1[0];
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[拷贝入缓冲区:" + tmpWork.jobsId + "]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[拷贝入缓冲区:" + tmpWork.jobsId + "]");
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[重新进入就绪队列:" + tmpWork.jobsId + "," + (tmpWork.instructionRegister.Count - tmpWork.programCounter) + "]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[重新进入就绪队列:" + tmpWork.jobsId + "," + (tmpWork.instructionRegister.Count - tmpWork.programCounter) + "]");
                    blockJobs1.RemoveAt(0);
                    processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                    CPU.CPU_REC(tmpWork);
                    count = 3;
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    Monitor.Exit(Program.keyboard);
                    Console.WriteLine("[V操作:释放buffer]");
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[V操作:释放buffer]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放buffer]");
                    Monitor.Exit(Program.buffer);
                }
            }
        }
        public static void counting()
        {
            if ( Monitor.TryEnter(Program.keyboard))
            {
                clockThread.content2.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
            }
            else { return; }
            if (Monitor.TryEnter(Program.buffer))
            {
                sig = 1;
                clockThread.content2.Add(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                while (--count != 0)
                {
                    Program.outputLock.Set();
                    Program.inputLock.WaitOne();
                }
                if (count == 0)
                {
                    process tmpWork = blockJobs1[0];
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[拷贝入缓冲区:" + tmpWork.jobsId + "]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[拷贝入缓冲区:" + tmpWork.jobsId + "]");
                    clockThread.content2.Add(clockThread.COUNTTIME + ":[重新进入就绪队列:" + tmpWork.jobsId + "," + (tmpWork.instructionRegister.Count - tmpWork.programCounter) + "]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[重新进入就绪队列:" + tmpWork.jobsId + "," + (tmpWork.instructionRegister.Count - tmpWork.programCounter) + "]");
                    blockJobs1.RemoveAt(0);
                    processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                    CPU.CPU_REC(tmpWork);
                    count = 3;
                }
            }
            clockThread.content2.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
            clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
            Monitor.Exit(Program.keyboard);
            if (sig == 1)
            {
                sig = 0;
                clockThread.content2.Add(clockThread.COUNTTIME + ":[V操作:释放buffer]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放buffer]");
                Monitor.Exit(Program.buffer);
            }
        }
    }
}
