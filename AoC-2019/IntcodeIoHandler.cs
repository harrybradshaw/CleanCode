using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class IntcodeIoHandler
    {
        public List<long> InputList { get; set; }
        public List<long> OutputList { get; }
        private int _nextInput;
        public long LastOutput => OutputList.Last();
        public string OutputAsString => string.Join(",", OutputList);

        public IntcodeIoHandler()
        {
            InputList = new List<long>();
            OutputList = new List<long>();
        }

        public IntcodeIoHandler(IEnumerable<long> inputList)
        {
            InputList = inputList.ToList();
            OutputList = new List<long>();
        }

        public long? GetNextInput()
        {
            if (_nextInput > InputList.Count - 1)
            {
                return null;
            }
            var input = InputList[_nextInput];
            _nextInput++;
            return input;
        }

        public void AppendOutput(long output)
        {
            OutputList.Add(output);
        }
    }
}