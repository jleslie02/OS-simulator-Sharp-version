using System;
using System.Collections.Generic;
using System.Linq;

namespace OSEmulator3
{
    class ShortTermScheduler:IDisposable
    {
        public Job job;
        public static List<int> regQueue = new List<int>();
        public int jobID;
        private Form1 form;
        public ShortTermScheduler(Form1 form1)
        {
            this.form = form1;
        }
        public void Dispose()
        {
            if (form != null)
            {
                form.Dispose();
                form = null;
            }
        }
        public void SJF()
        {
            //Sort readyQueue in priority order
            int n = LongTermScheduler.readyQueue.Count;
            bool doMore = true;
            while (doMore)
            {
                n--;
                doMore = false;
                for (int i = 0; i < n ; i++)
                {
                    Job j1 = LongTermScheduler.readyQueue[i];
                    Job j2 = LongTermScheduler.readyQueue[i + 1];
                    if (j1.jobPriority < j2.jobPriority)
                    {
                        job = j1;
                        LongTermScheduler.readyQueue[i] = LongTermScheduler.readyQueue[i + 1];
                        LongTermScheduler.readyQueue[i + 1] = job;
                        doMore = true;
                    }
                }
            } 
        }
        public Job Store(int jID)
        {
            Job tmp = LongTermScheduler.readyQueue[jID];
            LongTermScheduler.readyQueue.Remove(tmp);
            tmp.DeQueueTime = Environment.TickCount;
            tmp.CpuStartTime = Environment.TickCount;
            form.outputTxt.Text += ("REMOVED JOB FROM READY Q.. SIZE IS NOW " + LongTermScheduler.readyQueue.Count);
            return tmp;
        }

        public Job GetJobByID(List<Job> j,int id)
        {
            foreach (Job job in j)
            {
                if (job.jobID == id)
                {
                    return job;
                }
            }
            return null;
        }

        public int GetJobIndex(List<Job> j, int id)
        {
            for (int i = 0; i < j.Count; i++)
            {
                if (j[i].jobID == id)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
