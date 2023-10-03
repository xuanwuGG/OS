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
using System.Windows.Forms;

namespace project
{
    internal class Program
    {
        public static string filePath = "D:/input2/";
        public static object buffer = new object();
        public static object keyboard = new object();
        public static object screen = new object();
        public static bool deadlock = false;
        public static bool partnersystem = false;
        public static AutoResetEvent jievent = new AutoResetEvent(false);
        public static AutoResetEvent psevent = new AutoResetEvent(false);
        public static AutoResetEvent inputLock = new AutoResetEvent(false);
        public static AutoResetEvent outputLock = new AutoResetEvent(false);
        public static List<process> tmpBackUpJob = new List<process>();
        public static memoryManager manager = new memoryManager();
        public static partnerManager partnermanager = new partnerManager();

        static void Main(string[] args)
        {
            //clockThread t1=new clockThread();
            //processSchedulingThread t2=new processSchedulingThread();
            //jobInThread t3=new jobInThread();
            //inputBlock_thread t4=new inputBlock_thread();
            //outputBlock_thread t5=new outputBlock_thread();
        }
    }
}
