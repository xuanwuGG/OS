using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    internal class work:IComparable<work>
    {
        public int jobsId;
        public int inTime;
        public int instructNum;
        public int CompareTo(work other)
        {
            if (this.inTime > other.inTime) return 1;
            else return -1;
        }
        public work() 
        {
            jobsId = 0;
            inTime = 0; 
            instructNum = 0;
        }   
        public work(int jobsId, int inTime, int instructNum)
        {
            this.jobsId = jobsId;
            this.inTime = inTime;
            this.instructNum = instructNum;
        } 
    }
}
