/// <summary>
/// 查找场景中具有某些标签的可见物体,返回物体列表 
/// </summary>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TF.Editor
{
    public class TagFinder : EditorWindow
    {
        public int tagCount = 0;
        private int maxTagCount = 10;
        private int minTagCount = 1;
        public List<GameObject> list = new List<GameObject>();
        private bool ifShowList = true;
        private Vector2 scrollVec;
        private string[] tags;
        private bool ifShowWarning = false;

        [MenuItem("Window/Tag/Tag Finder %t")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            TagFinder window = (TagFinder)EditorWindow.GetWindow(typeof(TagFinder));
        }



        void OnEnable()
        {

            tags = new string[maxTagCount];
            ifShowList = false;
        }

        void OnGUI()
        {
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            GUILayout.Label("要查找物体的Tag:", EditorStyles.boldLabel);
            tagCount = EditorGUILayout.IntSlider(tagCount, minTagCount, maxTagCount);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            EditorGUI.indentLevel = 1;

            for (int i = 0; i < tagCount; i++)
            {
                tags[i] = EditorGUILayout.TagField("Tag" + i.ToString(), tags[i]);
                //            Debug.Log ("tags" + i.ToString () + "is " + tags [i]);
            }

            GUILayout.Space(4);

            if (GUILayout.Button("获取物体列表"))
            {
                if (list.Count > 0)
                {
                    list.Clear();
                }

                for (int i = 0; i < tagCount; i++)
                {
                    if (tags[i] == null || tags[i].Equals(string.Empty))
                    {
                        continue;
                    }

                    GameObject[] os = GameObject.FindGameObjectsWithTag(tags[i]);


                    if (os == null)
                    {
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < os.Length; j++)
                        {
                            if (!list.Contains(os[j]))
                                list.Add(os[j]);
                        }
                    }
                }

                //            ifShowList = !ifShowList;
                ifShowList = true;
            }




            if (ifShowList)
            {

                GUILayout.Space(4);
                GUILayout.BeginHorizontal();

                GUILayout.Label("你要找的所有物体如下:", EditorStyles.boldLabel);
                GUILayout.Label("总数:" + list.Count.ToString(), EditorStyles.boldLabel);
                if (GUILayout.Button("清除物体列表"))
                {
                    if (list != null)
                    {
                        list.Clear();
                    }

                    ifShowList = false;
                }

                GUILayout.EndHorizontal();
                EditorGUI.indentLevel = 1;
                float height = list.Count == 0 ? 10f : 300f;
                scrollVec = EditorGUILayout.BeginScrollView(scrollVec, GUILayout.Height(height));

                GUILayout.Space(4);

                for (int i = 0; i < list.Count; i++)
                {
                    EditorGUILayout.ObjectField(list[i], typeof(GameObject), false);
                }

                EditorGUILayout.EndScrollView();

                if (list.Count == 0)
                {
                    ifShowWarning = true;
                }
                else
                {
                    ifShowWarning = false;
                }

                if (ifShowWarning)
                {
                    EditorGUILayout.HelpBox("Oops,没有找到你想要东东哦 !", MessageType.Warning);
                }
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}