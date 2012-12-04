using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LinearProgramming.Model 
{
    public class StandardLPModel : ISerializable
    {

        private readonly List<String> _listOfVariables;
        private readonly List<List<int>> _listOfColumns;
        private readonly LPGoalType _lpGoalType;

        public StandardLPModel(int length , LPGoalType type)
        {
            _listOfVariables = new List<String>(length);
            _listOfColumns = new List<List<int>>(length);
            _lpGoalType = type;
        }

        public LPGoalType GetLPType
        {
            get { return _lpGoalType; }
        }

        public List<String> GetVariables
        {
            get { return _listOfVariables; }
        }
        public List<List<int>> GetColmns
        {
            get { return _listOfColumns; }
        }

        public List<int> GetRow(int index)
        {
            // TODO :: Return ROW at Specified Index
            return null;
        }

        public void AddNewVariable(string varName, List<int> column)
        {
            _listOfVariables.Add(varName);
            _listOfColumns.Add(column);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
