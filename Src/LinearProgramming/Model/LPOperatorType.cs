using System.ComponentModel;

namespace LinearProgramming.Model
{
    //Left TO Right
    public enum LPOperatorType
    {
        [Description("=")] Equals,
        [Description("<")] LessThan,
        [Description("<=")] LessOrEqualsTo,
        [Description(">")] MoreThan,
        [Description("=>")] MoreOrEqualsTo
    }
}