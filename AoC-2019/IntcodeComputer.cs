using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CleanCode
{
    public class IntcodeComputer
    {
        private List<long> _intList;
        public IntCodeStates State { get; set;}
        private readonly string _intcodeProgram;
        public string OutputProgram => string.Join(",", _intList);
        public long Output => _intList[0];
        public IntcodeIoHandler IntcodeIoHandler { get; }
        private int _i;
        private long _relativeBase;
        private InstructionFactory _instructionFactory;

        public IntcodeComputer(string programCode)
        {
            _intcodeProgram = programCode;
            _intList = programCode.Split(',').Select(long.Parse).ToList();
            State = IntCodeStates.Initialised;
            _i = 0;
            _relativeBase = 0;
            _instructionFactory = new InstructionFactory();
            IntcodeIoHandler = new IntcodeIoHandler();
        }
        
        public IntcodeComputer(string programCode, IntcodeIoHandler intcodeIoHandler) :this (programCode)
        {
            IntcodeIoHandler = intcodeIoHandler;
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
                    RunProgram();
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
            _intList = _intcodeProgram.Split(',').Select(long.Parse).ToList();
        }

        public void InitNounVerb(int noun, int verb)
        {
            _intList[1] = noun;
            _intList[2] = verb;
        }

        private static long ProcessTwoNums(long one, long two, int opCode)
        {
            switch (opCode)
            {
                case 1:
                    return one + two;
                case 2:
                    return one * two;
                default:
                    throw new Exception();
            }
        }

        private void ProcessBoolean(bool test, int outputIndex)
        {
            _intList[outputIndex] = test ? 1 : 0;
        }

        private long GetValue(ReferenceValueResponse response)
        {
            switch (response.Type)
            {
                case RefValue.Value:
                    return response.Value;
                case RefValue.Reference:
                    return _intList[(int)response.Value];
                default:
                    throw new Exception();
            }
        }

        public void ResetComputer()
        {
            _i = 0;
            State = IntCodeStates.Initialised;
        }

        private void HandleInstruction(Instruction instruction)
        {
            switch (instruction.OpCode)
            {
                case 1:
                case 2:
                    _intList[(int)instruction.Parameters[2]] = ProcessTwoNums(
                        GetValue(instruction.RefOrValue(0, _relativeBase)),
                        GetValue(instruction.RefOrValue(1, _relativeBase)),
                        instruction.OpCode
                    );
                    break;
                case 3:
                    // Read input.
                    try
                    {
                        // Try and read next input from buffer. If none exist prompt user.
                        _intList[(int)instruction.Parameters[0]] = IntcodeIoHandler?.GetNextInput() ??
                                                              int.Parse(Console.ReadLine() ?? "0");
                    }
                    catch
                    {
                        State = IntCodeStates.Paused;
                    }
                    break;
                case 4:
                    // Write output
                    var value = GetValue(instruction.RefOrValue(0, _relativeBase));
                    IntcodeIoHandler?.AppendOutput((int)value);
                    break;
                case 5:
                    _i = GetValue(instruction.RefOrValue(0, _relativeBase)) != 0
                        ? (int)GetValue(instruction.RefOrValue(1, _relativeBase)) - instruction.Length
                        : _i;
                    break;
                case 6:
                    _i = GetValue(instruction.RefOrValue(0, _relativeBase)) == 0
                        ? (int)GetValue(instruction.RefOrValue(1, _relativeBase)) - instruction.Length
                        : _i;
                    break;
                case 7:
                    ProcessBoolean(
                        GetValue(instruction.RefOrValue(0, _relativeBase)) <
                        GetValue(instruction.RefOrValue(1, _relativeBase)),
                        (int)instruction.Parameters[2]
                    );
                    break;
                case 8:
                    ProcessBoolean(
                        GetValue(instruction.RefOrValue(0, _relativeBase)) ==
                        GetValue(instruction.RefOrValue(1, _relativeBase)),
                        (int)instruction.Parameters[2]
                    );
                    break;
                case 9:
                    _relativeBase += GetValue(instruction.RefOrValue(0, _relativeBase));
                    break;
                default:
                    throw new Exception("OpCode not implemented.");
            }
        }

        public void RunProgram()
        {
            State = IntCodeStates.Running;
            while (_i < _intList.Count)
            {
                // Pad the first value such that it is as long as it could ever be.
                var firstValue = _intList[_i].ToString("D5");
                var instruction = _instructionFactory.CreateInstruction(firstValue);
                
                if (instruction.OpCode == 99)
                {
                    State = IntCodeStates.Halted;
                    return;
                }
                
                instruction.Parameters = _intList.GetRange(_i + 1, instruction.Length - 1).Select(x => (long)x).ToList();
                
                try
                {
                    HandleInstruction(instruction);
                    _i += instruction.Length;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    _intList.AddRange(new long[_intList.Count].ToList());
                }
            }
        }
    }
}