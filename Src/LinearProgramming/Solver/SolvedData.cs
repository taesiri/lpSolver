using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearProgramming.Solver
{
    public class SolvedData
    {
        private readonly List<Tuple<string, long>> _solvedData;

        public SolvedData()
        {
            _solvedData = new List<Tuple<string, long>>();
        }

        public SolvedData(List<Tuple<string, long>> data)
        {
            _solvedData = data;
        }

        public void PushElement(string name, long value)
        {
            _solvedData.Add(new Tuple<string, long>(name, value));
        }

        public override string ToString()
        {
            return _solvedData.Aggregate("",
                                         (current, tuple) => current + (String.Format("Variable {0}; Value {1}",
                                                                                      tuple.Item1, tuple.Item2) +
                                                                        Environment.NewLine));
        }
    }
}

