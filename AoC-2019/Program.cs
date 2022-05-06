using System;

namespace CleanCode
{
    class Program
    {
        static void Main(string[] args)
        {
            const string input = "1102,34915192,34915192,7,4,7,99,0";
            var computer = new IntcodeComputer(input);
            computer.RunProgram();
            Console.WriteLine(computer.IntcodeIoHandler.OutputAsString);
        }
    }
}