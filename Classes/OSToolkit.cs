using System;
using System.Collections.Generic;
using System.Linq;

namespace OSEmulator3
{
    class OSToolkit
    {
        public int effective_addrs;
        public int INDIRECT;
        public int DIRECT;
        public OSToolkit()
        {
            this.effective_addrs = 0;
            INDIRECT = 0;
            DIRECT = 1;
        }

        public int effective_address(int i, int b)
        {
            return i + b;
        }
        public int effective_addr(int flag, byte[] b, String D)
        {
            int offset = NumericConvert.BinaryToInt(D);
            if (flag == INDIRECT)
            {
                effective_addrs = content(b[0]) + content(b[1]) + offset;
            }
            else if (flag == DIRECT)
            {
                effective_addrs = content(b[0]) + offset;
            }
            return effective_addrs;
        }
        public string hexToByte(String h)
        {
            String tmp = h.Substring(2);
            string bin = NumericConvert.HexToBinary(tmp);
            return bin;
        }
        public int content(int b)
        {
            return OSDriver.MemManager.ReadRamData(b);
        }

        public String getBinaryData(int index)
        {
            String hexString = OSDriver.MemManager.readDiskData(index);

            hexString = hexString.Substring(2, 8);

            long t = NumericConvert.HexToLong(hexString);

            String binaryBits = Convert.ToString(t, 2);

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

        public Boolean hasLoadedAllJobs()
        {
            for (int i = 0; i < OSDriver.PCB.GetJobCount(); i++)
            {
                Job tmp = OSDriver.PCB.GetJob(i);
                if (tmp.status < 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
