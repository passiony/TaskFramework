using TMPro;
using UnityEngine;
using UnityEditor;

namespace TF.Editor
{
    public class ChangeFontWindow : EditorWindow
    {
        private static Font toFont;
        private static TMP_FontAsset tmFont;

        [MenuItem("Tools/Replace Font")]
        public static void ShowWin()
        {
            EditorWindow.CreateInstance<ChangeFontWindow>().Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            // toFont = (Font)EditorGUILayout.ObjectField(new GUIContent("Font:"),toFont, typeof(Font), true, GUILayout.MinWidth(100f));
            tmFont = (TMP_FontAsset)EditorGUILayout.ObjectField(new GUIContent("TMP_FontAsset:"), tmFont,
                typeof(TMP_FontAsset), true, GUILayout.MinWidth(100f));
            GUILayout.Space(10);
            if (GUILayout.Button("Replace Font"))
            {
                ReplaceFont();
            }
        }

        public static void ReplaceFont()
        {
            EditorUtility.DisplayProgressBar("Progress", "Replace Font...", 0);

            string[] sdirs = { "Assets/Third/TaskFramework/Runtime/Resources/Prefab/UI" };
            var asstIds = AssetDatabase.FindAssets("t:Prefab", sdirs);
            int count = 0;
            for (int i = 0; i < asstIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(asstIds[i]);
                var pfb = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                // var texts = pfb.GetComponentsInChildren<Text>(true);
                // foreach (var item in texts)
                // {
                //     item.font = toFont;
                // }

                var tmTexts = pfb.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var item in tmTexts)
                {
                    item.font = tmFont;
                }

                PrefabUtility.SavePrefabAsset(pfb, out bool success);
                if (success)
                {
                    count++;
                }

                EditorUtility.DisplayProgressBar("Replace Font Progress", pfb.name, count / (float)asstIds.Length);
            }

            EditorUtility.ClearProgressBar();
        }
    }

}