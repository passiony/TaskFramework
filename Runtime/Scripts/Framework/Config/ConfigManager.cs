using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace TF.Runtime
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        string configPath = "Config/";

        public int ProjectId { get; private set; }
        public int TaskId { get; private set; }

        public Dictionary<string, IConfig[]> tableArray =
            new Dictionary<string, IConfig[]>();

        public Dictionary<string, Dictionary<string, IConfig>> tableDic =
            new Dictionary<string, Dictionary<string, IConfig>>();

        public override void Init()
        {
            ReadTable<db_NpcConfig>("db_NpcConfig");
            ReadTable<db_ItemConfig>("db_ItemConfig");
            ReadTable<db_GameConfig>("db_GameConfig");
            ReadTable<db_AudioConfig>("db_AudioConfig");
            ReadTable<db_ExamConfig>("db_ExamConfig");
        }

        public void LoadTaskCfg(int projectId, int taskId)
        {
            ProjectId = projectId;
            TaskId = taskId;

            ReadTable<db_TaskConfig>($"db_TaskConfig{projectId}-{taskId}");
        }

        void ReadTable<T>(string jsonName) where T : IConfig
        {
            var tableName = typeof(T).Name;
            var jsonPath = configPath + jsonName;
            var jsonstr = Resources.Load<TextAsset>(jsonPath);
            if (jsonstr == null)
            {
                Debug.LogError(jsonPath + " not found!!!");
                return;
            }

            var array = JsonConvert.DeserializeObject<T[]>(jsonstr.text);
            if (tableArray.ContainsKey(tableName))
                tableArray.Remove(tableName);
            tableArray.Add(tableName, array);

            var dic = new Dictionary<string, IConfig>();
            foreach (var item in array)
            {
                if (dic.ContainsKey(item.id))
                {
                    Debug.LogError($"{jsonName}.{item.id} 键值已经存在");
                }

                dic.Add(item.id, item);
            }

            if (tableDic.ContainsKey(tableName))
                tableDic.Remove(tableName);
            tableDic.Add(tableName, dic);

            Debug.Log("初始化表格成功:" + tableName);
        }

        public T GetTable<T>(string id) where T : IConfig
        {
            string tableName = typeof(T).Name;
            if (tableDic.TryGetValue(tableName, out Dictionary<string, IConfig> tb))
            {
                if (tb.TryGetValue(id, out IConfig t))
                {
                    return (T)t;
                }

                Debug.LogError($"{tableName}表，找不到配置{id}");
                return null;
            }

            Debug.LogError("找不到配置表:" + tableName);
            return null;
        }

        public T GetTable<T>(Predicate<T> predicate) where T : IConfig
        {
            string tableName = typeof(T).Name;
            if (tableDic.TryGetValue(tableName, out Dictionary<string, IConfig> tb))
            {
                foreach (var pair in tb)
                {
                    var config = pair.Value  as T;
                    if (predicate(config))
                    {
                        return config;
                    }
                }

                return null;
            }

            Debug.LogError("找不到配置表:" + tableName);
            return null;
        }
        
        public T[] GetTables<T>()
        {
            string tableName = typeof(T).Name;
            if (tableArray.TryGetValue(tableName, out IConfig[] tb))
            {
                return tb as T[];
            }

            Debug.LogError("找不到配置表:" + tableName);
            return null;
        }

        public override void Dispose()
        {
        }
    }
}