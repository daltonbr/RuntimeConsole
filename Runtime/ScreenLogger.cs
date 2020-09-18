using UnityEngine;
using System.Collections.Generic;

namespace DaltonLima.RuntimeConsole
{
    public class ScreenLogger : MonoBehaviour
    {
        private const string Space = " ";
        private const string ScrollToTop = "▬";
        private const string ScrollToBottom = "▬";
        private const string ScrollUp = "▲";
        private const string ScrollDown = "▼";
        private const string ClearView = "CLEAR";

        public static bool IsPersistent = true;

        private static ScreenLogger instance;
        private static bool instantiated = false;

        private Vector2 scrollPosition = Vector2.zero;
        private bool manuallyScroll = false;

        private GUILayoutOption buttonWidth;
        private GUILayoutOption buttonHeight;

        private class LogMessage
        {
            public string Message;
            public LogType Type;

            public LogMessage(string message, LogType type)
            {
                Message = message;
                Type = type;
            }
        }

        public enum LogAnchor
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public bool showLog = true;
        public bool showInEditor = true;
        public bool advancedButtons = false;

        [Tooltip("Height of the log area as a percentage of the screen height")]
        [Range(0.3f, 1.0f)]
        public float height = 0.5f;

        [Tooltip("Width of the log area as a percentage of the screen width")]
        [Range(0.3f, 1.0f)]
        public float width = 0.5f;

        public int margin = 20;

        public LogAnchor anchorPosition = LogAnchor.BottomRight;

        public int fontSize = 14;

        public float scrollSpeed = 10f;

        [Range(0f, 01f)]
        public float backgroundOpacity = 0.5f;
        public Color backgroundColor = Color.black;

        public bool logMessages = true;
        public bool logWarnings = true;
        public bool logErrors = true;

        public Color messageColor = Color.white;
        public Color warningColor = Color.yellow;
        public Color errorColor = new Color(1, 0.5f, 0.5f);

        public bool stackTraceMessages = false;
        public bool stackTraceWarnings = false;
        public bool stackTraceErrors = true;

        private static Queue<LogMessage> queue = new Queue<LogMessage>();

        private GUIStyle styleContainer, styleText;
        private int padding = 5;

        private bool destroying = false;

        /// Singleton instance
        public static ScreenLogger Instance
        {
            get
            {
                if (instantiated) return instance;

                instance = FindObjectOfType<ScreenLogger>();

                // Object not found, we create a new one
                if (instance == null)
                {
                    // Try to load the default prefab
                    try
                    {
                        instance = Instantiate(Resources.Load<ScreenLogger>("ScreenLoggerPrefab"));
                    }
                    catch
                    {
                        Debug.Log("[ScreenLogger] Failed to load default Screen Logger prefab...");
                        instance = new GameObject("ScreenLogger", typeof(ScreenLogger)).GetComponent<ScreenLogger>();
                    }

                    // Problem during the creation, this should not happen
                    if (instance == null)
                    {
                        Debug.LogError("[ScreenLogger] Problem during the creation of ScreenLogger");
                    }
                    else instantiated = true;
                }
                else
                {
                    instantiated = true;
                }

                return instance;
            }
        }

        private void Awake()
        {
            var obj = FindObjectsOfType<ScreenLogger>();

            if (obj.Length > 1)
            {
                Debug.Log("[ScreenLogger] Destroying ScreenLogger, already exists...");

                destroying = true;

                Destroy(gameObject);
                return;
            }

            InitStyles();

            if (IsPersistent)
                DontDestroyOnLoad(this);
        }

        private void InitStyles()
        {
            var background = new Texture2D(1, 1);
            backgroundColor.a = backgroundOpacity;
            background.SetPixel(0, 0, backgroundColor);
            background.Apply();

            styleContainer = new GUIStyle
            {
                normal = {background = background},
                wordWrap = false,
                padding = new RectOffset(padding, padding, padding, padding)
            };

            styleText = new GUIStyle
            {
                fontSize = fontSize,
                wordWrap = true
            };
        }

        private void OnEnable()
        {
            if (!showInEditor && Application.isEditor) return;
            queue = new Queue<LogMessage>();
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            // If destroyed because already exists, don't need to de-register callback
            if (destroying) return;
            Application.logMessageReceived -= HandleLog;
        }

