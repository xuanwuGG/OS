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
        private int requiredBlocks;
        private List<int> allocatedBlocks;
        public int CompareTo(process other)
        {
            if (this.priority > other.priority) return 1;
            else return -1;
        }

        public void allocateMemory(physicalManager manager) 
        {
            var beSpace = manager.physicalSpace;
            var be = this.requiredBlocks;
            for (int i = 0; i < 16; i++)
            {
                if (requiredBlocks <= 0)
                {
                    break;
                }
                if (manager.physicalSpace[i]==0)
                {
                    manager.allocate(i);
                    allocatedBlocks.Add(i);
                    requiredBlocks--;
                }
            }
            if (requiredBlocks > 0)
            {
                manager.physicalSpace = beSpace;
                this.requiredBlocks = be;
                throw new ArgumentException("Do not have enough space");
            }
        }
        public void free(physicalManager manager)
        {
            for(int i=0;i<allocatedBlocks.Count; i++)
            {
                manager.free(allocatedBlocks[i]);
            }
        }
        public process(int jobsId, int priority, int inTime, int instructNum)
        {
            this.jobsId = jobsId;
            this.inTime = inTime;
            this.instructNum = instructNum;
            this.requiredBlocks = instructNum + 9 / 10;
            this.priority = priority;
            this.allocatedBlocks = new List<int>();
            TIMES = processSchedulingThread.timeslice;

        }
    }
}
