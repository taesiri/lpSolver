using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LinearProgramming.Parser
{
    //try to Prase 10*x+30*y+3+4
    public class ParserHelper
    {
        private static List<string> _variables;


        public static List<string> ClearUpTheCode(List<string> code)
        {
            try
            {
                var cleanLines = new List<string>();

                foreach (string line in code)
                {
                    string tempLine = line.Replace("\t", "");

                    if (tempLine.Contains("#"))
                        tempLine = tempLine.Replace(
                            tempLine.Substring(tempLine.IndexOf("#", StringComparison.Ordinal)), "");

                    if (tempLine != "")
                        cleanLines.Add(tempLine);
                }
                return cleanLines;
            }
            catch (Exception)
            {
                Debug.WriteLine("Error While Cleaning the Code!");
                return code;
            }
        }

        public static List<string> ExtractVariables(string str)
        {
            try
            {
                _variables = new List<string>();
                str = str.Replace(" ", string.Empty);
                string[] list = str.Split('+');
                foreach (string s in list)
                {
                    AddtoVariables(ExtractSingleElement(s));
                }
                return _variables;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ExtractSingleElement(string str)
        {
            //str = 10*x

            //if contains *
            if (str.Contains("*"))
            {
                string[] lst = str.Split('*');

                return lst[1];
            }
            else
            {
                Match match = Regex.Match(str, @"(\d+)(\w+)");
                //var Coefficient = match.Groups[0].Value;

                string variable = match.Groups[2].Value;
                if (variable != string.Empty)
                    return variable;
            }

            //else

            return null;
        }

        public static void AddtoVariables(string var)
        {
            if (var == null) return;
            if (!_variables.Contains(var)) _variables.Add(var);
        }
    }
}