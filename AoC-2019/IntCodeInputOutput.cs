using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class Io
    {
        public List<int> InputList { get; set; }
        public List<int> OutputList { get; set; }
        private int _nextInput = 0;
        public int LastOutput => OutputList.Last();

        public Io()
        {
            InputList = new List<int>();
            OutputList = new List<int>();
        }

        public Io(IEnumerable<int> inputList)
        {
            InputList = inputList.ToList();
            OutputList = new List<int>();
        }

        public int GetNextInput()
        {
            var input =  InputList[_nextInput];
            _nextInput++;
            return input;
        }

        public void AppendOutput(int output)
        {
            OutputList.Add(output);
        }
    }
}