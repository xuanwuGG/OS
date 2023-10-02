using project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS
{
    public class outputBlock_thread
    {
        Thread outBlock;
        public static List<process> blockJobs2 = new List<process>();
        public static process getScreen = new process();
        public static int count = 3;
        public static int sig = 0;
        public outputBlock_thread()
        {
            outBlock = new Thread(outblock);
            outBlock.Start();
        }
        public static void outblock()
        {
            while (true)
            {
                Program.outputLock.WaitOne();
                if (!Program.deadlock && blockJobs2.Count != 0)
                {
                    counting();
                }
                else if (Program.deadlock && (blockJobs2.Count != 0 || getScreen.jobsId != 0))
                {
                    counting2();
                }
                else if(Program.deadlock&& (blockJobs2.Count != 0 || getScreen.jobsId != 0)||inputBlock_thread.getKeyboardProcess.jobsId!=0)
                {
                    Console.WriteLine("[缓冲区无进程]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[缓冲区无进程]");
                } 
                else
                {
                    Console.WriteLine("[缓冲区无进程]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[缓冲区无进程]");
                }
                clockThread.clevent.Set();
            }
        }

        public static void counting2()
        {
            if (Monitor.TryEnter(Program.screen))
            {
                Console.WriteLine("[P操作:获取screen]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取screen]");
                if (getScreen.jobsId == 0) { getScreen = blockJobs2[0]; blockJobs2.RemoveAt(0); }
            }
            else { return; }
            if (Monitor.TryEnter(Program.buffer))
            {
                if (blockJobs2.Count != 0 && getScreen.jobsId != 0 && ((new Random()).Next(0, 10) < 11))//调整死锁概率
                {
                    Console.WriteLine("[V操作:释放screen]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放screen]");
                    Monitor.Exit(Program.screen);
                    blockJobs2.Insert(1, getScreen);
                    getScreen = new process();
                    Console.WriteLine("[P操作:获取screen]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取screen]");
                    Monitor.TryEnter(Program.screen);
                }
                else if (blockJobs2.Count == 0 && getScreen.jobsId != 0)
                {
                    blockJobs2.Add(getScreen);
                    getScreen=new process();
                }
                Console.WriteLine("[P操作:获取buffer]");
                clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                if (blockJobs2.Count != 0 && getScreen.jobsId != 0)
                {
                    Console.WriteLine("检测到死锁");
                    Console.ReadKey();
                    return;
                }
                while (--count != 0)
                {
                    clockThread.clevent.Set();
                    Program.outputLock.WaitOne();
                }
                if (count == 0)
                {
                    process tmpWork = blockJobs2[0];
                    Console.WriteLine("[拷贝入缓冲区:进程 ID:{0}]", tmpWork.jobsId);
                    Console.WriteLine("[重新进入就绪队列:进程 ID:{0},待执行的指令数:{1}]", tmpWork.jobsId, (tmpWork.instructionRegister.Count + tmpWork.programCounter));
                    clockThread.content.Add(clockThread.COUNTTIME + ":[拷贝入缓冲区:进程 ID:" + tmpWork.jobsId + "]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[重新进入就绪队列:进程 ID:" + tmpWork.jobsId + ",待执行的指令数:" + (tmpWork.instructionRegister.Count - tmpWork.programCounter) + "]");
                    blockJobs2.RemoveAt(0);
                    processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                    CPU.CPU_REC(tmpWork);
                    count = 3;
                    Console.WriteLine("[V操作:释放screen]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放screen]");
                    Monitor.Exit(Program.screen);
                    Console.WriteLine("[V操作:释放buffer]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放buffer]");
                    Monitor.Exit(Program.buffer);
                }
            }
        }

        public static void counting()
        {
                if (Monitor.TryEnter(Program.screen))
                {
                    Console.WriteLine("[P操作:获取screen]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取screen]");
                    if (Monitor.TryEnter(Program.buffer))
                    {
                        Console.WriteLine("[P操作:获取buffer]");
                        clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                        while (--count != 0)
                        {
                            if (blockJobs2.Count > 1) { sig = 1; }
                            clockThread.clevent.Set();
                            Program.outputLock.WaitOne();
                        }
                        if (count == 0)
                        {
                            process tmpWork = blockJobs2[0];
                            Console.WriteLine("[拷贝出缓冲区:进程 ID:{0}]", tmpWork.jobsId);
                            Console.WriteLine("[重新进入就绪队列:进程 ID{0},待执行的指令数:{1}]", tmpWork.jobsId, (tmpWork.instructionRegister.Count - tmpWork.programCounter));
                            clockThread.content.Add(clockThread.COUNTTIME + ":[拷贝入缓冲区:进程 ID:" + tmpWork.jobsId + "]");
                            clockThread.content.Add(clockThread.COUNTTIME + ":[重新进入就绪队列:进程 ID:" + tmpWork.jobsId + ",待执行的指令数:" + (tmpWork.instructionRegister.Count - tmpWork.programCounter) + "]");
                            blockJobs2.RemoveAt(0);
                            processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                            CPU.CPU_REC(tmpWork);
                            count = 3;
                        }
                        Console.WriteLine("[V操作:释放buffer]");
                        clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放buffer]");
                        Monitor.Exit(Program.buffer);
                    }
                    Console.WriteLine("[V操作:释放screen]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放screen]");
                    Monitor.Exit(Program.screen);
                }

        }

    }
}
