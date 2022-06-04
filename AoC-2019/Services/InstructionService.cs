using System;
using System.Diagnostics;
using System.Linq;

namespace CleanCode
{
    public class InstructionService
    {
        private readonly ReferenceValueService _referenceValueService;
        public InstructionService(ReferenceValueService referenceValueService)
        {
            _referenceValueService = referenceValueService;
        }
        public InstructionResponse HandleInstruction(IntcodeComputer computer, Instruction instruction)
        {
            computer.State = IntCodeStates.Running;
            instruction.LogInstruction();
            InstructionResponse response;
            switch (instruction.OpCode)
            {
                case 1:
                case 2:
                    return HandleAddOrMultiply(computer, instruction);
                case 3:
                    return HandleInputFromBuffer(computer, instruction);
                case 4:
                    return HandleOutputToBuffer(computer, instruction);
                case 5:
                    response = HandleJumpIfTrue(computer, instruction);
                    computer.Pointer = (int) response.Value;
                    return response;
                case 6:
                    response = HandleJumpIfFalse(computer, instruction);
                    computer.Pointer = (int) response.Value;
                    return response;
                case 7:
                    return HandleLessThan(computer, instruction);
                case 8:
                    return HandleEquals(computer, instruction);
                case 9:
                    return HandleModifyRelativeBase(computer, instruction);
                default:
                    throw new Exception("OpCode not implemented.");
            }
        }

        private InstructionResponse HandleAddOrMultiply(IntcodeComputer computer, Instruction instruction)
        {
            var outputAddress = _referenceValueService.GetAddress(computer, instruction, 2);
            if (!computer.CanAccessMemoryAddress(outputAddress))
            {
                return new InstructionResponse
                {
                    WasInstructionSuccess = false,
                    FailureReason = FailureReason.CouldNotAccessMemoryAddress,
                };
            }

            var value1 = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var value2 = _referenceValueService.GetFromInstruction(computer, instruction, 1);
            
            var result = ProcessTwoNums(
                 value1,
                 value2,
                instruction.OpCode
            );
            computer.IntList[outputAddress] = result;
            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleInputFromBuffer(IntcodeComputer computer, Instruction instruction)
        {
            var outputAddress = _referenceValueService.GetAddress(computer, instruction, 0);
            if (!computer.CanAccessMemoryAddress(outputAddress))
            {
                return new InstructionResponse
                {
                    WasInstructionSuccess = false,
                    FailureReason = FailureReason.CouldNotAccessMemoryAddress,
                };
            }
            
            // Try and read next input from buffer. If none exist prompt user.
            try
            {
                var result =  computer.IntcodeIoHandler?.GetNextInput() ?? GetInputFromUser();
                computer.IntList[outputAddress] = result;

            }
            catch
            {
                computer.State = IntCodeStates.Paused;
                return new InstructionResponse
                {
                    WasInstructionSuccess = false,
                };
            }
            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleOutputToBuffer(IntcodeComputer computer, Instruction instruction)
        {
            var value = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            computer.IntcodeIoHandler?.AppendOutput(value);
            computer.State = IntCodeStates.Paused;
            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleJumpIfTrue(IntcodeComputer computer, Instruction instruction)
        {
            var testValue = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var value = testValue != 0
                ? (int)_referenceValueService.GetFromInstruction(computer, instruction, 1) - instruction.Length
                : computer.Pointer;
            
            return new InstructionResponse
            {
                Value = value,
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleJumpIfFalse(IntcodeComputer computer, Instruction instruction)
        {
            var value = _referenceValueService.GetFromInstruction(computer, instruction, 0) == 0
                ? (int)_referenceValueService.GetFromInstruction(computer, instruction, 1) - instruction.Length
                : computer.Pointer;
            
            return new InstructionResponse
            {
                Value = value,
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleLessThan(IntcodeComputer computer, Instruction instruction)
        {
            var outputAddress = _referenceValueService.GetAddress(computer, instruction, 2);
            var value1 =
                _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var value2 = 
                _referenceValueService.GetFromInstruction(computer, instruction, 1);
           Console.WriteLine($"LessThan: {value1} < {value2}");
            
            ProcessBoolean(
                computer,
                value1 < value2,
                outputAddress
            );
            
            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleEquals(IntcodeComputer computer, Instruction instruction)
        {
            var value1 = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var value2 = _referenceValueService.GetFromInstruction(computer, instruction, 1);
            var outputAddress = _referenceValueService.GetAddress(computer, instruction, 2);
            
            ProcessBoolean(
                computer,
                value1 == value2,
                outputAddress
            );

            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleModifyRelativeBase(IntcodeComputer computer, Instruction instruction)
        {
            var value = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            Console.WriteLine($"AdjustingRelativeBase: {computer.RelativeBase} -> {computer.RelativeBase + value}");
            computer.RelativeBase += value;
            return new InstructionResponse
            {
                Value = value,
                WasInstructionSuccess = true,
            };
        }

        private static long ProcessTwoNums(long one, long two, int opCode)
        {
            switch (opCode)
            {
                case 1:
                    Console.WriteLine($"Addition: {one} + {two}");
                    return one + two;
                case 2:
                    Console.WriteLine($"Multiplication: {one} * {two}");
                    return one * two;
                default:
                    throw new Exception("Unknown OpCode");
            }
        }
        
        private void ProcessBoolean(IntcodeComputer computer, bool test, int outputIndex)
        {
            computer.IntList[outputIndex] = test ? 1 : 0;
        }
        
        private int GetInputFromUser()
        {
            Console.WriteLine("Awaiting input...");
            return int.Parse(Console.ReadLine() ?? "0");
        }
    }
}