using OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace project
{
    internal class process:IComparable<process>
    {
        public int jobsId;
        public int inTime;
        public int instructNum;
        public int priority;
        public string PSW = null;//new ready running block exit
        public string instrucNum=null;
        public List<int> instruct=new List<int>();
        public int TIMES;
        public int queueNum = 0;
        public bool isReflect = false;
        public int instr1Count = 0;
        public int sAddress=0;
        public int CompareTo(process other)
        {
            if (this.priority > other.priority) return 1;
            else return -1;
        }

        public process(int jobsId,  int inTime, int instructNum)
        {
            this.jobsId = jobsId;
            this.inTime = inTime;
            this.instructNum = instructNum;
            this.requiredBlocks = instructNum + 9 / 10;
            this.allocatedBlocks = new List<int>();
            TIMES = processSchedulingThread.timeslice;

        }
    }
}
