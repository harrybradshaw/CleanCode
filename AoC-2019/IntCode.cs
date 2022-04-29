using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CleanCode
{
    public class IntcodeComputer
    {
        private List<int> _intList;
        public IntCodeStates State { get; set;}
        private readonly InstructionFactory _instructionFactory;
        private readonly string _intcodeProgram;
        public string OutputString => string.Join(",", _intList);
        public int Output => _intList[0];
        public IntcodeIoHandler IntcodeIoHandler { get; set; }
        private int _i;

        public IntcodeComputer(string programCode)
        {
            _intcodeProgram = programCode;
            _intList = programCode.Split(',').Select(int.Parse).ToList();
            _instructionFactory = new InstructionFactory();
            State = IntCodeStates.Initialised;
            _i = 0;
            IntcodeIoHandler = new IntcodeIoHandler();
        }
        
        public IntcodeComputer(string programCode, IntcodeIoHandler intcodeIoHandler)
        {
            _intcodeProgram = programCode;
            _intList = programCode.Split(',').Select(int.Parse).ToList();
            _instructionFactory = new InstructionFactory();
            State = IntCodeStates.Initialised;
            _i = 0;
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
                    ProcessIntCode();
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
            _intList = _intcodeProgram.Split(',').Select(int.Parse).ToList();
        }

        public void InitNounVerb(int noun, int verb)
        {
            _intList[1] = noun;
            _intList[2] = verb;
        }

        private static int ProcessTwoNums(int one, int two, int opCode)
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

        private int GetValue(ReferenceValueResponse response)
        {
            return response.Type == RefValue.Value 
                ? response.Value 
                : _intList[response.Value];
        }

        public void ResetComputer()
        {
            _i = 0;
            State = IntCodeStates.Initialised;
        }
        
        public void ProcessIntCode()
        {
            State = IntCodeStates.Running;
            while (_i < _intList.Count)
            {
                //Pad the first value such that it is as long as it could ever be.
                var firstValue = _intList[_i].ToString("D5");
                var instruction = InstructionFactory.CreateInstruction(firstValue);
                
                if (instruction.OpCode == 99)
                {
                    State = IntCodeStates.Halted;
                    return;
                }
                
                instruction.Parameters = _intList.GetRange(_i + 1, instruction.Length - 1);
                switch (instruction.OpCode)
                {
                    case 1:
                    case 2:
                        _intList[instruction.Parameters[2]] = ProcessTwoNums(
                            GetValue(instruction.RefOrValue(0)),
                            GetValue(instruction.RefOrValue(1)),
                            instruction.OpCode
                        );
                        break;
                    case 3:
                        try
                        {
                            _intList[instruction.Parameters[0]] = IntcodeIoHandler?.GetNextInput() ?? int.Parse(Console.ReadLine() ?? "0");
                        }
                        catch
                        {
                            State = IntCodeStates.Paused;
                            return;
                        }
                        
                        break;
                    case 4:
                        var v = GetValue(instruction.RefOrValue(0));
                        IntcodeIoHandler?.AppendOutput(v);
                        break;
                    case 5:
                        _i = GetValue(instruction.RefOrValue(0)) != 0 ? GetValue(instruction.RefOrValue(1)) - instruction.Length : _i;
                        break;
                    case 6:
                        _i = GetValue(instruction.RefOrValue(0)) == 0 ? GetValue(instruction.RefOrValue(1)) - instruction.Length : _i;
                        break;
                    case 7: 
                        ProcessBoolean(
                            GetValue(instruction.RefOrValue(0)) <
                            GetValue(instruction.RefOrValue(1)),
                            instruction.Parameters[2]
                        );
                        break;
                    case 8:
                        ProcessBoolean(
                            GetValue(instruction.RefOrValue(0)) ==
                            GetValue(instruction.RefOrValue(1)),
                            instruction.Parameters[2]
                        );
                        break;
                    default:
                        throw new Exception();
                }

                _i += instruction.Length;
            }

            State = IntCodeStates.Halted;
        }
    }
}