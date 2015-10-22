using System;
using System.Collections.Generic;
using System.Linq;

namespace OSEmulator3
{
    class DiskMemory
    {
        public String[] disk;
        public DiskMemory(int s)
        {
            disk = new String[s];
        }
        public void WriteData(int loc, String data)
        {
            disk[loc] = data;
        }
        public String ReadData(int i)
        {
            return disk[i];
        }

        public String ToString()
        {
            Console.WriteLine(disk.ToString());
            return disk.ToString();
        }
    }
}
