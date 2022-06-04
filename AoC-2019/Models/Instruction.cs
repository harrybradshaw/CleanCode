using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class Instruction
    {
        public int OpCode { get; set; }
        public int Length { get; set; }
        public List<ParameterMode> ParameterModes { get; set; }
        public List<long> Parameters { get; set; }
    }
}