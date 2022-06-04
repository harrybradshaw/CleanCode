using System.Collections.Generic;

namespace CleanCode
{
    public class ReferenceValueService
    {
        private readonly ReferenceValueResponseFactory _referenceValueResponseFactory = new ReferenceValueResponseFactory();
        public InstructionResponse GetValue(ReferenceValueResponse response, List<long> intList)
        {
            switch (response.Type)
            {
                default:
                case RefValue.Value:
                    return new InstructionResponse
                    {
                        Value = response.Value,
                        WasInstructionSuccess = true,
                    };
                case RefValue.Reference:
                    if ((int) response.Value < intList.Count)
                    {
                        return new InstructionResponse
                        {
                            Value = intList[(int) response.Value],
                            WasInstructionSuccess = true,
                        };
                    }
                    return new InstructionResponse
                    {
                        WasInstructionSuccess = false,
                        FailureReason = FailureReason.CouldNotAccessMemoryAddress,
                    };
            }
        }

        public InstructionResponse GetFromInstruction(IntcodeComputer computer, Instruction instruction, int index)
        {
            var response = _referenceValueResponseFactory.Create(instruction, index, computer.RelativeBase);
            return  GetValue(response, computer.IntList);
        }

        public int GetAddress(IntcodeComputer computer, Instruction instruction, int index)
        {
            var response = _referenceValueResponseFactory.Create(instruction, index, computer.RelativeBase);
            return (int) response.Value;
        }
    }
}