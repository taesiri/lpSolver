﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LinearProgramming.Model
{
    public class LPPolynomial
    {
        private readonly List<Tuple<string, double>> _poly;

        public LPPolynomial()
        {
            _poly = new List<Tuple<string, double>>();
            Constant = 0;
        }

        public LPPolynomial(LPPolynomial poly)
        {
            _poly = new List<Tuple<string, double>>(poly.GetPoly);
            Constant = poly.Constant;
        }

        public double Constant { get; set; }


        public List<String> GetVariables
        {
            get { return _poly.Select(tuple => tuple.Item1).ToList(); }
        }

        public List<Tuple<string, double>> GetPoly
        {
            get { return _poly; }
        }

        public void AddConstant(int numb)
        {
            Constant += numb;
        }

        public void AddNewVariable(string variable, double coefficient)
        {
            if (!GetVariables.Contains(variable))
                _poly.Add(new Tuple<string, double>(variable, coefficient));
            else
            {
                // TODO: the Variable Already Exist! Find it and ....
                var sElement = new Tuple<string, double>("", 0);
                foreach (var tuple in _poly)
                {
                    if (tuple.Item1 == variable)
                        sElement = new Tuple<string, double>(tuple.Item1, tuple.Item2);
                }
                _poly.Remove(sElement);

                _poly.Add(new Tuple<string, double>(variable, coefficient + sElement.Item2));
            }
        }

        private static string StringHelper(double d)
        {
            if (d < 0)
                return string.Format("({0})", d.ToString());
            return d.ToString();
        }

        public override string ToString()
        {
            if (_poly.Count == 0)
            {
                return Constant.ToString();
            }
            string toStr = string.Empty;

            foreach (var mol in _poly)
            {
                if (mol.Item2 == 1.0f)
                {
                    toStr += mol.Item1 + " + ";
                }
                else
                {
                    if (mol.Item2 < 0f)
                    {
                        toStr += "(" + mol.Item2.ToString(CultureInfo.InvariantCulture) + ")*" + mol.Item1 + " + ";
                    }
                    else
                    {
                        toStr += mol.Item2.ToString(CultureInfo.InvariantCulture) + "*" + mol.Item1 + " + ";
                    }
                }
            }
            toStr = toStr.Remove(toStr.Length - 2, 2);
            if (Constant != 0.0f)
                toStr += " + " + Constant.ToString(CultureInfo.InvariantCulture);
            return toStr;
        }


        public static LPPolynomial JoinPolys(List<LPPolynomial> polys)
        {
            return new LPPolynomial();
        }

        public static LPPolynomial JoinPolys(LPPolynomial poly1, LPPolynomial poly2)
        {
            var rLPoly = new LPPolynomial(poly1);

            foreach (var polyn in poly2.GetPoly)
            {
                rLPoly.AddNewVariable(polyn.Item1, polyn.Item2);
            }

            rLPoly.Constant += poly2.Constant;

            return rLPoly;
        }
    }
}