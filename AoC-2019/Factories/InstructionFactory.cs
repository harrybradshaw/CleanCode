using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class InstructionFactory
    { 
        public Instruction CreateInstruction(List<long> intList, int currentPointer)
        {
            var firstTerm = intList[currentPointer].ToString("D5");
            // OpCode is defined as last two digits of the first term.
            var opCode = int.Parse(firstTerm.Substring(3));
            var paramModes = firstTerm.Substring(0, 3).Select(c => (ParameterMode)int.Parse(c.ToString())).ToList();
            // Parameter modes are given in opposite order to instructions.
            paramModes.Reverse();
            var length = InstructionLengthForOpCode(opCode);
            
            return new Instruction
            {
                OpCode = opCode,
                Length = length,
                ParameterModes = paramModes.GetRange(0,length - 1),
                Parameters = intList.GetRange(currentPointer + 1, length - 1).ToList(),
            };
        }

        private static int InstructionLengthForOpCode(int opCode)
        {
            // Length of instruction includes the OpCode.
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
                case 9:
                    return 2;
                case 99:
                    return 1;
                default:
                    throw new Exception("OpCode not implemented.");
            }
        }
    }
}