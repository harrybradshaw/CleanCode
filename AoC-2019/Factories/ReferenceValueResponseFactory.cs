using System;

namespace CleanCode
{
    public class ReferenceValueResponseFactory
    {
        public ReferenceValueResponse Create(Instruction instruction, int index, long relativeBase)
        {
            if (index >= instruction.ParameterModes.Count || index >= instruction.Parameters.Count)
            {
                throw new Exception("Index out of range");
            }

            var response = new ReferenceValueResponse
            {
                Value = instruction.Parameters[index]
            };


            switch (instruction.ParameterModes[index])
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
}