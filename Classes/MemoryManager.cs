using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OSEmulator3
{
    class MemoryManager
    {
        public DiskMemory disk;
        public RamMemory ram;
        public MemoryManager()
        {
            disk = new DiskMemory(2048);
            ram = new RamMemory(1024);
        }

        //  This writes the given data to the disk starting at the location provided.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteDiskData(int loc, String data)
        {

            disk.WriteData(loc, data);

        }

        //  Returns a string representation of the hex code
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string readDiskData(int r)
        {
            return disk.ReadData(r);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteRamData(int loc, int data)
        {
            ram.WriteData(loc, data);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int ReadRamData(int r)
        {
            return ram.ReadData(r);
        }

        public String PrintDisk()
        {
            return disk.ToString();
        }

        public String PrintRam()
        {
            return ram.ToString();
        }
    }
}
