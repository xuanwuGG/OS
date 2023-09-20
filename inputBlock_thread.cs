using project;
using System;
using System.Collections.Generic;
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
                if (blockJobs1.Count != 0)
                {
                    if (Monitor.TryEnter(Program.keboard))
                    {
                        Console.WriteLine("[P操作:获取keyboard]");

                        clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取keyboard]");
                        if (Monitor.TryEnter(Program.buffer))
                        {
                            Console.WriteLine(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                            clockThread.content.Add(clockThread.COUNTTIME + ":[P操作:获取buffer]");
                            while (--count != 0)
                            {
                                Program.outputLock.Set();
                                Program.inputLock.WaitOne();
                            }
                            if (count == 0)
                            {
                                process tmpWork = blockJobs1[0];
                                if (tmpWork.TIMES == 0&&tmpWork.queueNum<3) { tmpWork.queueNum++;tmpWork.TIMES = processSchedulingThread.timeslice*(tmpWork.queueNum+1); }
                                Console.WriteLine("[拷贝入缓冲区:进程 ID:{0}]", tmpWork.jobsId);
                                Console.WriteLine("[重新进入就绪队列:进程 ID:{0},待执行的指令数:{1}]", tmpWork.jobsId, (tmpWork.instructionRegister.Count + tmpWork.programCounter));
                                clockThread.content.Add(clockThread.COUNTTIME + ":[拷贝入缓冲区:进程 ID:" + tmpWork.jobsId + "]");
                                clockThread.content.Add(clockThread.COUNTTIME + ":[重新进入就绪队列:进程 ID:" + tmpWork.jobsId + ",待执行的指令数:" + (tmpWork.instructionRegister.Count - tmpWork.programCounter) + "]");
                                blockJobs1.RemoveAt(0);
                                processSchedulingThread.readyJob[tmpWork.queueNum].Add(tmpWork);
                                count = 3;
                            }
                            Console.WriteLine("[V操作:释放buffer]");

                            clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放buffer]");
                            Monitor.Exit(Program.buffer);
                        }
                        Console.WriteLine(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                        clockThread.content.Add(clockThread.COUNTTIME + ":[V操作:释放keyboard]");
                        Monitor.Exit(Program.keboard);
                    }
                }
                Program.outputLock.Set();
            }
        }
    }
}
