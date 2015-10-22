using System;
using System.Collections.Generic;
using System.Linq;

namespace OSEmulator3
{
    class LongTermScheduler:IDisposable
    {
        public static List<Job> readyQueue;
        private const int RAMSIZE = 1024;
        private static int Memleft = 1024;
        private int loc = 0;
        private int CURRJOB;
        private int jobStart;
        private int jobSize;
        private int dataSize;
        private int jobIBSize;
        private int jobOBSize;
        private int jobTBSize;
        private const int ready = 0;
        Job job;
        public static  double average;
        public static  double percent;
        private Form1 form;
        public void Dispose()
        {
            if (form != null)
            {
                form.Dispose();
                form = null;
            }
        }
        public LongTermScheduler(Form1 form1)
        {
            this.form = form1;
            readyQueue = new List<Job>();
            CURRJOB = 0;
            Start();
        }
        public void textOutPut(object content)
        {
            form.outputTxt.AppendText(content + "\n");
        }
        public void Start()
        {
            if (HasLoadedAllJobs())
            {
                textOutPut("ALL JOBS HAVE COMPLETED.");
                RamPercent(1);
                OSDriver.DONE = true;
                return;
            }
            else
            {
                loc = 0;
                Memleft = 1024;
            }
            if (CURRJOB >= OSDriver.PCB.GetJobCount())
            {
                OSDriver.DONE = true;
                return;
            }
            job = OSDriver.PCB.GetJob(CURRJOB);

            jobStart = job.DiskAddress;
            jobSize = job.jobSize;
            jobIBSize = job.InBufferSize;
            jobOBSize = job.OutBufferSize;
            jobTBSize = job.TmpBufferSize;
            dataSize = job.DataSize;

            textOutPut("Start: " + jobStart);
            textOutPut("Size: " + jobSize);
            textOutPut("IP Buffer Size: " + jobIBSize);
            textOutPut("OP Buffer Size: " + jobOBSize);
            textOutPut("TMP Buffer Size: " + jobTBSize);

            textOutPut("\nIs there enough memory left for the next job? " + (Memleft >= ((jobSize * 4) + (jobIBSize * 4))));
            textOutPut("Have we reached the end of the job list? " + (CURRJOB < OSDriver.PCB.GetJobCount() + 1));

            while (Memleft >= (jobSize * 4) && (CURRJOB < OSDriver.PCB.GetJobCount()))
            {

                job.MemStart = loc;
                int v = jobStart + jobSize;

                for (int p = jobStart; p < v; p++)
                {

                    String binaryBits = OSDriver.tools.getBinaryData(p);
                    textOutPut("Binary bits: " + binaryBits);

                    string tmps = "";
                    tmps = binaryBits.Substring(0, 8);
                    int binaryBits1 = NumericConvert.BinaryToInt(tmps);
                    textOutPut(binaryBits1);
                    textOutPut(String.Format("Decimal: {0}\t added at location: {1}", binaryBits1, loc));
                    OSDriver.MemManager.WriteRamData(loc++, binaryBits1);
                    tmps = binaryBits.Substring(8, 8);
                    int binaryBits2 = NumericConvert.BinaryToInt(tmps);
                    textOutPut(binaryBits2);
                    textOutPut(String.Format("Decimal: {0}\t added at location: {1}", binaryBits2, loc));
                    OSDriver.MemManager.WriteRamData(loc++, binaryBits2);
                    tmps = binaryBits.Substring(16, 8);
                    int binaryBits3 = NumericConvert.BinaryToInt(tmps);
                    textOutPut(binaryBits3);
                    textOutPut(String.Format("Decimal: {0}\t added at location: {1}", binaryBits3, loc));
                    OSDriver.MemManager.WriteRamData(loc++, binaryBits3);
                    tmps = binaryBits.Substring(24, 8);
                    int binaryBits4 = NumericConvert.BinaryToInt(tmps);
                    textOutPut(binaryBits4);
                    textOutPut(String.Format("Decimal: {0}\t added at location: {1}", binaryBits4, loc));
                    OSDriver.MemManager.WriteRamData(loc++, binaryBits4);

                    Memleft -= 4;
                }

                textOutPut(String.Format("Data Size: {0} and location is {1}", dataSize, loc));
                int z = 0;
                int[] tmp = new int[dataSize * 4];

                String binaryDataBits;
                //Get input buffer from datafile and save it in PCB
                while (z < job.DataSize * 4)
                {
                    textOutPut("GETTING JOB DATA...");
                    binaryDataBits = ToBinaryString(v++);
                    tmp[z++] = NumericConvert.BinaryToInt(binaryDataBits.Substring(24, 8));
                    tmp[z++] = NumericConvert.BinaryToInt(binaryDataBits.Substring(16, 8));
                    tmp[z++] = NumericConvert.BinaryToInt(binaryDataBits.Substring(8, 8));
                    tmp[z++] = NumericConvert.BinaryToInt(binaryDataBits.Substring(0, 8));
                }
                job.IPBuffers = tmp;
                job.MemEnd = loc;
                job.SetStatus(ready);

                //Add the job to the ready queue and record the time.
                readyQueue.Add(job);
                job.EnQueueTime = Environment.TickCount;

                textOutPut(String.Format("Added job: {0} at address: {1}-{2}\n", CURRJOB, job.MemStart, job.MemEnd));
                if (CURRJOB >= OSDriver.PCB.GetJobCount())
                {
                    OSDriver.DONE = true;
                    return;
                }
                CURRJOB++;
                textOutPut("Is the current job < total jobs " + (CURRJOB < OSDriver.PCB.GetJobCount()));
                textOutPut(String.Format("Current job: {0}\tTotal jobs: {1}", CURRJOB, OSDriver.PCB.GetJobCount()));

                if (CURRJOB < OSDriver.PCB.GetJobCount())
                {
                    textOutPut("CURRENT JOB: " + CURRJOB);
                    job = OSDriver.PCB.GetJob(CURRJOB);
                    jobStart = job.DiskAddress;
                    jobSize = job.jobSize;
                    dataSize = job.DataSize;
                    jobIBSize = job.InBufferSize;
                }
                else
                {
                    RamPercent(1);
                    OSDriver.DONE = true;
                }
            }
            RamPercent(0);
        }

