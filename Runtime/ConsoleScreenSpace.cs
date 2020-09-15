using System;
using System.Collections.Generic;
using UnityEditor;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaltonLima.RuntimeConsole
{
    public class ConsoleScreenSpace : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        private static string _myLog = "";
        private string _output;
        private string _stack;

        [SerializeField] private Sprite infoIcon;
        [SerializeField] private Sprite errorIcon;
        [SerializeField] private Sprite warningIcon;
        
        [SerializeField] private Text text;
        [SerializeField] private Text stackText;
        
        // [SerializeField] private TMP_Text textPro;
        private const string Red = "#FF0000";
        private const string Yellow = "#FFFF00";
        private const string White = "#FFFFFF";
        private const string Black = "#000000";

        // Buttons
        [SerializeField] private Button pauseButton;
        
        [Header("Toggles")]
        [SerializeField] private Toggle infoToggle;
        [SerializeField] private Toggle warnToggle;
        [SerializeField] private Toggle errorToggle;
        
        [Header("-- Toggle Texts")]
        [SerializeField] private Text infoCountText;
        [SerializeField] private Text warnCountText;
        [SerializeField] private Text errorCountText;

        private Queue<Log> _infoLogs;
        private Queue<Log> _warnLogs;
        private Queue<Log> _errorLogs;

        private void OnEnable()
        {
            _infoLogs = new Queue<Log>(1000);
            _warnLogs = new Queue<Log>(1000);
            _errorLogs = new Queue<Log>(1000);
            
            // this event only triggers in the main-thread
            Application.logMessageReceived += HandleLog;
            pauseButton.onClick.AddListener(HandlePause);
            
            // event for all threads, including main thread
            // Application.logMessageReceivedThreaded += LogThreaded;
            
            // some ideas
            //StackTraceLogType logType = Application.GetStackTraceLogType(LogType.Assert);
            //var stackTrace = StackTraceUtility.ExtractStackTrace();
            //Debug.Log($"logType");
        }

        private void LogThreaded(string condition, string stacktrace, LogType type)
        {
            //throw new NotImplementedException();
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
            pauseButton.onClick.RemoveAllListeners();
        }

        private static void HandlePause()
        {
            Debug.Break();
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            var log = new Log(logString, stackTrace, type);
            
            string color = Black;
            switch (type)
            {
                case LogType.Error:
                    _errorLogs.Enqueue(log);
                    var errorCount = _errorLogs.Count;
                    errorCountText.text = errorCount < 1000 ? errorCount.ToString() : "999+";
                    color = Red;
                    break;
                case LogType.Assert:
                    break;
                case LogType.Warning:
                    _warnLogs.Enqueue(log);
                    var warnCount = _warnLogs.Count;
                    warnCountText.text = warnCount < 1000 ? warnCount.ToString() : "999+";
                    color = Yellow;
                    break;
                case LogType.Log:
                    _infoLogs.Enqueue(log);
                    var infoCount = _infoLogs.Count;
                    infoCountText.text = infoCount < 1000 ? infoCount.ToString() : "999+";
                    //color = Black;
                    break;
                case LogType.Exception:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            _output = $"<color={color}>{logString}</color>";
            _stack = stackTrace;
            _myLog = $"{_output}\n{_myLog}";
            if (_myLog.Length > 5000)
            {
                _myLog = _myLog.Substring(0, 4000);
            }
            
            UpdateText();
            
            //TODO: use a String builder. e.g.:
                // var sb = new StringBuilder();
                // sb.AppendLine("<b>Position</b>: " + position);
                // sb.AppendLine("<b>delta</b>: " + delta);
                // sb.AppendLine("<b>eligibleForClick</b>: " + eligibleForClick);
                // sb.AppendLine("<b>pointerEnter</b>: " + pointerEnter);
                // sb.AppendLine("<b>pointerPress</b>: " + pointerPress);
                // sb.AppendLine("<b>lastPointerPress</b>: " + lastPress);
                // sb.AppendLine("<b>pointerDrag</b>: " + pointerDrag);
                // sb.AppendLine("<b>Use Drag Threshold</b>: " + useDragThreshold);
                // sb.AppendLine("<b>Current Raycast:</b>");
                // sb.AppendLine(pointerCurrentRaycast.ToString());
                // sb.AppendLine("<b>Press Raycast:</b>");
                // sb.AppendLine(pointerPressRaycast.ToString());
                // return sb.ToString();
        }

        private void UpdateText()
        {
            text.text = _myLog;
        }
        
        // private void OnGUI()
        // {
        //     //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
        //     {
        //         _myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), _myLog);
        //     }
        // }
        //#endif
    }
}
