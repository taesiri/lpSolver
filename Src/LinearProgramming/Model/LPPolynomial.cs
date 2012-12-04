using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearProgramming.Model
{
    public class LPPolynomial
    {
        private readonly List<Tuple<string, int>> _poly;

        public LPPolynomial()
        {
            _poly = new List<Tuple<string, int>>();
            Constant = 0;
        }

        public LPPolynomial(LPPolynomial poly)
        {
            _poly = new List<Tuple<string, int>>(poly.GetPoly);
            Constant = poly.Constant;
        }

        public int Constant { get; set; }


        public List<String> GetVariables
        {
            get { return _poly.Select(tuple => tuple.Item1).ToList(); }
        }

        public List<Tuple<string, int>> GetPoly
        {
            get { return _poly; }
        }

        public void AddConstant(int numb)
        {
            Constant += numb;
        }

        public void AddNewVariable(string var, int coefficient)
        {
            if (!GetVariables.Contains(var))
                _poly.Add(new Tuple<string, int>(var, coefficient));
            else
            {
                // the Variable Already Exist! Find it and ....
            }
        }
    }
}