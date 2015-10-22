using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace OSEmulator3
{
    class Loader : IDisposable
    {
        public StreamReader  sr2;
        public static int count = 0;
        public int addr;
        private Form1 form;
        public void Dispose()
        {
            if (sr2 != null)
            {
                sr2.Dispose();
                sr2 = null;
            }
        }
        public Loader(Form1 form1,string file)
        {
            try
            {
                this.form = form1;
                FileStream fs2 = new FileStream(file, FileMode.Open);
                this.sr2 = new StreamReader(fs2);
            }
            catch
            {
                textOutPut("System Error!Cannot Open File!System Will Exit!");
                return;
            }
        }
        public void textOutPut(string content)
        {
            form.outputTxt.AppendText(content + "\n");
        }
        public void Start()
        {
            addr = 0;
            try
            {
                string str = sr2.ReadLine();
                while (str != "#")
                {
                    //processData(str);
                    textOutPut("trying to load datafile");
                    if (str.Contains("JOB"))
                    {
                        textOutPut(str);
                        str = str.Replace("// JOB ", "");
                        //textOutPut(str);

                        AddJob(str, 0);
                        str = sr2.ReadLine();

                        while (!str.Contains("//"))
                        {
                            textOutPut(String.Format("adding {0} to location {1}", str, count));
                            AddData(str, 0, count++);
                            str = sr2.ReadLine();
                        }

                    }
                    else if (str.Contains("Data"))
                    {
                        textOutPut(str);
                        str = str.Replace("// Data ", "");
                        AddJob(str, 1);
                        str = sr2.ReadLine();

                        while (!str.Contains("//"))
                        {
                            AddData(str, 0, count++);
                            str = sr2.ReadLine();
                        }
                    }
                    else
                    {
                        str = sr2.ReadLine(); 
                        if (str == null || str == "#")
                        {
                            return;
                        }
                        textOutPut("Read data: " + str);
                    }
                }
            }
            catch
            {
                textOutPut("System Error!Cannot Open File!System Will Exit!");
                return;
            }
        }
        public void AddJob(string s, int choice)
        {
            string[] parts = s.Split(' ');
            if (choice == 0)
            {
                int jobID = NumericConvert.HexToInt(parts[0]);
                int jobSize = NumericConvert.HexToInt(parts[1]);
                int jobPrio = NumericConvert.HexToInt(parts[2]);
                int jobAddr = addr;
                OSDriver.PCB.createJob(jobID, jobSize, jobPrio, jobAddr);
            }
            else
            {
                int input = NumericConvert.HexToInt(parts[0]);
                int output = NumericConvert.HexToInt(parts[1]);
                int temp = NumericConvert.HexToInt(parts[2]);
                OSDriver.PCB.addMeta(input, output, temp);
                OSDriver.PCB.SetDataSize(input + output + temp);
            }
        }

        public void AddData(string s, int choice, int loc)
        {
            if (choice == 0)
            {
                OSDriver.MemManager.WriteDiskData(loc, s);
                addr++;
            }

        }
    }
}
