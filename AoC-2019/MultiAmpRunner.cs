using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class MultiAmpRunner
    {
        private List<int> _phaseSettings;
        private string _inputString;
        private List<IntCode> _amps;
        private MultiAmpMode _mode;

        public int GetThrusterSignal()
        {
            return  _mode == MultiAmpMode.Single 
                ? SingleAmplifierPass(new List<int>{0}).Last() 
                : AmpFeedback();
        } 

        public MultiAmpRunner(List<int> phaseSettings, string inputString, MultiAmpMode mode)
        {
            _phaseSettings = phaseSettings;
            _inputString = inputString;
            _amps = new List<IntCode>();
            _mode = mode;
            InitAmps();
        }

        private void InitAmps()
        {
            for (var i = 0; i < 5; i++)
            {
                _amps.Add(new IntCode(_inputString));
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
                inputList.AddRange(i == 0 ? firstInput : _amps[i - 1].Io.OutputList);
                _amps[i].Io.InputList = inputList;
                _amps[i].ProcessIntCode();
            }
            return _amps[4].Io.OutputList;
        }

        private int AmpFeedback()
        {
            var first = SingleAmplifierPass(new List<int>());
            while (_amps.Select(amp => amp.State).All(state => state != IntcodeStates.IntCodeStates.Halted))
            {
                first = SingleAmplifierPass(first);
            }

            return _amps.Last().Io.LastOutput;

        }
    }

    public enum MultiAmpMode
    {
        Single,
        Feedback,
    }
}