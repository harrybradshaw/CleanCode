using System;

namespace CleanCode
{
    public static class InstructionExtensionMethods
    {
        public static void LogInstruction(this Instruction instruction)
        {
            Console.WriteLine($"OpCode: {instruction.OpCode}");
            Console.WriteLine($"Parameters: {string.Join(", ", instruction.Parameters)}");
            Console.WriteLine($"ParameterMode: {string.Join(", ", instruction.ParameterModes)}");
        }
    }
}