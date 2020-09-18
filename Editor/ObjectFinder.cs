using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaltonLima.RuntimeConsole.Editor
{
    public class ObjectFinder
    {
        /// Use this method to get all loaded objects of some type, including inactive objects.
        /// This is an alternative to Resources.FindObjectsOfTypeAll (returns project assets, including prefabs), and GameObject.FindObjectsOfTypeAll (deprecated).
        public static IEnumerable<Component> FindObjectsOfTypeInActiveScene(Type type, bool findInactive = false)
        {
            var scene = SceneManager.GetActiveScene();
            Debug.Log($"Finding objects in scene {scene.name}");
            var results = new List<Component>();
        
            var allGameObjects = scene.GetRootGameObjects();
    
            foreach (var go in allGameObjects)
            {
                results.AddRange(go.GetComponentsInChildren(type, findInactive));
            }
        
            return results;
        }

        public static IEnumerable<Component> FindObjectsOfTypeAll(Type type, bool findInactive = false)
        {
            var results = new List<Component>();
            var sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var allGameObjects = scene.GetRootGameObjects();
                foreach (var go in allGameObjects)
                {    
                    results.AddRange(go.GetComponentsInChildren(type, findInactive));
                }
            }

            Debug.Log($"Find {results.Count} objects in all scenes");
            return results;
        }
        
    }
}