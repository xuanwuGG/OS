using project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS
{
    internal class outputBlock_thread
    {
        Thread outBlock;
        public static List<process> blockJobs2 = new List<process>();
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
                counting2();
                //while (sig == 1 && Monitor.TryEnter(Program.buffer)) { sig = 0; Monitor.Exit(Program.buffer); counting(); }
                if (inputBlock_thread.blockJobs1.Count == 0 && blockJobs2.Count == 0)
                {
                    Console.WriteLine("[缓冲区无进程]");
                    clockThread.content.Add(clockThread.COUNTTIME + ":[缓冲区无进程]");
                }
                //Program.clevent.Set();
            }
        }
        public static void counting()
        {
            if (blockJobs2.Count != 0)
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
                            Program.clevent.Set();
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
                    clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                    Monitor.Exit(Program.screen);
                }
            }
        }
        public static void counting2()
        {
            while (true)
            {
                if (blockJobs2.Count != 0)
                {
                    if (Monitor.TryEnter(Program.buffer))
                    {
                        Console.WriteLine("{0}获取缓冲区", blockJobs2[0]);
                        Thread.Sleep(1000);
                        if (Program.deadlock==0&&Monitor.TryEnter(Program.screen))
                        {
                            Program.deadlock++;
                            Console.WriteLine("{0}获取屏幕", blockJobs2[0]);
                            Thread.Sleep(1000);
                            while (count-- != 0)
                            {
                                Program.outputLock.WaitOne();
                                Program.readyBlock++;
                                if (Program.readyBlock == 2)
                                {
                                    Program.readyBlock = 0;
                                    Program.clevent.Set();

                                }
                            }
                            if (count == 0)
                            {
                                count = 3;
                                Console.WriteLine("4指令运行结束");
                                processSchedulingThread.readyJob[blockJobs2[0].queueNum].Add(blockJobs2[0]);
                                blockJobs2.RemoveAt(0);
                                Thread.Sleep(1000);
                            }
                            Monitor.Exit(Program.screen);
                            Console.WriteLine("退出屏幕");
                        }
                        Monitor.Exit(Program.buffer);
                        Console.WriteLine("退出缓冲区");
                        if (Program.deadlock == 1) { Program.deadlock = 0; }
                    }
                    if (Program.deadlock == 1) { Program.deadlock = 0; }
                }
                Program.readyBlock++;
                if (Program.readyBlock == 2)
                {
                    Program.readyBlock = 0;
                    Program.clevent.Set();
                }
            }
        }

    }
}
