using System.Collections.Generic;

namespace LinearProgramming.Parser
{
    public class CodeBlocks
    {
        private readonly List<string> _listOfLines;

        public CodeBlocks(IEnumerable<string> data)
        {
            _listOfLines = new List<string>(data);
        }

        public List<string> GetList()
        {
            return _listOfLines;
        }

        public void PushItem(string str)
        {
            _listOfLines.Add(str);
        }
    }
}