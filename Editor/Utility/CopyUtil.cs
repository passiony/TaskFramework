using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TF.Editor
{
    public class CopyUtil : EditorWindow
    {
        private static List<string> copyBuffers = new List<string>();

        private static EditorWindow win;

        [MenuItem("Window/Open Copy Manager", false, 45)]
        private static void CreateWindow()
        {
            win = EditorWindow.GetWindow(typeof(CopyUtil));
            win.titleContent.text = "Copy Manager";
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clean Buffers"))
            {
                copyBuffers = new List<string>();
            }

            if (GUILayout.Button("Copy All"))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var buffer in copyBuffers)
                {
                    sb.AppendLine(buffer);
                }

                var path = sb.ToString();
                GUIUtility.systemCopyBuffer = path;
                Debug.Log(path);
            }

            EditorGUILayout.EndHorizontal();

            foreach (var copyBuffer in copyBuffers)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(copyBuffer);

                if (GUILayout.Button("Copy"))
                {
                    GUIUtility.systemCopyBuffer = copyBuffer;
                    GUILayout.Space(10);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        [MenuItem("GameObject/Copy Relative Path #C", false, -10)]
        private static void CopyRelativePath()
        {
            var selection = Selection.activeObject as GameObject;

            if (selection != null)
            {
                var parent = selection.transform.parent;
                var path = selection.name;

                while (parent != null)
                {
                    path = parent.name + "/" + path;
                    parent = parent.parent;
                }

                Debug.Log(path);
                GUIUtility.systemCopyBuffer = path;
                copyBuffers.Add(path);
            }
        }
    }
}