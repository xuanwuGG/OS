using project;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS
{
    public class addJobs
    {
        public static int jobnum = 0;
        public static void add(int num)
        {
            StreamReader reader = new StreamReader(Program.filePath + "jobs-input.txt",true);
            string lastline=null;
            string line=reader.ReadLine();
            while (line!= null)
            {
                lastline = line;
                line = reader.ReadLine();
            }
            if (lastline!=null) 
            {
                string []jobno = lastline.Split(',');
                jobnum = int.Parse(jobno[0]);
            }
            reader.Close();
            StreamWriter writer = new StreamWriter(Program.filePath + "jobs-input.txt",true);
            for(int i = 0; i < num; i++)
            {
                int a = new Random(Guid.NewGuid().GetHashCode()).Next(0,2);
                int strNum;
                if (a >0) { strNum = 20; }
                else { strNum = 10; }
                writer.Write((++jobnum)+","+(clockThread.COUNTTIME)+","+strNum+ "\n" );
                StreamWriter writer1 = new StreamWriter(Program.filePath + jobnum + ".txt");
                for(int j = 1; j <= strNum;j++)
                {
                    int s= new Random(Guid.NewGuid().GetHashCode()).Next(0,4);
                    writer1.WriteLine(j+","+s);
                }
                writer1.Close();
            }
            writer.Close();
        }
    }
}
