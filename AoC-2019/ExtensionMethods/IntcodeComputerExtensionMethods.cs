using System;

namespace CleanCode
{
    public static class IntcodeComputerExtensionMethods
    {
        public static bool CanAccessMemoryAddress(this IntcodeComputer intcodeComputer, int address)
        {
            return address < intcodeComputer.IntList.Count;
        }
        
        public static void LogComputerState(this IntcodeComputer intcodeComputer)
        {
            Console.WriteLine("\r\n");
            Console.WriteLine($"RelativeBase: {intcodeComputer.RelativeBase}");
            Console.WriteLine($"CurrentPointer: {intcodeComputer.Pointer}");
        }
    }
}