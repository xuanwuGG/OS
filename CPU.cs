using project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS
{
    internal class CPU
    {
        public int ProgramCounter = 0;
        public static readonly object cpuLock=new object();
        public CPU() { }
        public static string usingCpu(work t)
        {
            if(t.workStatus.Equals("Ready"))
            {
                IDictionaryEnumerator enumerator = processSchedulingThread.strucTable.GetEnumerator();
                while((string)enumerator.Key!=t.instrucNum)
                {
                    enumerator.MoveNext();
                    if(enumerator==null)
                    {
                        break;
                    }
                }
                if(enumerator==null)
                {
                    Console.WriteLine("警告：未找到该作业指令内容，请确认作业指令编号准确性且确保指令表有关该指令编号非空");
                }
                else
                {
                    
                }
            }
            return null;
        }
        public static void CPU_PRO(work t)
        {
            t.workStatus = "block";

        }
        public static void CPU_REC() 
        {
            
        }
    }
}
