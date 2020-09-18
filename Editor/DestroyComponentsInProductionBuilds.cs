#if !DEVELOPMENT_BUILD //Not working?
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
 
namespace DaltonLima.RuntimeConsole.Editor
{
    public static class DestroyComponentsInProductionBuilds
    {
        private static readonly Type[] TypesToDeleteOnBuild = {
            typeof(RuntimeConsole.ScreenLogger),
            typeof(RuntimeConsole.ConsoleScreenSpace)
        };
 
        [PostProcessScene (0)]
        //[PostProcessBuild]
        public static void DestroyComponents()
        {
            Debug.Log($"[DestroyComponentsInProductionBuilds] Start PostProcessScene {SceneManager.GetActiveScene().name}!");
            if (BuildPipeline.isBuildingPlayer && !Debug.isDebugBuild)
            {
                foreach (var type in TypesToDeleteOnBuild)
                {
                    // PostProcessScene iterate on every build scene
                    Debug.Log($"[DestroyComponentsInProductionBuilds] Destroying all instances of {type.Name} on build!");
                    foreach (var component in ObjectFinder.FindObjectsOfTypeInActiveScene(type, true))
                    {
                        Debug.Log($"[DestroyComponentsInProductionBuilds] Found {component.gameObject.name} with the component {nameof(type)}");
                        Object.DestroyImmediate(component);
                    }
                }
            }
            else
            {
                Debug.Log($"[DestroyComponentsInProductionBuilds] Not BuildingPlayer. Aborted");
            }
        }
    }
    
}
#endif // !DEVELOPMENT_BUILD