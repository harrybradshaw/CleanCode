using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class MultiAmpRunner
    {
        private readonly List<int> _phaseSettings;
        private readonly string _inputString;
        private readonly MultiAmpMode _mode;
        private List<IntcodeComputer> _amps;

        public MultiAmpRunner(List<int> phaseSettings, string inputString, MultiAmpMode mode)
        {
            _phaseSettings = phaseSettings;
            _inputString = inputString;
            _mode = mode;
            CreateAmplifiers();
        }
        
        public long GetThrusterSignal()
        {
            return  _mode == MultiAmpMode.Single 
                ? SingleAmplifierPass(new List<long>{0}).Last() 
                : AmplifierFeedbackLoop();
        } 

        private void CreateAmplifiers()
        {
            _amps = new List<IntcodeComputer>();
            for (var i = 0; i < 5; i++)
            {
                _amps.Add(new IntcodeComputer(_inputString));
            }
        }

        private List<long> SingleAmplifierPass(List<long> firstInput)
        {
            for (var i = 0; i < 5; i++)
            {
                var inputList = new List<long> {_phaseSettings[i]};
                if (i == 0)
                {
                    inputList.Add(0);
                }
                inputList.AddRange(i == 0 
                    ? firstInput 
                    : _amps[i - 1].IntcodeIoHandler.OutputList
                );
                _amps[i].IntcodeIoHandler.InputList = inputList;
                _amps[i].RunProgramUntilPause();
            }
            return _amps[4].IntcodeIoHandler.OutputList;
        }

        private long AmplifierFeedbackLoop()
        {
            var firstInput = SingleAmplifierPass(new List<long>());
            while (_amps.Select(amp => amp.State).All(state => state != IntCodeStates.Halted))
            {
                firstInput = SingleAmplifierPass(firstInput);
            }
            Console.WriteLine(_amps.Last().IntcodeIoHandler.LastOutput);
            return _amps.Last().IntcodeIoHandler.LastOutput;
        }
    }
}