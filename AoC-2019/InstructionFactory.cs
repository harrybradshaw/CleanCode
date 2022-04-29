using System;
using System.Linq;

namespace CleanCode
{
    public class InstructionFactory
    { 
        public static Instruction CreateInstruction(string firstTerm)
        {
            // OpCode is defined as last two digits of this.
            var opCode = int.Parse(firstTerm.Substring(3));
            var paramModes = firstTerm.Substring(0, 3).Select(c => (ParameterMode)int.Parse(c.ToString())).ToList();
            // Parameter modes are given in opposite order to instructions.
            paramModes.Reverse();
            
            return new Instruction()
            {
                OpCode = opCode,
                Length = InstructionLengthForOpCode(opCode),
                ParameterModes = paramModes,
            };
        }

        private static int InstructionLengthForOpCode(int opCode)
        {
            switch (opCode)
            {
                case 1:
                case 2:
                case 7:
                case 8:
                    return 4;
                case 5:
                case 6:
                    return 3;
                case 3:
                case 4:
                    return 2;
                case 99:
                    return 1;
                default:
                    throw new Exception();
            }
        }
    }
}