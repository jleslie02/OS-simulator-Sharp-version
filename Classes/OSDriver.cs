using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace OSEmulator3
{
    class OSDriver : IDisposable
    {
        public static Loader loader;
        public static MemoryManager MemManager;
        public static ProcessControlBlock PCB;
        public static OSToolkit tools;
        public static LongTermScheduler LTS;
        public static ShortTermScheduler STS;
        public static int totalJobSize = 0;
        public static Boolean DONE = false;
        public static double sumPercent;
        public static int counter = 0;
        public static double totalPercent;

        private Form1 form;

        public void Dispose()
        {
            if (form != null)
            {
                form.Dispose();
                form = null;
            }
        }

        public void Start()
        {
            MemManager = new MemoryManager();
            PCB = new ProcessControlBlock(form);
            tools = new OSToolkit();
            try
            {
                loader = new Loader(form,@"..\..\DataFiles\DataFile.txt");
                try
                {
                    loader.Start();
                }
                catch (IOException ioe)
                {
                    Console.WriteLine(ioe.StackTrace);
                }
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine(fnfe.StackTrace);
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.StackTrace);
            }

            LTS = new LongTermScheduler(form);
            STS = new ShortTermScheduler(form);
            STS.SJF();
            Job jMeta;

            CPU cpu1 = null;
            try
            {
                cpu1 = new CPU();
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.StackTrace);
            }
            while (!DONE || LongTermScheduler.readyQueue.Count > 0)
            {
                sumPercent += LongTermScheduler.percent;
                counter++;
                if (LongTermScheduler.readyQueue.Count > 0)
                {
                    Console.WriteLine("NUMBER OF JOBS: " + LongTermScheduler.readyQueue.Count);
                    form.outputTxt.Text += "NUMBER OF JOBS: " + LongTermScheduler.readyQueue.Count;
                    while (LongTermScheduler.readyQueue.Count > 0)
                    {
                        jMeta = STS.Store(0);
                        try
                        {
                            cpu1.Load(form, jMeta);
                        }
                        catch (IOException ioe)
                        {
                            Console.WriteLine(ioe.StackTrace);
                        }
                    }
                    Console.WriteLine("\nADDING MORE JOBS........\n");
                    form.outputTxt.Text += "\nADDING MORE JOBS........\n";
                    LTS.Start();
                    STS.SJF();
                }
            }
        }

        internal void Start(Form1 form1)
        {
            form = form1;
            Start();
        }
    }
}
