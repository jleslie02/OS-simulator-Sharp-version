using System;
using System.Collections.Generic;
using System.Linq;

namespace OSEmulator3
{
    class RamMemory
    {
        public int[] ram;
        public RamMemory(int s)
        {
            ram = new int[s];
        }
        public void WriteData(int loc, int data)
        {
            ram[loc] = data;
        }
        public int ReadData(int i)
        {
            return ram[i];
        }
        public string ToString()
        {
            for (int i = 0; i < ram.Length; i++)
            {
                Console.WriteLine(ram[i].ToString());
            }
            return ram.ToString();
        }
    }
}
