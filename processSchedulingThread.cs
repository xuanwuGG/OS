using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace project
{
    internal class processSchedulingThread
    {
        public static Dictionary<int, string[]> strucTable=new Dictionary<int, string[]>();//指令表

        public static int timeslice = 3;
        public static int TIMES= timeslice;
        public Thread ProcessScheduling_thread=null;
        public processSchedulingThread()
        {
            ProcessScheduling_thread = new Thread(schedule);
            ProcessScheduling_thread.Start();
        }
        public static void schedule()
        {
            updateStrucTable();
            while(true) 
            {
                
                Program.psevent.WaitOne();
                if(--TIMES!=0)
                {
                    
                }
                else
                {
                    Console.WriteLine("time up!现在CPU换进程-----");
                    Thread.Sleep(1000);
                    ProcessScheduling();
                    Console.WriteLine("成功更换！------");
                    Thread.Sleep(1000);
                    TIMES = timeslice;
                }
                Program.clevent.Set();
            }
        }
        public static void ProcessScheduling()
        {

        }
        public static void SetSchedulingTime(int num)
        {
            timeslice = num;
            TIMES = timeslice;
        }
        public static void wake()
        {
            Program.psevent.Set();
        }

        public static void updateStrucTable()
        {
            StreamReader strucStream = new StreamReader(Program.filePath + "instrc.txt");
            for (; strucStream.Peek() >= 0;)
            {
                string[] eachline=strucStream.ReadLine().Split(',');
                int num = int.Parse(eachline[0]);
                string[] tmp = new string[eachline.Length];
                eachline.CopyTo(tmp, 1);
                strucTable.Add(num, tmp);
            }
            strucStream.Close();
        }
    }
}
