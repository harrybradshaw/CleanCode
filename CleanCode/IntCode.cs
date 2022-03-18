using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class IntCode
    {
        private List<int> _intList;
        private readonly string _inputString;
        public string OutputString => string.Join(",", _intList);
        public int Output => _intList[0];

        public IntCode(string stringCode)
        {
            _inputString = stringCode;
            _intList = stringCode.Split(',').Select(int.Parse).ToList();
        }

        public string CalcNounVerb()
        {
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 100; j++)
                {
                    InitIntList();
                    InitNounVerb(i,j);
                    ProcessIntCode();
                    if (Output == 19690720)
                    {
                        return i.ToString("D2") + j.ToString("D2");
                    }
                }
            }

            return "fuck";
        }

        private void InitIntList()
        {
            _intList = _inputString.Split(',').Select(int.Parse).ToList();
        }

        public void InitNounVerb(int noun, int verb)
        {
            _intList[1] = noun;
            _intList[2] = verb;
        }

        public void ProcessIntCode()
        {
            for (var i = 0; i < _intList.Count; i+=4)
            {
                if (_intList[i] == 99)
                {
                    return;
                }
                // This might fail...
                var firstIndex = _intList[i + 1];
                var secondIndex = _intList[i + 2];
                var outputIndex = _intList[i + 3];
                switch (_intList[i])
                {
                    case 1:
                        _intList[outputIndex] = _intList[firstIndex] + _intList[secondIndex];
                        break;
                    case 2:
                        _intList[outputIndex] = _intList[firstIndex] * _intList[secondIndex];
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
    }
}