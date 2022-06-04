using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class MaximalThrusterSignal
    {
        private readonly IEnumerable<IEnumerable<int>> _phasePermutations;
        private readonly string _programString;
        private readonly MultiAmpMode _mode;

        public MaximalThrusterSignal(string programString, MultiAmpMode mode = MultiAmpMode.Single)
        {
            _programString = programString;
            _mode = mode;
            // Single mode => PhaseSettings are integers [0,1,2,3,4] used once.
            // Multiple mode => PhaseSettings are [5,6,7,8,9] used once.
            _phasePermutations = mode == MultiAmpMode.Single 
                ? GetPermutations(Enumerable.Range(0, 5), 5) 
                : GetPermutations(Enumerable.Range(5, 5), 5);
        }
        
        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public long CalculateMaximalSignal()
        {
            var outputs = CalculateOutputs();
            return outputs.Select(output => output.ThrusterSignal).Max();
        }

        private List<ThrusterOutput> CalculateOutputs()
        {
            return _phasePermutations.Select(phaseSettings =>
            {
                var phaseSettingsList = phaseSettings.ToList();
                var thrusterOutput = new ThrusterOutput
                {
                    PhaseSettings = phaseSettingsList,
                    ThrusterSignal = new MultiAmpRunner(phaseSettingsList, _programString, _mode).GetThrusterSignal(),
                };
                Console.WriteLine($"{string.Join(",", thrusterOutput.PhaseSettings)} - {thrusterOutput.ThrusterSignal}");
                return thrusterOutput;
            }).ToList();
        }
    }

    public class ThrusterOutput
    {
        public List<int> PhaseSettings { get; set; }
        public long ThrusterSignal { get; set; }
    }
}