        private void Update()
        {
            if (!showInEditor && Application.isEditor) return;

            float innerHeight = (Screen.height - 2 * margin) * height - 2 * padding;
            int totalRows = (int) (innerHeight / styleText.lineHeight);

            // Remove overflowing rows
            while (queue.Count > totalRows)
                queue.Dequeue();
        }

        private void OnGUI()
        {
            if (!showLog) return;
            if (!showInEditor && Application.isEditor) return;

            if (advancedButtons && (buttonWidth == null || buttonHeight == null))
            {
                buttonWidth = GUILayout.Width(25f);
                buttonHeight = GUILayout.Height(25f);
            }

            float w = (Screen.width - 2 * margin) * width;
            float h = (Screen.height - 2 * margin) * height;
            float x = 1, y = 1;

            switch (anchorPosition)
            {
                case LogAnchor.BottomLeft:
                    x = margin;
                    y = margin + (Screen.height - 2 * margin) * (1 - height);
                    break;

                case LogAnchor.BottomRight:
                    x = margin + (Screen.width - 2 * margin) * (1 - width);
                    y = margin + (Screen.height - 2 * margin) * (1 - height);
                    break;

                case LogAnchor.TopLeft:
                    x = margin;
                    y = margin;
                    break;

                case LogAnchor.TopRight:
                    x = margin + (Screen.width - 2 * margin) * (1 - width);
                    y = margin;
                    break;
            }

            GUILayout.BeginArea(new Rect((int) x, (int) y, w, h), styleContainer);

            if (advancedButtons)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(buttonWidth);

                if (GUILayout.Button(ScrollToTop, buttonHeight))
                {
                    manuallyScroll = true;
                    scrollPosition.y = 0;
                }

                GUILayout.Space(10f);

                if (GUILayout.RepeatButton(ScrollUp, buttonHeight))
                {
                    manuallyScroll = true;
                    scrollPosition.y -= scrollSpeed;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.RepeatButton(ScrollDown, buttonHeight))
                {
                    manuallyScroll = true;
                    scrollPosition.y += scrollSpeed;
                }

                GUILayout.Space(10f);

                if (GUILayout.Button(ScrollToBottom, buttonHeight))
                {
                    manuallyScroll = false;
                }

                if (!manuallyScroll)
                {
                    scrollPosition.y = int.MaxValue;
                }

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

            var started = false;

            foreach (LogMessage m in queue)
            {
                switch (m.Type)
                {
                    case LogType.Warning:
                        styleText.normal.textColor = warningColor;
                        break;

                    case LogType.Log:
                        styleText.normal.textColor = messageColor;
                        break;

                    case LogType.Assert:
                    case LogType.Exception:
                    case LogType.Error:
                        styleText.normal.textColor = errorColor;
                        break;

                    default:
                        styleText.normal.textColor = messageColor;
                        break;
                }

                if (!m.Message.StartsWith(Space) && started)
                    GUILayout.Space(fontSize);

                GUILayout.Label(m.Message, styleText);
                started = true;
            }

            GUILayout.EndScrollView();

            if (advancedButtons)
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                if (GUILayout.Button(ClearView, buttonHeight))
                {
                    queue.Clear();
                }
            }

            GUILayout.EndArea();
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            // ignoring unwanted LogTypes
            switch (type)
            {
                case LogType.Assert when !logErrors:
                case LogType.Error when !logErrors:
                case LogType.Exception when !logErrors:
                case LogType.Log when !logMessages:
                case LogType.Warning when !logWarnings:
                    return;
            }

            string[] lines = message.Split(new char[] { '\n' });

            foreach (string line in lines)
                queue.Enqueue(new LogMessage(line, type));

            // ignoring unwanted stack traces
            switch (type)
            {
                case LogType.Assert when !stackTraceErrors:
                case LogType.Error when !stackTraceErrors:
                case LogType.Exception when !stackTraceErrors:
                case LogType.Log when !stackTraceMessages:
                case LogType.Warning when !stackTraceWarnings:
                    return;
            }

            string[] trace = stackTrace.Split(new char[] { '\n' });

            foreach (string t in trace)
                if (t.Length != 0) queue.Enqueue(new LogMessage($"    {t}", type));
        }

        public void InspectorGUIUpdated()
        {
            InitStyles();
        }
    }
}
