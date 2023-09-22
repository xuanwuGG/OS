using project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS
{
    internal class inputBlock_thread
    {
        public static ReaderWriterLockSlim bufferLock = new ReaderWriterLockSlim();
        public static List<process> blockJobs1 = new List<process>();
        public static int count = 3;
        public static process waitBuffer = new process();
        public static process waitKeyboard = new process();
        public static int sig = 0;

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
                counting();
                while (sig == 1 && Monitor.TryEnter(Program.buffer)) { sig = 0; Monitor.Exit(Program.buffer); counting(); }
                Program.outputLock.Set();
            }
        }
        public static void counting()
        {
            if (blockJobs1.Count != 0)
            {
                if (Monitor.TryEnter(Program.keyboard))
                {
                    Console.WriteLine("[P操作:获取keyboard]");

                    clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                    if (Monitor.TryEnter(Program.buffer))
                    {
                        Console.WriteLine(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                        clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                        while (--count != 0)
                        {
                            if (blockJobs1.Count > 1) { sig = 1; }
                            Program.outputLock.Set();
                            Program.inputLock.WaitOne();
                        }
                        if (count == 0)
                        {
                            process tmpWork = blockJobs1[0];
                            Console.WriteLine("[拷贝入缓冲区:进程 ID:{0}]", tmpWork.jobsId);
                            Console.WriteLine("[重新进入就绪队列:进程 ID:{0},待执行的指令数:{1}]", tmpWork.jobsId, (tmpWork.instructionRegister.Count + tmpWork.programCounter));
                            clockThread.content.Add(clockThread.COUNTTIME + ":[拷贝入缓冲区:进程 ID:" + tmpWork.jobsId + "]");
                            clockThread.content.Add(clockThread.COUNTTIME + ":[重新进入就绪队列:进程 ID:" + tmpWork.jobsId + ",待执行的指令数:" + (tmpWork.instructionRegister.Count - tmpWork.programCounter) + "]");
                            blockJobs1.RemoveAt(0);
                            processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                            CPU.CPU_REC(tmpWork);
                            count = 3;
                        }
                        Console.WriteLine("[V操作:释放buffer]");

                        clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放buffer]");
                        Monitor.Exit(Program.buffer);
                    }
                    Console.WriteLine(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    Monitor.Exit(Program.keyboard);
                }
            }
        }
        public static void counting2()
        {
            while (true)
            {
                Program.deadLock.WaitOne();
                if (blockJobs1.Count != 0)
                {
                    if (Monitor.TryEnter(Program.keyboard))
                    {
                        waitBuffer = blockJobs1[0];
                        while(!Monitor.TryEnter(Program.buffer))//抢到不放
                        {
                            Program.deadLock.WaitOne();
                            if(blockJobs1.Count>1&&waitKeyboard.jobsId!=0&&Monitor.TryEnter(Program.buffer))
                            {
                                waitKeyboard= blockJobs1[1];
                            }
                        }
                    }
                    else if(Monitor.TryEnter(Program.buffer))
                    {
                        while(!Monitor.TryEnter(Program.keyboard))//抢到不放
                        {
                            Program.deadLock.WaitOne();
                            if (blockJobs1.Count > 1 && waitBuffer.jobsId != 0 && Monitor.TryEnter(Program.keyboard))
                            {
                                waitBuffer = blockJobs1[1];
                            }
                        }
                    }
                    //else 既抢不到键盘也抢不到buffer
                }
            }
        }
    }
}
