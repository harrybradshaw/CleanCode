using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode
{
    public class MaximalThrusterSignal
    {
        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public int CalcMax()
        {
            var outputs = CalcOutputs();
            return outputs.Select(output => output.ThrusterSignal).Max();
        }

        private List<ThrusterOutput> CalcOutputs()
        {
            return perms.Select(perm =>
            {
                var to = new ThrusterOutput
                {
                    PhaseSettings = perm.ToList(),
                    ThrusterSignal = new MultiAmpRunner(perm.ToList(), inputString, _mode).GetThrusterSignal(),
                };
                Console.WriteLine($"{string.Join(",", to.PhaseSettings)} - {to.ThrusterSignal}");
                return to;
            }).ToList();
        }

        private IEnumerable<IEnumerable<int>> perms;
        private string inputString;
        private MultiAmpMode _mode;

        public MaximalThrusterSignal(string inputString, MultiAmpMode mode = MultiAmpMode.Single)
        {
            this.inputString = inputString;
            _mode = mode;
            perms = mode == MultiAmpMode.Single ? GetPermutations(Enumerable.Range(0, 5), 5) : GetPermutations(Enumerable.Range(5, 5), 5);
        }
    }

    public class ThrusterOutput
    {
        public List<int> PhaseSettings { get; set; }
        public int ThrusterSignal { get; set; }
    }
}