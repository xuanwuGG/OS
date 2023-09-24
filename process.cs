using OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace project
{
    internal class process : IComparable<process>
    {
        public int jobsId;
        public int inTime;
        public int endTime;
        public int runTime;
        public int instructNum;
        public int priority;
        public string PSW = null;//new ready running block exit
        public List<int> instructionRegister = new List<int>();
        public int programCounter = 0;
        public int TIMES;
        public int queueNum = 0;
        public int instr1Count = 0;
        public int sAddress = 0;
        public int requiredBlocks = 0;
        public process() { }
        public int CompareTo(process other)
        {
            if (this.priority > other.priority) return 1;
            else return -1;
        }
        public void reset()
        {
            this.jobsId = 0;
            this.instructionRegister = new List<int>();
        }
        public process(int jobsId, int inTime, int instructNum)
        {
            this.jobsId = jobsId;
            this.inTime = inTime;
            this.instructNum = instructNum;
            this.requiredBlocks = (instructNum + 9) / 10;
            TIMES = processSchedulingThread.timeslice;

        }
    }
}
