using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using TF.Runtime;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif

namespace TF.Editor
{
#if ODIN_INSPECTOR
    public class TaskEditorWindow : OdinEditorWindow
#else
    public class TaskEditorWindow : EditorWindow
#endif
    {
        [MenuItem("Tools/TaskFramework/Task Window", false, 0)]
        private static void OpenWindow()
        {
            _instance = GetWindow<TaskEditorWindow>();
        }

        private static TaskEditorWindow _instance;

        public static TaskEditorWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetWindow<TaskEditorWindow>();
                }

                return _instance;
            }
        }

        private bool isFocus = false;
        private Event currentEvent;

        private string configId;
        private string error;
        private Vector2 scrollPosition;

#if ODIN_INSPECTOR
        [Searchable] [ListDrawerSettings(CustomAddFunction = "AddNewTaskData")]
#endif
        public List<TaskData> taskArray = new List<TaskData>();

        private TaskData AddNewTaskData()
        {
            TaskData taskData = new TaskData();
            if (taskArray.Count > 0)
            {
                int currentId = taskArray.Last().id;
                taskData.id = currentId + 1;
            }
            return taskData;
        }

#if ODIN_INSPECTOR
        protected override void OnEnable()
#else
        void OnEnable()
#endif
        {
            EditorApplication.delayCall += OnLostFocus;
        }

        private void OnFocus()
        {
            isFocus = true;
        }

        private void OnLostFocus()
        {
            isFocus = false;
        }

        private void OnValidate()
        {
            error = CheckError();
        }

        private void Update()
        {
            if (isFocus && currentEvent != null)
            {
                if (currentEvent.modifiers.Equals(Event.KeyboardEvent("^S").modifiers) &&
                    currentEvent.keyCode == Event.KeyboardEvent("^S").keyCode)
                {
                    SaveExcel();
                }
            }
        }


        public string GetConfigId()
        {
            return configId;
        }


#if UNITY_EDITOR // Editor-related code must be excluded from builds
#if ODIN_INSPECTOR
        [OnInspectorInit]
#endif
        private void CreateData()
        {
            taskArray.Clear();
            TaskTools.Refresh();
            LoadExcel();
            this.Repaint();
        }

        void RefreshConfigId()
        {
            var main = GameObject.FindObjectOfType<Main>();
            var sceneData = GameObject.FindObjectOfType<SceneData>();
            configId = main.projectId + "-" + sceneData.taskId;
        }

#if ODIN_INSPECTOR
        protected override void OnGUI()
#else   
        protected void OnGUI()
#endif
        {
            currentEvent = Event.current;
            if (TaskTools.IsItemNil())
            {
                CreateData();
            }

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal("box");
            GUILayout.Label($"TaskConfig{configId}.xlsx", GUILayout.Width(150));

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ExcelExporter", GUILayout.Width(100)))
            {
                ExcelExporter.Init();
            }

            if (GUILayout.Button("打开文件夹", GUILayout.Width(100)))
            {
                ExcelExporter.OpenConfigPath();
            }

            if (GUILayout.Button("保存修改", GUILayout.Width(100)))
            {
                // bool result = EditorUtility.DisplayDialog("提示", $"确认要保存TaskConfig{configId}？", "确定", "取消");
                // if (result)
                SaveExcel();
            }

            // if (GUILayout.Button("格式转换", GUILayout.Width(100)))
            // {
            //     bool result = EditorUtility.DisplayDialog("提示", $"确认要转换TaskConfig{configId}？", "确定", "取消");
            //     if (result)
            //     {
            //         ExcelConvert.Convert1(configId);
            //         ExcelConvert.Convert2(configId);
            //     }
            // }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("加载配置", GUILayout.Width(100)))
            {
                taskArray.Clear();
                TaskTools.Refresh();
                LoadExcel();
            }

            if (GUILayout.Button("刷新ID", GUILayout.Width(100)))
            {
                if (taskArray.Count == 0) return;
                for (int i = 0; i < taskArray.Count - 1; i++)
                {
                    int first = taskArray[i].id;
                    int second = taskArray[i + 1].id;
                    if (Mathf.Abs(first - second) < 5)
                    {
                        taskArray[i + 1].id = first + 1;
                    }
                }
            }

            if (GUILayout.Button("生成音频", GUILayout.Width(100)))
            {
                TTSWindow.GenBatch(taskArray);
            }

            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(error))
            {
                GUI.color = Color.red;
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(200));
                GUILayout.TextArea(error);
                GUILayout.EndScrollView();
                GUI.color = Color.white;
            }

            GUILayout.EndVertical();
#if ODIN_INSPECTOR
            base.OnGUI();
#endif
        }

        string CheckError()
        {
            var sb = new StringBuilder();
            foreach (var taskData in taskArray)
            {
                var error = taskData?.GetError();
                if (!string.IsNullOrEmpty(error))
                {
                    sb.AppendLine(error);
                }
            }

            return sb.ToString();
        }

        void LoadExcel()
        {
            RefreshConfigId();
            var fileName = "TaskConfig";
            int current = 0;
            try
            {
                fileName = "TaskConfig" + configId;
                string json = ExcelExporter.ReadExcel(fileName);
                var tasks = JsonConvert.DeserializeObject<List<TaskStruct>>(json);
                foreach (var task in tasks)
                {
                    current = task.id;
                    taskArray.Add(task.ToData());
                }

                error = CheckError();
                Debug.Log("加载成功");
            }
            catch (Exception e)
            {
                Debug.LogError(fileName + "-- taskId:" + current + "-- error:" + e);
            }
        }

        void SaveExcel()
        {
            var stucts = new List<TaskStruct>();
            foreach (var taskData in taskArray)
            {
                stucts.Add(taskData.ToStruct());
            }

            var fileName = "TaskConfig" + configId;
            ExcelExporter.WriteExcel(fileName, stucts);
            ExcelExporter.ExportExcel(fileName);
        }

#endif
    }
}