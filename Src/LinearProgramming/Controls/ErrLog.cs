using Irony;

namespace LinearProgramming.Controls
{
    public class ErrLog
    {
        public ErrLog(string msg, string location, string state)
        {
            ErrorMessage = msg;
            ParserState = state;
            ErrorLocation = location;
        }

        public ErrLog(string msg, string location, string state, LogMessage originalMessage)
        {
            ErrorMessage = msg;
            ParserState = state;
            ErrorLocation = location;
            LogData = originalMessage;
        }

        public string ErrorMessage { get; set; }
        public string ErrorLocation { get; set; }
        public string ParserState { get; set; }

        public LogMessage LogData { get; set; }
    }
}