using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace project
{
    internal class work:IComparable<work>
    {
        public int jobsId;
        public int inTime;
        public int instructNum;
        public int priority;
        public string jobStatus = null;//new ready running block exit
        public string instrucNum=null;
        public List<int> instruct=new List<int>();
        public int TIMES;
        public int queueNum = 0;
        public bool isReflect = false;
        public int instr1Count = 0;
        public int CompareTo(work other)
        {
            if (this.priority > other.priority) return 1;
            else return -1;
        }
        public work() 
        {
            jobsId = 0;
            inTime = 0; 
            instructNum = 0;
            TIMES = processSchedulingThread.timeslice;
        }   
        public work(int jobsId, int priority, int inTime, int instructNum)
        {
            this.jobsId = jobsId;
            this.inTime = inTime;
            this.instructNum = instructNum;
            this.priority = priority;
            TIMES = processSchedulingThread.timeslice;
        }
    }
}
