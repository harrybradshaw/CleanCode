using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            //instruction.LogInstruction();
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
                    return HandleJumpIfTrue(computer, instruction);
                case 6:
                    return HandleJumpIfFalse(computer, instruction);
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
            
            var response1 = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var response2 = _referenceValueService.GetFromInstruction(computer, instruction, 1);

            if (!AssertCanAccessInputs(new List<InstructionResponse> {response1, response2}))
            {
                return new InstructionResponse
                {
                    WasInstructionSuccess = false,
                    FailureReason = FailureReason.CouldNotAccessMemoryAddress,
                };
            }
            
            var result = ProcessTwoNums(
                 response1.Value,
                 response2.Value,
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
                var result = computer.IntcodeIoHandler?.GetNextInput();
                if (!result.HasValue)
                {
                    if (computer.PauseOnInput)
                    {
                        computer.State = IntCodeStates.Paused;
                    }
                    else if (computer.AwaitInput)
                    {
                        return new InstructionResponse
                        {
                            WasInstructionSuccess = false,
                            FailureReason = FailureReason.AwaitingInputThatIsNotPresent,
                        };
                    }
                    else
                    {
                        computer.IntList[outputAddress] = GetInputFromUser();
                    }
                }
                else
                {
                    computer.IntList[outputAddress] = result.Value;
                }

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
            var response1 = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            if (!response1.WasInstructionSuccess)
            {
                return response1;
            }
            computer.IntcodeIoHandler?.OutputList.Add(response1.Value);
            if (computer.PauseOnOutput)
            {
                computer.State = IntCodeStates.Paused;
            }
            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleJumpIfTrue(IntcodeComputer computer, Instruction instruction)
        {
            var response1 = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var response2 = _referenceValueService.GetFromInstruction(computer, instruction, 1);
            if (!AssertCanAccessInputs(new List<InstructionResponse> {response1, response2}))
            {
                return new InstructionResponse
                {
                    WasInstructionSuccess = false,
                    FailureReason = FailureReason.CouldNotAccessMemoryAddress,
                };
            }
            var value = response1.Value != 0
                ? (int)response2.Value - instruction.Length
                : computer.Pointer;
            computer.Pointer = value;

            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleJumpIfFalse(IntcodeComputer computer, Instruction instruction)
        {
            var response1 = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var response2 = _referenceValueService.GetFromInstruction(computer, instruction, 1);
            if (!AssertCanAccessInputs(new List<InstructionResponse> {response1, response2}))
            {
                return new InstructionResponse
                {
                    WasInstructionSuccess = false,
                    FailureReason = FailureReason.CouldNotAccessMemoryAddress,
                };
            }
            var value = response1.Value == 0
                ? (int) response2.Value - instruction.Length
                : computer.Pointer;
            computer.Pointer = value;
            
            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleLessThan(IntcodeComputer computer, Instruction instruction)
        {
            var outputAddress = _referenceValueService.GetAddress(computer, instruction, 2);
            var response1 = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var response2 = _referenceValueService.GetFromInstruction(computer, instruction, 1);
            if (!AssertCanAccessInputs(new List<InstructionResponse> {response1, response2}))
            {
                return new InstructionResponse
                {
                    WasInstructionSuccess = false,
                    FailureReason = FailureReason.CouldNotAccessMemoryAddress,
                };
            }
            //Console.WriteLine($"LessThan: {value1} < {value2}");
            
            ProcessBoolean(
                computer,
                response1.Value < response2.Value,
                outputAddress
            );
            
            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleEquals(IntcodeComputer computer, Instruction instruction)
        {
            var response1 = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            var response2 = _referenceValueService.GetFromInstruction(computer, instruction, 1);
            if (!AssertCanAccessInputs(new List<InstructionResponse> {response1, response2}))
            {
                return new InstructionResponse
                {
                    WasInstructionSuccess = false,
                    FailureReason = FailureReason.CouldNotAccessMemoryAddress,
                };
            }
            var outputAddress = _referenceValueService.GetAddress(computer, instruction, 2);
            
            ProcessBoolean(
                computer,
                response1.Value == response2.Value,
                outputAddress
            );

            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private InstructionResponse HandleModifyRelativeBase(IntcodeComputer computer, Instruction instruction)
        {
            var response = _referenceValueService.GetFromInstruction(computer, instruction, 0);
            if (!response.WasInstructionSuccess)
            {
                return response;
            }
            //Console.WriteLine($"AdjustingRelativeBase: {computer.RelativeBase} -> {computer.RelativeBase + value}");
            computer.RelativeBase += response.Value;
            return new InstructionResponse
            {
                WasInstructionSuccess = true,
            };
        }

        private static long ProcessTwoNums(long one, long two, int opCode)
        {
            switch (opCode)
            {
                case 1:
                    //Console.WriteLine($"Addition: {one} + {two}");
                    return one + two;
                case 2:
                    //Console.WriteLine($"Multiplication: {one} * {two}");
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

        private bool AssertCanAccessInputs(List<InstructionResponse> inputResponses)
        {
            return inputResponses.All(i => i.WasInstructionSuccess);
        }
    }
}