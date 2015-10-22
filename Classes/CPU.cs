using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace OSEmulator3
{
    class CPU : IDisposable
    {
        public int opCode;
        public int type;
        public int s1_reg;
        public int s2_reg;
        public int d_reg;
        public int b_reg;
        public int reg1;
        public int reg2;

        public Boolean jumped;

        public static int data = 0;
        public int jobSize;

        public int oBufferSize;
        public int iBufferSize;
        public int tBufferSize;

        public int[] cpu_buffer;

        public long address;

        public int[] reg_Array;
        public int pc;
        public static int ACCUM = 0;
        public static int RUNNING = 2;
        public static int ZERO = 1;
        public StreamWriter output;

        public static int ioCount;
        public Job j;

        private Form1 form;

        public void Dispose()
        {
            if (output != null)
            {
                output.Dispose();
                output = null;
            }
        }
        public CPU()
        {
            ioCount = 0;

            jumped = false;
            //Create registers and make the accumulator
            reg_Array = new int[16];
            reg_Array[ZERO] = 0;

            output = new StreamWriter(new FileStream("CPU_log.txt", FileMode.Create));
            output.WriteLine("CPU LOG FILE");
        }

        public void Load(Job job)
        {
            j = job;
            ioCount = 0;
            output.WriteLine("\n|||||||||||||||||");
            output.WriteLine("\nJob #" + j.jobID);
            textOutPut(String.Format("\nJob #{0} EXECUTING", j.jobID));

            //set the pc counter & buffer sizes
            pc = j.MemStart;
            oBufferSize = j.OutBufferSize;//size in # of words
            iBufferSize = j.inputBufferSize;
            tBufferSize = j.TmpBufferSize;
            cpu_buffer = j.IPBuffers;
            jobSize = j.jobSize;
            textOutPut(String.Format("Program Counter starting at: {0}\n", pc));
            //run the duration of the job
            while (pc < j.MemEnd)
            {
                String instr = FetchInstruction(pc);
                execute(Decode(instr), j.jobID);
                if (!jumped)
                    pc += 4;
                else
                    jumped = false;

                output.WriteLine("\n\nPROGRAM COUNTER=" + pc);
                textOutPut("PROGRAM COUNTER=" + pc);
            }
        }

        public String FetchInstruction(int pc)
        {
            try
            {
                output.WriteLine("\nfetching instruction..");
            }
            catch (IOException ioe)
            {
                textOutPut(ioe.StackTrace);
            }
            //create a new string to hold the instruction
            String instruction;
            String returnedInst = "";

            //loop 4 times to get all pieces of the word
            for (int i = 0; i < 4; i++)
            {
                //get binary represenation of value.
                instruction = Convert.ToString(OSDriver.MemManager.ReadRamData(pc++), 2);
                textOutPut("AFTER EXTRACTION " + instruction);
                //add any leading zeros that were left off by the previous operation
                int b = instruction.Length;
                if (instruction.Length < 8)
                {
                    for (int y = 0; y < 8 - b; y++)
                    {
                        instruction = "0" + instruction;
                    }
                    returnedInst = String.Format("{0}{1}", returnedInst, instruction);
                }
                else
                {
                    returnedInst = String.Format("{0}{1}", returnedInst, instruction);
                }
                textOutPut("AFTER EXTRACTION & APPENDING " + returnedInst);
            }
            return returnedInst;

        }

        public int Decode(String instr_req)
        {
            textOutPut("\nBinary instruction: " + instr_req);
            output.WriteLine("decoding instruction.... " + instr_req);

            //EXTRACT THE TYPE AND OPCODE FROM THE INSTRUCTION
            this.type = NumericConvert.BinaryToInt(instr_req.Substring(0, 2));
            this.opCode = NumericConvert.BinaryToInt(instr_req.Substring(2, 6));
            output.WriteLine(String.Format("\nTYPE: {0}\tOPCODE: {1}", type, opCode));
            //USE TYPE TO DETERMINE HOW TO EXTRACT THE REMAINING COMPONENTS
            output.WriteLine("\nInstruction type:");
            switch (type)
            {
                case 0:
                    output.WriteLine(" arithmetic");
                    s1_reg = NumericConvert.BinaryToInt(instr_req.Substring(8, 4));
                    s2_reg = NumericConvert.BinaryToInt(instr_req.Substring(12, 4));
                    d_reg = NumericConvert.BinaryToInt(instr_req.Substring(16, 4));
                    textOutPut(String.Format("s1_reg:{0} s2_reg:{1} d_reg:{2}", s1_reg, s2_reg, d_reg));
                    break;

                case 1:
                    output.WriteLine(" branch of immediate");
                    b_reg = NumericConvert.BinaryToInt(instr_req.Substring(8, 4));
                    d_reg = NumericConvert.BinaryToInt(instr_req.Substring(12, 4));
                    address = NumericConvert.BinaryToLong(instr_req.Substring(16,16));
                    textOutPut(String.Format("b_reg:{0} d_reg:{1} address:{2}", b_reg, d_reg, address));
                    break;

                case 2:
                    output.WriteLine(" jump");
                    address = NumericConvert.BinaryToLong(instr_req.Substring(8, 24));
                    textOutPut("JUMP ADDRESS: " + address);
                    break;

                case 3:
                    output.WriteLine(" IO");
                    reg1 = NumericConvert.BinaryToInt(instr_req.Substring(8, 4));
                    reg2 = NumericConvert.BinaryToInt(instr_req.Substring(12, 4));
                    address = NumericConvert.BinaryToLong(instr_req.Substring(16,16));
                    textOutPut(String.Format("Reg1: {0} Reg2: {1} Address: {2}", reg1, reg2, address));
                    ioCount++;
                    break;

                default:
                    output.WriteLine("\nERROR: HIT DEFAULT DECODE TYPE");
                    break;

            }

            return opCode;
        }

        public void execute(int o, int jID)
        {
            output.WriteLine(String.Format("\nExecuting instruction.... OPCODE = {0}", o));
            textOutPut(String.Format("\nExecuting instruction.... OPCODE = {0}", o));

            if (!(opCode < 0) || (opCode > 26))
            {
                switch (opCode)
                {

                    case 0:
                        output.WriteLine("\nPutting input buffer contents into accumulator");
                        if (address > 0)
                        {
                            //Reads content of I/P buffer into a accumulator
                            reg_Array[reg1] = cpu_buffer[buff_address((int)address)];
                        }
                        else
                        {
                            reg_Array[reg1] = reg_Array[reg2];
                        }
                        textOutPut(String.Format("Register {0} now Contains {1}", reg1, reg_Array[reg1]));
                        break;

                    case 1:
                        output.WriteLine(String.Format("\nWriting content of accumulator {0} into outputput buffer", reg_Array[ACCUM]));
                        textOutPut("Writing content of accumulator into outputput buffer");
                        //Writes the content of accumulator into O/P buffer
                        cpu_buffer[buff_address((int)address)] = (int)reg_Array[reg2];
                        textOutPut(String.Format("Writing {0} into outputput buffer", reg_Array[ACCUM]));
                        break;

                    case 2:
                        output.WriteLine("\nStoring register in address");
                        textOutPut("Storing register in address");
                        //Stores content of a reg.  into an address
                        reg_Array[(int)address] += reg_Array[(int)d_reg];
                        textOutPut(String.Format("r_index: {0} now Contains {1}", address, reg_Array[(int)d_reg]));
                        break;

                    case 3:
                        output.WriteLine("\nLoading address into register");
                        textOutPut("Loading address into register");
                        //Loads the content of an address into a reg.
                        reg_Array[d_reg] = reg_Array[effective_address(b_reg, address) % 16];
                        textOutPut(String.Format("r_index: {0} now Contains {1}", d_reg, reg_Array[d_reg]));
                        break;

                    case 4:
                        output.WriteLine("\nSwapping registers");
                        textOutPut("Swapping registers");
                        //Transfers the content of one register into another
                        calc_arith(6);
                        break;

                    case 5:
                        output.WriteLine("\nAdding s_regs into d_reg");
                        textOutPut("Adding s_regs into d_reg");
                        //Adds content of two S-regs into D-reg
                        calc_arith(0);
                        break;

                    case 6:
                        output.WriteLine("\nSubtracting s_regs into d_reg");
                        textOutPut("Subtracting s_regs into d_reg");
                        //Subtracts content of two S-regs into D-reg
                        calc_arith(1);
                        break;

                    case 7:
                        output.WriteLine("\nMultiplying s_regs into d_reg");
                        textOutPut("Multiplying s_regs into d_reg");
                        //Multiplies content of two S-regs into D-reg
                        calc_arith(2);
                        break;

                    case 8:
                        output.WriteLine("\nDividing s_regs into d_reg");
                        textOutPut("Dividing s_regs into d_reg");
                        //Divides content of two S-regs into D-reg
                        calc_arith(3);
                        break;

                    case 9:
                        output.WriteLine("\nLogical AND of s_regs");
                        textOutPut("Logical AND of s_regs");
                        //Logical AND of two S-regs into D-reg
                        calc_arith(4);
                        break;

                    case 10:
                        output.WriteLine("\nLogical OR of s_regs");
                        textOutPut("Logical OR of s_regs");
                        //Logical OR of two S-regs into D-reg
                        calc_arith(5);
                        break;

                    case 11:
                        output.WriteLine("\nTransferring data into register");
                        textOutPut("Transferring data into register");
                        reg_Array[d_reg] = cpu_buffer[buff_address((int)address)];
                        textOutPut(String.Format("r_index: {0} now Contains {1}", d_reg, reg_Array[d_reg]));
                        //immediate - MOVI
                        break;

                    case 12:
                        output.WriteLine("\nAdding data into register");
                        textOutPut("Adding data into register");
                        reg_Array[d_reg] += (int)address;
                        textOutPut(String.Format("r_index: {0} now Contains {1}", d_reg, reg_Array[d_reg]));
                        //Adds a data directly to the content of a register
                        //ADDI
                        break;

                    case 13:
                        output.WriteLine("\nMultiplying data into register");
                        textOutPut("Multiplying data into register");
                        reg_Array[d_reg] *= (int)address;
                        //Multiplies a data directly to the content of a register
                        textOutPut(String.Format("r_index: {0} now Contains {1}", d_reg, reg_Array[d_reg]));
                        //MULI
                        break;

                    case 14:
                        output.WriteLine("\nDividing data into register");
                        textOutPut("Dividing data into register");
                        reg_Array[d_reg] /= (int)address;
                        textOutPut(String.Format("r_index: {0} now Contains {1}", d_reg, reg_Array[d_reg]));
                        //Divides a data directly to the content of a register
                        //DIVI
                        break;

                    case 15:
                        output.WriteLine("\nLoading data/address into register");
                        textOutPut("Loading data/address into register");
                        reg_Array[d_reg] = (int)address;
                        textOutPut(String.Format("r_index: {0} now Contains {1}", d_reg, reg_Array[d_reg]));
                        //Loads a data/address directly to the content of a register
                        break;

                    case 16:
                        //Sets the D-reg to 1 if first S-reg is less than second S-reg, and 0 otherwise
                        output.WriteLine(String.Format("\nChecking if {0} < {1}", reg_Array[s1_reg], reg_Array[s2_reg]));
                        textOutPut(String.Format("Checking if {0} < {1}", reg_Array[s1_reg], reg_Array[s2_reg]));
                        if (reg_Array[s1_reg] < reg_Array[s2_reg])
                        {
                            reg_Array[d_reg] = 1;
                        }
                        else
                        {
                            reg_Array[d_reg] = 0;
                        }
                        output.WriteLine(String.Format("\nr_index: {0} is now {1}", d_reg, reg_Array[d_reg]));
                        textOutPut(String.Format("r_index: {0} is now {1}", d_reg, reg_Array[d_reg]));
                        break;

                    case 17:
                        //Sets the D-reg to 1 if first S-reg is less than a data, and 0 otherwise
                        output.WriteLine(String.Format("\nChecking if {0} < {1}", s1_reg, data));
                        textOutPut(String.Format("Checking if {0} < {1}", s1_reg, data));
                        if (reg_Array[s1_reg] < (int)address)
                        {
                            reg_Array[d_reg] = 1;
                        }
                        else
                        {
                            reg_Array[d_reg] = 0;
                        }
                        output.WriteLine(String.Format("\nr_index: {0} is now {1}", d_reg, reg_Array[d_reg]));
                        textOutPut(String.Format("r_index: {0} is now {1}", d_reg, reg_Array[d_reg]));
                        break;

                    case 18:
                        output.WriteLine("\nEnd of program detected!");
                        textOutPut("END OF PROGRAM - OPCODE " + opCode);
                        Job tmp = OSDriver.PCB.GetJob(jID - 1);
                        tmp.CpuEndTime = Environment.TickCount;
                        tmp.SetStatus(1);

                        double wt = (double)(tmp.DeQueueTime - tmp.EnQueueTime) / 1000000;
                        double ct = (double)(tmp.CpuEndTime - tmp.CpuStartTime) / 1000000;
                        form.dataGridView1.Rows.Add(jID, wt, ct, ioCount);
                        String avgs = String.Format("JOB #{0} was waiting for {1} milliseconds and had a CPU time of {2} milliseconds.", jID, wt, ct);
                        textOutPut(avgs);
                        output.WriteLine("\n" + avgs);

                        String ios = String.Format("There were {0} IO requests in job # {1}", ioCount, jID);
                        textOutPut(ios);
                        output.WriteLine("\n" + ios);

                        break;

                    case 19:
                        output.WriteLine("\nMoving to the next instruction");
                        textOutPut("Moving to the next instruction");
                        //Does nothing and moves to next instruction
                        break;

                    case 20:
                        output.WriteLine("\nJumping to another location");
                        textOutPut("Jumping to another location");
                        //Jumps to a specified location
                        pc = (int)address;
                        jumped = true;
                        output.WriteLine("\nProgram counter set to " + pc);
                        textOutPut("Program counter set to " + pc);
                        break;

                    case 21:
                        output.WriteLine("\nChecking if b_reg = d_reg, then branch");
                        textOutPut(String.Format("Checking if {0} = {1} , then branch", reg_Array[b_reg], reg_Array[d_reg]));
                        //Branches to an address when content of B-reg = D-reg
                        if (reg_Array[d_reg] == reg_Array[b_reg])
                        {
                            pc = (int)address;
                            pc += j.MemStart;
                            textOutPut("Program counter set to " + pc);
                            output.WriteLine("\nProgram counter set to " + pc);
                        }
                        break;

                    case 22:
                        output.WriteLine("\nChecking if b_reg != d_reg, then branch");
                        textOutPut("Checking if b_reg != d_reg, then branch");
                        textOutPut(String.Format("b_reg: {0} d_reg:{1}", reg_Array[b_reg], reg_Array[d_reg]));
                        if (reg_Array[b_reg] != reg_Array[d_reg])
                        {
                            pc = (int)address;
                            pc += j.MemStart;
                            textOutPut("Program counter set to " + pc);
                            output.WriteLine("\nProgram counter set to " + pc);
                        }
                        break;

                    case 23:
                        output.WriteLine("\nChecking if d_reg is 0, then branch");
                        textOutPut("Checking if d_reg is 0, then branch");
                        //Branches to an address when content of D-reg = 0
                        if (reg_Array[d_reg] == 0)
                        {
                            pc = (int)address;
                            pc += j.MemStart;
                            textOutPut("Program counter set to " + pc);
                            output.WriteLine("\nProgram counter set to " + pc);
                        }
                        break;

                    case 24:
                        output.WriteLine("\nChecking if b_reg != 0, then branch");
                        textOutPut("Checking if b_reg != 0, then branch");
                        //Branches to an address when content of B-reg <> 0
                        if (reg_Array[b_reg] != 0)
                        {
                            pc = (int)address;
                            pc += j.MemStart;
                            output.WriteLine("\nProgram counter set to " + pc);
                            textOutPut("Program counter set to " + pc);
                        }
                        break;

                    case 25:
                        output.WriteLine("\nChecking if b_reg > 0, then branch");
                        textOutPut("Checking if b_reg > 0, then branch");
                        //Branches to an address when content of B-reg > 0
                        if (reg_Array[b_reg] > 0)
                        {
                            pc = (int)address;
                            pc += j.MemStart;
                            output.WriteLine("\nProgram counter set to " + pc);
                            textOutPut("Program counter set to " + pc);
                        }
                        break;

                    case 26:
                        output.WriteLine("\nChecking if b_reg < 0, then branch");
                        textOutPut("Checking if b_reg < 0, then branch");
                        //Branches to an address when content of B-reg < 0
                        if (reg_Array[b_reg] < 0)
                        {
                            pc = (int)address;
                            pc += j.MemStart;

                            textOutPut("Program counter set to " + pc);
                            output.WriteLine("\nProgram counter set to " + pc);
                        }
                        break;

                    default:
                        textOutPut("UNKNOWN OPCODE");
                        break;

                }
            }
            else
            {
                output.WriteLine("\nDIDN'T DECODE... OPCODE = " + opCode);
                textOutPut("DIDN'T DECODE... OPCDOE = " + opCode);
            }
        }

        public void calc_arith(int i)
        {

            // i=0 - ADD
            // i=1 - SUBTRACT
            // i=2 - MULTIPLY
            // i=3 - DIVIDE
            // i=4 - LOGICAL 'AND'
            // I=5 - LOGICAL 'OR'
            // i=6 - SWAP REGISTERS

            switch (i)
            {
                case 0:
                    d_reg = (int)(s1_reg + s2_reg);
                    output.WriteLine(String.Format("\ns1_reg: {0} + s2_reg: {1}\tmakes d_reg: {2}", s1_reg, s2_reg, d_reg));
                    break;
                case 1:
                    d_reg = (int)(s1_reg - s2_reg);
                    output.WriteLine(String.Format("\ns1_reg: {0} - s2_reg: {1}\tmakes d_reg: {2}", s1_reg, s2_reg, d_reg));
                    break;
                case 2:
                    d_reg = (int)(s1_reg * s2_reg);
                    output.WriteLine(String.Format("\ns1_reg: {0} * s2_reg: {1}\tmakes d_reg: {2}", s1_reg, s2_reg, d_reg));
                    break;
                case 3:
                    if (s2_reg == 0)
                        return;
                    else
                    {
                        d_reg = (int)(s1_reg / s2_reg);
                        output.WriteLine(String.Format("\ns1_reg: {0} / s2_reg: {1}\tmakes d_reg: {2}", s1_reg, s2_reg, d_reg));
                    }
                    break;
                case 4:
                    d_reg = (int)(s1_reg & s2_reg);
                    output.WriteLine(String.Format("\ns1_reg: {0} LOGICAL AND'd with s2_reg: {1}\tmakes d_reg: {2}", s1_reg, s2_reg, d_reg));
                    break;
                case 5:
                    d_reg = (int)(s1_reg | s2_reg);
                    output.WriteLine(String.Format("\ns1_reg: {0} LOGICAL OR'd with s2_reg: {1}\tmakes d_reg: {2}", s1_reg, s2_reg, d_reg));
                    break;
                case 6:
                    int tmp_reg = s1_reg;
                    s1_reg = s2_reg;
                    s2_reg = tmp_reg;
                    output.WriteLine("\ns1_reg is now: " + s1_reg);
                    output.WriteLine("\ns2_reg is now: " + s2_reg);
                    break;
                default:
                    output.WriteLine("\nDEFAULT CALC_ARITH SWITCH REACHED");
                    textOutPut("DEFAULT CALC_ARITH SWITCH REACHED");
                    break;
            }
        }
        public int extract(int oc)
        {

            if (oc == 1)
            {

            }
            return 1;
        }
        public int buff_address(int a)
        {
            return Math.Abs(a - jobSize * 4);
        }
        public int effective_address(int i, long a)
        {
            return reg_Array[i] + (int)a;
        }

        internal void Load(Form1 form1, Job jMeta)
        {
            this.form = form1;
            Load(jMeta);
        }

        private void textOutPut(string content)
        {
            form.outputTxt.AppendText(content + "\n");
            Console.WriteLine(content + "\n");
        }
    }
}
