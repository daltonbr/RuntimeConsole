using UnityEngine;

namespace DaltonLima.RuntimeConsole
{
    internal class Log
    {
        private string _logString;
        private string _stackTrace;
        private LogType _type;

        public Log(string logString, string stackTrace, LogType type)
        {
            _logString = logString;
            _stackTrace = stackTrace;
            _type = type;
        }
    }    
}
