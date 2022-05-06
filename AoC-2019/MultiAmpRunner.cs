using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class MultiAmpRunner
    {
        private List<int> _phaseSettings;
        private string _inputString;
        private List<IntcodeComputer> _amps;
        private MultiAmpMode _mode;

        public int GetThrusterSignal()
        {
            return  _mode == MultiAmpMode.Single 
                ? SingleAmplifierPass(new List<int>{0}).Last() 
                : AmplifierFeedbackLoop();
        } 

        public MultiAmpRunner(List<int> phaseSettings, string inputString, MultiAmpMode mode)
        {
            _phaseSettings = phaseSettings;
            _inputString = inputString;
            _amps = new List<IntcodeComputer>();
            _mode = mode;
            InitAmps();
        }

        private void InitAmps()
        {
            for (var i = 0; i < 5; i++)
            {
                _amps.Add(new IntcodeComputer(_inputString));
            }
        }

        private List<int> SingleAmplifierPass(List<int> firstInput)
        {
            for (var i = 0; i < 5; i++)
            {
                var inputList = new List<int> {_phaseSettings[i]};
                if (i == 0)
                {
                    inputList.Add(0);
                }
                inputList.AddRange(i == 0 
                    ? firstInput 
                    : _amps[i - 1].IntcodeIoHandler.OutputList
                );
                _amps[i].IntcodeIoHandler.InputList = inputList;
                _amps[i].RunProgram();
            }
            return _amps[4].IntcodeIoHandler.OutputList;
        }

        private int AmplifierFeedbackLoop()
        {
            var first = SingleAmplifierPass(new List<int>());
            while (_amps.Select(amp => amp.State).All(state => state != IntCodeStates.Halted))
            {
                first = SingleAmplifierPass(first);
            }

            return _amps.Last().IntcodeIoHandler.LastOutput;

        }
    }
}