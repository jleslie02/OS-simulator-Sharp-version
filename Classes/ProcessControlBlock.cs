using System;
using System.Collections.Generic;
using System.Linq;

namespace OSEmulator3
{
    class ProcessControlBlock : IDisposable
    {
        public static List<Job> jobQueue;
        public static String[][] jobQ;
        public Job pcb_e;
        public static int count;
        private Form1 form;
        public void Dispose()
        {
            if (form != null)
            {
                form.Dispose();
                form = null;
            }
        }
        public ProcessControlBlock(Form1 form1)
        {
            this.form = form1;
            count = 0;
            jobQueue = new List<Job>();
        }

        public void textOutPut(string content)
        {
            form.outputTxt.AppendText(content + "\n");
        }
        public void createJob(int i, int s, int p, int a)
        {
            pcb_e = new Job(i, s, p, a);
        }

        public void addMeta(int i, int o, int t)
        {
            pcb_e.AddData(i, o, t);
            jobQueue.Add(pcb_e);
            count++;
        }

        public void SetDataSize(int s)
        {
            pcb_e.DataSize = s;
        }

        public int GetJobCount()
        {
            return count;
        }

        public Job GetJob(int i)
        {
            return jobQueue.ElementAt(i);
        }
        
        public void removeJob(int j)
        {
            jobQueue.ElementAt(j);
        }

        public void printPCB()
        {
            foreach (Job v in jobQueue)
            {
                textOutPut(String.Format("JobID: {0}\tJobPriority: {1}\tJobSize: {2}", v.jobID, v.jobPriority, v.jobSize));
            }
        }
    }
}
