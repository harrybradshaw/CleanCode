using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CleanCode
{
    public class IntCode
    {
        private List<int> _intList;
        public IntcodeStates.IntCodeStates State { get; set;}
        private InstructionFactory _instructionFactory;
        private readonly string _inputString;
        public string OutputString => string.Join(",", _intList);
        public int Output => _intList[0];
        public Io Io { get; set; }
        private int _i;

        public IntCode(string stringCode)
        {
            _inputString = stringCode;
            _intList = stringCode.Split(',').Select(int.Parse).ToList();
            _instructionFactory = new InstructionFactory();
            State = IntcodeStates.IntCodeStates.Initialised;
            _i = 0;
            Io = new Io();
        }
        
        public IntCode(string stringCode, Io io)
        {
            _inputString = stringCode;
            _intList = stringCode.Split(',').Select(int.Parse).ToList();
            _instructionFactory = new InstructionFactory();
            State = IntcodeStates.IntCodeStates.Initialised;
            _i = 0;
            Io = io;
        }
        
        public string CalcNounVerb()
        {
            for (var a = 0; a < 100; a++)
            {
                for (var j = 0; j < 100; j++)
                {
                    InitIntList();
                    InitNounVerb(a,j);
                    Reset();
                    ProcessIntCode();
                    if (Output == 19690720)
                    {
                        return a.ToString("D2") + j.ToString("D2");
                    }
                }
            }

            return "fuck";
        }

        private void InitIntList()
        {
            _intList = _inputString.Split(',').Select(int.Parse).ToList();
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
            return response.Type == RefValue.Value ? response.Value : _intList[response.Value];
        }

        public void Reset()
        {
            _i = 0;
        }
        
        public void ProcessIntCode()
        {
            State = IntcodeStates.IntCodeStates.Running;
            while (_i < _intList.Count)
            {
                var firstTerm = _intList[_i].ToString("D5");
                var inst = _instructionFactory.CreateInstruction(firstTerm);
                
                if (inst.OpCode == 99)
                {
                    State = IntcodeStates.IntCodeStates.Halted;
                    return;
                }
                
                inst.Parameters = _intList.GetRange(_i + 1, inst.Length - 1);
                switch (inst.OpCode)
                {
                    case 1:
                    case 2:
                        _intList[inst.Parameters[2]] = ProcessTwoNums(
                            GetValue(inst.RefOrValue(0)),
                            GetValue(inst.RefOrValue(1)),
                            inst.OpCode
                        );
                        break;
                    case 3:
                        try
                        {
                            _intList[inst.Parameters[0]] = Io?.GetNextInput() ?? int.Parse(Console.ReadLine() ?? "0");
                        }
                        catch
                        {
                            State = IntcodeStates.IntCodeStates.Paused;
                            return;
                        }
                        
                        break;
                    case 4:
                        var v = GetValue(inst.RefOrValue(0));
                        Io?.AppendOutput(v);
                        //Console.WriteLine(v);
                        break;
                    case 5:
                        _i = GetValue(inst.RefOrValue(0)) != 0 ? GetValue(inst.RefOrValue(1)) - inst.Length : _i;
                        break;
                    case 6:
                        _i = GetValue(inst.RefOrValue(0)) == 0 ? GetValue(inst.RefOrValue(1)) - inst.Length : _i;
                        break;
                    case 7: 
                        ProcessBoolean(
                            GetValue(inst.RefOrValue(0)) <
                            GetValue(inst.RefOrValue(1)),
                            inst.Parameters[2]
                        );
                        break;
                    case 8:
                        ProcessBoolean(
                            GetValue(inst.RefOrValue(0)) ==
                            GetValue(inst.RefOrValue(1)),
                            inst.Parameters[2]
                        );
                        break;
                    default:
                        throw new Exception();
                }

                _i += inst.Length;
            }

            State = IntcodeStates.IntCodeStates.Halted;
        }
    }
}