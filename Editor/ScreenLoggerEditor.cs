using UnityEngine;
using UnityEditor;

namespace DaltonLima.RuntimeConsole.Editor
{
    [CustomEditor(typeof(ScreenLogger))]
    public class ScreenLoggerEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/Create Other/Screen Logger")]
        private static void AddScreenLogger()
        {
            if (FindObjectOfType<ScreenLogger>() == null)
            {
                GameObject gameObject = new GameObject();
                gameObject.name = "ScreenLogger";
                gameObject.AddComponent<ScreenLogger>();
            }
            else
            {
                Debug.LogError("[ScreenLoggerEditor] ScreenLogger already added to the scene.");
            }
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            ScreenLogger.Instance.InspectorGUIUpdated();
        }
    }
}