        public bool HasLoadedAllJobs()
        {
            for (int i = 0; i < OSDriver.PCB.GetJobCount(); i++)
            {
                Job tmp = OSDriver.PCB.GetJob(i);
                if (tmp.status == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void RamPercent(int f)
        {
            switch (f)
            {
                case 0:
                    average = (RAMSIZE - Memleft);
                    percent = average / RAMSIZE;

                    textOutPut("Percentage of RAM used: " + percent * 100);

                    break;

                case 1:

                    OSDriver.totalPercent = OSDriver.sumPercent / OSDriver.counter;
                    textOutPut("Total average percent of RAM used:  " + OSDriver.totalPercent * 100);
                    form.ramPercentLb.Text = "Average RAM Percent used is:" + OSDriver.totalPercent * 100;
                    break;
            }
        }

        public String ToBinaryString(int index)
        {
            textOutPut("index:" + index);
            String hexString = OSDriver.MemManager.readDiskData(index);

            // so we need to strip of the prefix 0x
            hexString = hexString.Replace("0x", "");

            // then print again to see that it's just 0000dd99
            textOutPut("Adding hexString: " + hexString);

            long t = NumericConvert.HexToLong(hexString);

            String binaryBits = Convert.ToString(t, 2);

            // then convert it to a string of bits
            textOutPut("BINARY STRING " + binaryBits);

            int length = binaryBits.Length;

            if (length < 32)
            {
                int diff = 32 - length;
                for (int i = 0; i < diff; i++)
                {
                    binaryBits = "0" + binaryBits;
                }
            }
            return binaryBits;
        }
    }
}
