using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CleanCode
{
    public class IntcodeComputer
    {
        public List<long> IntList { get; set; }
        public IntCodeStates State { get; set;}
        private readonly string _intcodeProgram;
        public string OutputProgram => string.Join(",", IntList);
        public long Output => IntList[0];
        public IntcodeIoHandler IntcodeIoHandler { get; set; }
        public int Pointer { get; set; }
        public long RelativeBase { get; set; }
        private readonly InstructionFactory _instructionFactory;
        private readonly InstructionService _instructionService;

        public IntcodeComputer(string programCode)
        {
            _intcodeProgram = programCode;
            IntList = programCode.Split(',').Select(long.Parse).ToList();
            State = IntCodeStates.Initialised;
            Pointer = 0;
            RelativeBase = 0;
            _instructionFactory = new InstructionFactory();
            _instructionService = new InstructionService(new ReferenceValueService());
            IntcodeIoHandler = new IntcodeIoHandler();
        }
        
        public IntcodeComputer(string programCode, IntcodeIoHandler intcodeIoHandler) :this (programCode)
        {
            IntcodeIoHandler = intcodeIoHandler;
        }
        public void RunProgramUntilPause()
        {
            State = IntCodeStates.Running;
            while (State == IntCodeStates.Running)
            {
                this.LogComputerState();
                var instruction = _instructionFactory.CreateInstruction(IntList, Pointer);
                
                if (instruction.OpCode == 99)
                {
                    State = IntCodeStates.Halted;
                    Console.WriteLine("Halted");
                    return;
                }

                var instructionResponse = _instructionService.HandleInstruction(this, instruction);
                if (instructionResponse.WasInstructionSuccess)
                {
                    Pointer += instruction.Length;
                }
                else
                {
                    switch (instructionResponse.FailureReason)
                    {
                        case FailureReason.CouldNotAccessMemoryAddress:
                            DoubleMemory();
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }   
                }
            }
        }
        public string CalcNounVerb()
        {
            for (var a = 0; a < 100; a++)
            {
                for (var j = 0; j < 100; j++)
                {
                    InitialiseIntList();
                    InitNounVerb(a,j);
                    ResetComputer();
                    RunProgramUntilPause();
                    if (Output == 19690720)
                    {
                        return a.ToString("D2") + j.ToString("D2");
                    }
                }
            }

            return "fuck";
        }
        private void InitialiseIntList()
        {
            IntList = _intcodeProgram.Split(',').Select(long.Parse).ToList();
        }

        public void InitNounVerb(int noun, int verb)
        {
            IntList[1] = noun;
            IntList[2] = verb;
        }

        public void ResetComputer()
        {
            Pointer = 0;
            State = IntCodeStates.Initialised;
        }

        private void DoubleMemory()
        {
            Console.WriteLine("Doubling memory");
            IntList.AddRange(new long[IntList.Count].ToList());
        }


        public void RunProgramUntilHalt()
        {
            while (State != IntCodeStates.Halted && Pointer < IntList.Count)
            {
                RunProgramUntilPause();
            }
        }
    }
}