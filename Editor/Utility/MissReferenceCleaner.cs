using UnityEditor;
using UnityEngine;

namespace TF.Editor
{
    public class MissReferenceCleaner : UnityEditor.Editor
    {

        [MenuItem("GameObject/Clear Missing Scripts", false, 45)]
        public static void CleanupMissingScripts()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                var gameObject = Selection.gameObjects[i];
                RemoveMissScript(gameObject.transform);

                Debug.Log("Done");
            }
        }

        static void RemoveMissScript(Transform transform)
        {
            int num = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(transform.gameObject);
            if (num > 0)
            {
                Debug.Log("remove missing script " + transform.name + "; num = " + num);
            }

            foreach (Transform child in transform.transform)
            {
                RemoveMissScript(child);
            }
        }
    }
}