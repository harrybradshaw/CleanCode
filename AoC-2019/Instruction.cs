using System;
using System.Collections.Generic;

namespace CleanCode
{
    public class Instruction
    {
        public int OpCode { get; set; }
        public int Length { get; set; }
        public List<int> ParameterModes { get; set; }
        public List<int> Parameters { get; set; }

        public ReferenceValueResponse RefOrValue(int index)
        {
            if (index >= ParameterModes.Count)
            {
                throw new Exception("Out of range!");
            }

            switch (ParameterModes[index])
            {
                case 0:
                    return new ReferenceValueResponse()
                    {
                        Type = RefValue.Reference,
                        Value = Parameters[index],
                    };

                case 1:
                    return new ReferenceValueResponse()
                    {
                        Type = RefValue.Value,
                        Value = Parameters[index],
                    };
                default:
                    throw new Exception();
            }
        }
    }

    public enum RefValue
    {
        Reference,
        Value,
    }

    public class ReferenceValueResponse
    {
        public RefValue Type
        {
            get;
            set;
        }
        public int Value { get; set; }
    }
}