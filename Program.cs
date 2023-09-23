using OS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace project
{
    internal class Program
    {
        public static string filePath = "D:/input2/";
        public static object buffer = new object();
        public static object keyboard = new object();
        public static object screen = new object();
        public static int readyBlock = 0;
        public static int deadlock = 0;
        public static AutoResetEvent clevent = new AutoResetEvent(false);
        public static AutoResetEvent jievent = new AutoResetEvent(false);
        public static AutoResetEvent psevent = new AutoResetEvent(false);
        public static AutoResetEvent inputLock = new AutoResetEvent(false);
        public static AutoResetEvent outputLock = new AutoResetEvent(false);
        public static List<process> tmpBackUpJob = new List<process>();
        public static List<process> BackUpJob = new List<process>();
        public static memoryManager manager = new memoryManager();
        public static AutoResetEvent deadLock=new AutoResetEvent(false);
        static void Main(string[] args)
        {
            clockThread t1 = new clockThread();
            jobInThread t2 = new jobInThread();
            processSchedulingThread t3 = new processSchedulingThread();
            inputBlock_thread t4 = new inputBlock_thread();
            outputBlock_thread t5 = new outputBlock_thread();

        }
    }
}
