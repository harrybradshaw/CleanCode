using System;
using System.Collections.Generic;

namespace CleanCode
{
    public class Instruction
    {
        public int OpCode { get; set; }
        public int Length { get; set; }
        public List<ParameterMode> ParameterModes { get; set; }
        public List<long> Parameters { get; set; }

        public ReferenceValueResponse RefOrValue(int index, long relativeBase)
        {
            if (index >= ParameterModes.Count || index >= Parameters.Count)
            {
                throw new Exception("Index out of range");
            }

            var response = new ReferenceValueResponse
            {
                Value = Parameters[index]
            };


            switch (ParameterModes[index])
            {
                case ParameterMode.Position:
                    response.Type = RefValue.Reference;
                    break;

                case ParameterMode.Intermediate:
                    response.Type = RefValue.Value;
                    break;
                
                case ParameterMode.RelativeMode:
                    response.Type = RefValue.Reference;
                    response.Value += relativeBase;
                    break;
                
                default:
                    throw new Exception("Unknown ParameterMode encountered.");
            }

            return response;
        }
    }

    public class ReferenceValueResponse
    {
        public RefValue Type { get; set; }
        public long Value { get; set; }
    }
}