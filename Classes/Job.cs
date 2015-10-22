using System;
using System.Collections.Generic;
using System.Linq;

namespace OSEmulator3
{
    class Job
    {
        public int jobID;
        public int jobSize;
        public int jobPriority;
        public int jobAddress;

        public  int inputBufferSize;
        public  int outputBufferSize;
        public  int tmpBufferSize;
        private int dataSize;
        public int status;

        private int[] IPBuffer;
        private int[] OPBuffer;
        private int[] TMPBuffer;
        public static int READY = 0;
        public static int FINISHED = 1;
        public static int LOADED = 2;
        public static int WAITING = 3;

        public long enQueueTime = 0;
        public long deQueueTime = 0;
        public long cpuStartTime = 0;
        public long cpuEndTime = 0;

        public int memStart;
        public int memEnd;

        public Job(int id, int length, int prio,int addr)
        {
            this.jobID = id;
            this.jobSize = length;
            this.jobPriority = prio;
            this.jobAddress = addr;
        }

        public void AddData(int i, int o, int t)
        {
            this.inputBufferSize = i;
            this.outputBufferSize = o;
            this.tmpBufferSize = t;
            this.IPBuffer = new int[i];
            this.OPBuffer = new int[o];
            this.TMPBuffer = new int[t];
        }

        public int DataSize
        {
            get 
            {
                return this.dataSize;
            }
            set
            {
                this.dataSize = value;
            }
        }

        public int[] IPBuffers
        {
            get
            {
                return this.IPBuffer;
            }
            set
            {
                this.IPBuffer = value;
            }
        }

        public int[] OPBuffers
        {
            get
            {
                return this.OPBuffer;
            }
            set
            {
                this.OPBuffer = value;
            }
        }

        public int[] TMPBuffers
        {
            get
            {
                return this.TMPBuffer;
            }
            set
            {
                this.TMPBuffer = value;
            }
        }

        public long EnQueueTime
        {
            get
            {
                return this.enQueueTime;
            }
            set
            {
                this.enQueueTime = value;
            }
        }

        public long DeQueueTime
        {
            get
            {
                return this.deQueueTime;
            }
            set
            {
                this.deQueueTime = value;
            }
        }

        public long CpuEndTime
        {
            get
            {
                return this.cpuEndTime;
            }
            set
            {
                this.cpuEndTime = value;
            }
        }

        public long CpuStartTime
        {
            get
            {
                return this.cpuStartTime;
            }
            set
            {
                this.cpuStartTime = value;
            }
        }

        public int MemStart
        {
            get
            {
                return this.memStart;
            }
            set
            {
                this.memStart = value;
            }
        }

        public int MemEnd
        {
            get
            {
                return this.memEnd;
            }
            set
            {
                this.memEnd = value;
            }
        }

        public int DiskAddress
        {
            get
            {
                return this.jobAddress;
            }
            set
            {
                this.jobAddress = value;
            }
        }

        public bool SetStatus(int s)
        {
            if (s == READY || s == FINISHED)
            {
                this.status = s;
                return true;
            }
            return false;
        }

        public void Terminate(int s)
        {
            this.status = s;
        }


        public int InBufferSize
        {
            get
            {
                return this.inputBufferSize;
            }
            set
            {
                this.inputBufferSize = value;
            }
        }

        public int OutBufferSize
        {
            get
            {
                return this.outputBufferSize;
            }
            set
            {
                this.outputBufferSize = value;
            }
        }

        public int TmpBufferSize
        {
            get
            {
                return this.tmpBufferSize;
            }
            set
            {
                this.tmpBufferSize = value;
            }
        }
    }
}
