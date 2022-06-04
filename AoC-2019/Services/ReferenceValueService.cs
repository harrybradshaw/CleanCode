using System.Collections.Generic;

namespace CleanCode
{
    public class ReferenceValueService
    {
        private readonly ReferenceValueResponseFactory _referenceValueResponseFactory = new ReferenceValueResponseFactory();
        public long GetValue(ReferenceValueResponse response, List<long> intList)
        {
            switch (response.Type)
            {
                default:
                case RefValue.Value:
                    return response.Value;
                case RefValue.Reference:
                    return intList[(int)response.Value];
            }
        }

        public long GetFromInstruction(IntcodeComputer computer, Instruction instruction, int index)
        {
            var response = _referenceValueResponseFactory.Create(instruction, index, computer.RelativeBase);
            return GetValue(response, computer.IntList);
        }

        public int GetAddress(IntcodeComputer computer, Instruction instruction, int index)
        {
            var response = _referenceValueResponseFactory.Create(instruction, index, computer.RelativeBase);
            return (int) response.Value;
        }
    }
}