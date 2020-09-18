// using System;
// using UnityEngine;
// using UnityEditor.Build;
// using UnityEditor.Build.Reporting;
// using Object = UnityEngine.Object;
//
// namespace DaltonLima.RuntimeConsole.Editor
// {
//     #if !DEVELOPMENT_BUILD
//     // We set the tag "EditorOnly" to types that we want to remove from Non-Development Builds
//     public class RemoveConsolePrefabsInProductionBuilds : IPreprocessBuildWithReport
//     {
//         private static readonly Type[] TypesToDeleteOnBuild = {
//             typeof(RuntimeConsole.ConsoleScreenSpace),
//             typeof(RuntimeConsole.ScreenLogger)
//         };
//         
//         public int callbackOrder { get; }
//         public void OnPreprocessBuild(BuildReport report)
//         {
//             if (Debug.isDebugBuild)
//             {
//                 Debug.Log("[RemoveConsolePrefabsInProductionBuilds] Not removing ConsolePrefabs in Dev Builds");
//                 return;
//             }
//             // Find all elements that we want to tag
//             foreach (var type in TypesToDeleteOnBuild)
//             {
//                 var allGameObjects = ObjectFinder.FindObjectsOfTypeAll(type, true);
//
//                 foreach (var component in allGameObjects)
//                 {
//                     Debug.Log($"[RemoveConsolePrefabs] Found {component.gameObject.name} set its tag to {component.gameObject.tag}");
//                     component.gameObject.tag = Debug.isDebugBuild ? "EditorOnly" : "Untagged";                     
//                 }
//             }
//             
//         }
//     }
//     #endif //DEVELOPMENT_BUILD
// }
