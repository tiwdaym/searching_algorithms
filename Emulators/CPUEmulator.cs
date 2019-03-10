using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class CPUEmulator
    {

        public struct INSTRUCTION_SET
        {

            static Random rnd = new Random();

            public const int INSTRUCTIONS_COUNT = 79;

            public const uint MASK_INSTRUCTION = 0xff000000;
            public const uint MASK_DATA = 0x00ffffff;

            public const uint HALT = 0x00000000;

            public const uint LOAD_A_DNA = 0x01000000;
            public const uint LOAD_A_DNA_ADDRESS = 0x02000000;
            public const uint LOAD_A_IO_DATA = 0x03000000;
            public const uint LOAD_A_IO_DATA_ADDRESS = 0x04000000;
            public const uint LOAD_B_DNA = 0x05000000;
            public const uint LOAD_B_DNA_ADDRESS = 0x06000000;
            public const uint LOAD_B_IO_DATA = 0x07000000;
            public const uint LOAD_B_IO_DATA_ADDRESS = 0x08000000;

            public const uint STORE_A_DNA = 0x09000000;
            public const uint STORE_A_DNA_ADDRESS = 0x0a000000;
            public const uint STORE_A_IO_DATA = 0x0b000000;
            public const uint STORE_A_IO_DATA_ADDRESS = 0x0c000000;
            public const uint STORE_B_DNA = 0x0d000000;
            public const uint STORE_B_DNA_ADDRESS = 0x0e000000;
            public const uint STORE_B_IO_DATA = 0x0f000000;
            public const uint STORE_B_IO_DATA_ADDRESS = 0x10000000;

            public const uint JMP = 0x11000000;
            public const uint JMP_ADDRESS = 0x12000000;
            public const uint JMP_IF_A_ZERO = 0x13000000;
            public const uint JMP_IF_A_ZERO_ADDRESS = 0x14000000;
            public const uint JMP_IF_A_LESS_THAN_B = 0x15000000;
            public const uint JMP_IF_A_LESS_THAN_B_ADDRESS = 0x16000000;
            public const uint JMP_IF_A_GREATER_THAN_B = 0x17000000;
            public const uint JMP_IF_A_GREATER_THAN_B_ADDRESS = 0x18000000;
            public const uint JMP_IF_A_EQUAL_TO_B = 0x19000000;
            public const uint JMP_IF_A_EQUAL_TO_B_ADDRESS = 0x1a000000;

            public const uint ADD_A = 0x1b000000;
            public const uint ADD_A_ADDRESS = 0x1c000000;
            public const uint ADD_B = 0x1d000000;
            public const uint ADD_B_ADDRESS = 0x1e000000;
            public const uint ADD_B_TO_A = 0x1f000000;
            public const uint ADD_A_TO_B = 0x20000000;

            public const uint SUB_A = 0x21000000;
            public const uint SUB_A_ADDRESS = 0x22000000;
            public const uint SUB_B = 0x23000000;
            public const uint SUB_B_ADDRESS = 0x24000000;
            public const uint SUB_B_TO_A = 0x25000000;
            public const uint SUB_A_TO_B = 0x26000000;

            public const uint MUL_A = 0x27000000;
            public const uint MUL_A_ADDRESS = 0x28000000;
            public const uint MUL_B = 0x29000000;
            public const uint MUL_B_ADDRESS = 0x2a000000;
            public const uint MUL_B_TO_A = 0x2b000000;
            public const uint MUL_A_TO_B = 0x2c000000;

            public const uint DIV_A = 0x2d000000;
            public const uint DIV_A_ADDRESS = 0x2e000000;
            public const uint DIV_B = 0x2f000000;
            public const uint DIV_B_ADDRESS = 0x30000000;
            public const uint DIV_B_TO_A = 0x31000000;
            public const uint DIV_A_TO_B = 0x32000000;

            public const uint MOD_A = 0x33000000;
            public const uint MOD_A_ADDRESS = 0x34000000;
            public const uint MOD_B = 0x35000000;
            public const uint MOD_B_ADDRESS = 0x36000000;
            public const uint MOD_B_TO_A = 0x37000000;
            public const uint MOD_A_TO_B = 0x38000000;

            public const uint AND_A = 0x39000000;
            public const uint AND_A_ADDRESS = 0x3a000000;
            public const uint AND_B = 0x3b000000;
            public const uint AND_B_ADDRESS = 0x3c000000;
            public const uint AND_B_TO_A = 0x3d000000;
            public const uint AND_A_TO_B = 0x3e000000;

            public const uint OR_A = 0x3f000000;
            public const uint OR_A_ADDRESS = 0x40000000;
            public const uint OR_B = 0x41000000;
            public const uint OR_B_ADDRESS = 0x42000000;
            public const uint OR_B_TO_A = 0x43000000;
            public const uint OR_A_TO_B = 0x44000000;

            public const uint XOR_A = 0x45000000;
            public const uint XOR_A_ADDRESS = 0x46000000;
            public const uint XOR_B = 0x47000000;
            public const uint XOR_B_ADDRESS = 0x48000000;
            public const uint XOR_B_TO_A = 0x49000000;
            public const uint XOR_A_TO_B = 0x4a000000;

            public const uint NOT_A = 0x4b000000;
            public const uint NOT_B = 0x4c000000;

            public const uint LOG_A = 0x4d000000;
            public const uint LOG_B = 0x4e000000;

            public static uint GetRandomInstruction()
            {
                return ((uint)rnd.Next(0, INSTRUCTIONS_COUNT) << 24);
            }

            public static uint GetRandomInstruction(int randomDataRange)
            {
                 return ((uint)rnd.Next(0, INSTRUCTIONS_COUNT) << 24) | (uint)rnd.Next(randomDataRange);
            }
        }
        
        bool isProcessingChangesDisabled = false;

        ulong maxTotalInstructionsProcessed;
        public ulong MaxTotalInstructionsProcessed
        {
            get => maxTotalInstructionsProcessed;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Maximum total instructions while processing.");
                maxTotalInstructionsProcessed = value;
            }
        }

        bool createInstructionsMemoryCopyWhenEvaluating;
        public bool CreateInstructionsMemoryCopyWhenEvaluating
        {
            get => createInstructionsMemoryCopyWhenEvaluating;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change value of CreateInstructionsMemoryCopyWhenEvaluating while processing.");
                createInstructionsMemoryCopyWhenEvaluating = value;
            }
        }


        public CPUEmulator(uint maxTotalInstructionsProcessed = 300, bool createInstructionsMemoryCopyWhenEvaluating = false)
        {
            this.maxTotalInstructionsProcessed = maxTotalInstructionsProcessed;
            this.createInstructionsMemoryCopyWhenEvaluating = createInstructionsMemoryCopyWhenEvaluating;
        }


        public void RunProcessing(uint[] instructionsMemory, uint[] IO_DATA)
        {
            #region checks
            if (instructionsMemory == null) throw new ArgumentNullException("Instructions cannot be null.");
            if (IO_DATA == null) throw new ArgumentNullException("IO_DATA cannot be null.");
            isProcessingChangesDisabled = true;
            #endregion


            #region registers and Control Unit data, intitialization
            uint register_A = 0;
            uint register_B = 0;
            uint instruction_code = 0;
            uint instruction_data = 0;
            uint instruction_address = 0;

            uint instructionsLength = (uint)instructionsMemory.Length;
            uint IO_DATA_Length = (uint)IO_DATA.Length;

            ulong totalInstructionsProcessed = 0;
            bool doProcessing = true;
            if (instruction_address >= instructionsLength) doProcessing = false;

            uint[] instructionsEvaluating = instructionsMemory;
            if (createInstructionsMemoryCopyWhenEvaluating)
            {
                instructionsEvaluating = new uint[instructionsLength];
                instructionsMemory.CopyTo(instructionsEvaluating, 0);
            }
            #endregion

            //Start processing
            while (doProcessing && instruction_address < instructionsLength)
            {
                if (totalInstructionsProcessed++ >= MaxTotalInstructionsProcessed) break;

                //Get instruction and data
                instruction_code = instructionsEvaluating[instruction_address] & INSTRUCTION_SET.MASK_INSTRUCTION;
                instruction_data = instructionsEvaluating[instruction_address] & INSTRUCTION_SET.MASK_DATA;
                switch (instruction_code)
                {

                    #region LOAD INSTRUCTIONS
                    //Register A LOAD Handling
                    case INSTRUCTION_SET.LOAD_A_DNA:
                        register_A = instruction_data;
                        break;
                    case INSTRUCTION_SET.LOAD_A_DNA_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_A = instruction_data;
                        }
                        break;
                    case INSTRUCTION_SET.LOAD_A_IO_DATA:
                        if (instruction_data < IO_DATA_Length) register_A = IO_DATA[instruction_data];
                        break;
                    case INSTRUCTION_SET.LOAD_A_IO_DATA_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < IO_DATA_Length) register_A = IO_DATA[instruction_data];
                        }
                        break;

                    //Register B LOAD Handling
                    case INSTRUCTION_SET.LOAD_B_DNA:
                        register_B = instruction_data;
                        break;
                    case INSTRUCTION_SET.LOAD_B_DNA_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_B = instruction_data;
                        }
                        break;
                    case INSTRUCTION_SET.LOAD_B_IO_DATA:
                        if (instruction_data < IO_DATA_Length) register_B = IO_DATA[instruction_data];
                        break;
                    case INSTRUCTION_SET.LOAD_B_IO_DATA_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < IO_DATA_Length) register_B = IO_DATA[instruction_data];
                        }
                        break;

                    #endregion


                    #region STORE INSTRUCTIONS

                    case INSTRUCTION_SET.STORE_A_DNA:
                        if (instruction_data < instructionsLength) instructionsEvaluating[instruction_data] = register_A;
                        break;
                    case INSTRUCTION_SET.STORE_A_DNA_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < instructionsLength) instructionsEvaluating[instruction_data] = register_A;
                        }
                        break;
                    case INSTRUCTION_SET.STORE_A_IO_DATA:
                        if (instruction_data < IO_DATA_Length) IO_DATA[instruction_data] = register_A;
                        break;
                    case INSTRUCTION_SET.STORE_A_IO_DATA_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < IO_DATA_Length) IO_DATA[instruction_data] = register_A;
                        }
                        break;

                    case INSTRUCTION_SET.STORE_B_DNA:
                        if (instruction_data < instructionsLength) instructionsEvaluating[instruction_data] = register_B;
                        break;
                    case INSTRUCTION_SET.STORE_B_DNA_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < instructionsLength) instructionsEvaluating[instruction_data] = register_B;
                        }
                        break;
                    case INSTRUCTION_SET.STORE_B_IO_DATA:
                        if (instruction_data < IO_DATA_Length) IO_DATA[instruction_data] = register_B;
                        break;
                    case INSTRUCTION_SET.STORE_B_IO_DATA_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < IO_DATA_Length) IO_DATA[instruction_data] = register_B;
                        }
                        break;
                    #endregion


                    #region JMP INSTRUCTIONS
                    case INSTRUCTION_SET.JMP:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_address = instruction_data;
                            continue;
                        }
                        break;
                    case INSTRUCTION_SET.JMP_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < instructionsLength)
                            {
                                instruction_address = instruction_data;
                                continue;
                            }
                        }
                        break;

                    case INSTRUCTION_SET.JMP_IF_A_ZERO:
                        if (register_A == 0 && instruction_data < instructionsLength)
                        {
                            instruction_address = instruction_data;
                            continue;
                        }
                        break;
                    case INSTRUCTION_SET.JMP_IF_A_ZERO_ADDRESS:
                        if (register_A == 0 && instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < instructionsLength)
                            {
                                instruction_address = instruction_data;
                                continue;
                            }
                        }
                        break;

                    case INSTRUCTION_SET.JMP_IF_A_LESS_THAN_B:
                        if (register_A < register_B && instruction_data < instructionsLength)
                        {
                            instruction_address = instruction_data;
                            continue;
                        }
                        break;
                    case INSTRUCTION_SET.JMP_IF_A_LESS_THAN_B_ADDRESS:
                        if (register_A < register_B && instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < instructionsLength)
                            {
                                instruction_address = instruction_data;
                                continue;
                            }
                        }
                        break;

                    case INSTRUCTION_SET.JMP_IF_A_GREATER_THAN_B:
                        if (register_A > register_B && instruction_data < instructionsLength)
                        {
                            instruction_address = instruction_data;
                            continue;
                        }
                        break;
                    case INSTRUCTION_SET.JMP_IF_A_GREATER_THAN_B_ADDRESS:
                        if (register_A > register_B && instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < instructionsLength)
                            {
                                instruction_address = instruction_data;
                                continue;
                            }
                        }
                        break;

                    case INSTRUCTION_SET.JMP_IF_A_EQUAL_TO_B:
                        if (register_A == register_B && instruction_data < instructionsLength)
                        {
                            instruction_address = instruction_data;
                            continue;
                        }
                        break;
                    case INSTRUCTION_SET.JMP_IF_A_EQUAL_TO_B_ADDRESS:
                        if (register_A == register_B && instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data < instructionsLength)
                            {
                                instruction_address = instruction_data;
                                continue;
                            }
                        }
                        break;


                    #endregion


                    #region ADD INSTRUCTIONS

                    case INSTRUCTION_SET.ADD_A:
                        register_A += instruction_data;
                        break;
                    case INSTRUCTION_SET.ADD_A_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_A += instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.ADD_B:
                        register_B += instruction_data;
                        break;
                    case INSTRUCTION_SET.ADD_B_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_B += instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.ADD_B_TO_A:
                        register_A += register_B;
                        break;
                    case INSTRUCTION_SET.ADD_A_TO_B:
                        register_B += register_A;
                        break;

                    #endregion


                    #region SUB INSTRUCTIONS

                    case INSTRUCTION_SET.SUB_A:
                        register_A -= instruction_data;
                        break;
                    case INSTRUCTION_SET.SUB_A_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_A -= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.SUB_B:
                        register_B -= instruction_data;
                        break;
                    case INSTRUCTION_SET.SUB_B_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_B -= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.SUB_B_TO_A:
                        register_A -= register_B;
                        break;
                    case INSTRUCTION_SET.SUB_A_TO_B:
                        register_B -= register_A;
                        break;

                    #endregion


                    #region MUL INSTRUCTIONS

                    case INSTRUCTION_SET.MUL_A:
                        register_A *= instruction_data;
                        break;
                    case INSTRUCTION_SET.MUL_A_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_A *= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.MUL_B:
                        register_B *= instruction_data;
                        break;
                    case INSTRUCTION_SET.MUL_B_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_B *= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.MUL_B_TO_A:
                        register_A *= register_B;
                        break;
                    case INSTRUCTION_SET.MUL_A_TO_B:
                        register_B *= register_A;
                        break;

                    #endregion


                    #region DIV INSTRUCTIONS

                    case INSTRUCTION_SET.DIV_A:
                        if (instruction_data != 0) register_A /= instruction_data;
                        break;
                    case INSTRUCTION_SET.DIV_A_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data != 0) register_A /= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.DIV_B:
                        if (instruction_data != 0) register_B /= instruction_data;
                        break;
                    case INSTRUCTION_SET.DIV_B_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data != 0) register_B /= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.DIV_B_TO_A:
                        if (register_B != 0) register_A /= register_B;
                        break;
                    case INSTRUCTION_SET.DIV_A_TO_B:
                        if (register_A != 0) register_B /= register_A;
                        break;

                    #endregion


                    #region MOD INSTRUCTIONS

                    case INSTRUCTION_SET.MOD_A:
                        if (instruction_data != 0) register_A %= instruction_data;
                        break;
                    case INSTRUCTION_SET.MOD_A_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data != 0) register_A %= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.MOD_B:
                        if (instruction_data != 0) register_B %= instruction_data;
                        break;
                    case INSTRUCTION_SET.MOD_B_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            if (instruction_data != 0) register_B %= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.MOD_B_TO_A:
                        if (register_B != 0) register_A %= register_B;
                        break;
                    case INSTRUCTION_SET.MOD_A_TO_B:
                        if (register_A != 0) register_B %= register_A;
                        break;

                    #endregion


                    #region AND INSTRUCTIONS

                    case INSTRUCTION_SET.AND_A:
                        register_A &= instruction_data;
                        break;
                    case INSTRUCTION_SET.AND_A_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_A &= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.AND_B:
                        register_B &= instruction_data;
                        break;
                    case INSTRUCTION_SET.AND_B_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_B &= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.AND_B_TO_A:
                        register_A &= register_B;
                        break;
                    case INSTRUCTION_SET.AND_A_TO_B:
                        register_B &= register_A;
                        break;

                    #endregion


                    #region OR INSTRUCTIONS

                    case INSTRUCTION_SET.OR_A:
                        register_A |= instruction_data;
                        break;
                    case INSTRUCTION_SET.OR_A_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_A |= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.OR_B:
                        register_B |= instruction_data;
                        break;
                    case INSTRUCTION_SET.OR_B_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_B |= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.OR_B_TO_A:
                        register_A |= register_B;
                        break;
                    case INSTRUCTION_SET.OR_A_TO_B:
                        register_B |= register_A;
                        break;

                    #endregion


                    #region XOR INSTRUCTIONS

                    case INSTRUCTION_SET.XOR_A:
                        register_A ^= instruction_data;
                        break;
                    case INSTRUCTION_SET.XOR_A_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_A ^= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.XOR_B:
                        register_B ^= instruction_data;
                        break;
                    case INSTRUCTION_SET.XOR_B_ADDRESS:
                        if (instruction_data < instructionsLength)
                        {
                            instruction_data = instructionsEvaluating[instruction_data];
                            register_B ^= instruction_data;
                        }
                        break;

                    case INSTRUCTION_SET.XOR_B_TO_A:
                        register_A ^= register_B;
                        break;
                    case INSTRUCTION_SET.XOR_A_TO_B:
                        register_B ^= register_A;
                        break;

                    #endregion


                    #region NOT INSTRUCTIONS
                    case INSTRUCTION_SET.NOT_A:
                        register_A ^= register_A;
                        break;
                    case INSTRUCTION_SET.NOT_B:
                        register_B ^= register_B;
                        break;
                    #endregion


                    #region LOG INSTRUCTIONS
                    case INSTRUCTION_SET.LOG_A:
                        register_A = (uint)Math.Round(Math.Log(register_A, 2));
                        break;
                    case INSTRUCTION_SET.LOG_B:
                        register_B = (uint)Math.Round(Math.Log(register_B, 2));
                        break;
                    #endregion


                    #region CUSTOM INSTRUCTIONS

                    case INSTRUCTION_SET.HALT:
                        doProcessing = false;
                        break;
                    default:
                        doProcessing = false;
                        break;

                        #endregion
                }

                //Increment instruction address if no continue
                instruction_address++;
            }

            isProcessingChangesDisabled = false;
        }

        public void ResetProcessing()
        {
            isProcessingChangesDisabled = false;
        }
    }
}
