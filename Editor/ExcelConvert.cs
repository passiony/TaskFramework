using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace TF.Editor
{
    public static class ExcelConvert
    {
        public static void Convert1(string configId)
        {
            var fileName = "TaskConfig";
            try
            {
                fileName = "TaskConfig" + configId;
                string json = ExcelExporter.ReadExcel(fileName);
                var tasks = JsonConvert.DeserializeObject<List<TaskStruct>>(json);
                foreach (var task in tasks)
                {
                    foreach (var goal in task.goals)
                    {
                        var GName = goal[0];
                        if (GName == "Object")
                        {
                            goal[3] = goal[3] + ":" + goal[4];
                            goal[4] = null;
                        }

                        if (GName == "Touch")
                        {
                            goal[5] = goal[5] + ":" + goal[6];
                            goal[6] = null;
                        }
                    }
                }

                ExcelExporter.WriteExcel(fileName, tasks);
                Debug.Log("转换成功" + fileName);
            }
            catch (Exception e)
            {
                Debug.LogError(fileName + "-- taskId:" + "-- error:" + e);
            }
        }

        public static void Convert2(string configId)
        {
            var fileName = "TaskConfig";
            try
            {
                fileName = "TaskConfig" + configId;
                string json = ExcelExporter.ReadExcel(fileName);
                var tasks = JsonConvert.DeserializeObject<List<TaskStruct>>(json);
                foreach (var task in tasks)
                {
                    foreach (var goal in task.goals)
                    {
                        var GName = goal[0];
                        if (GName == "Talk")
                        {
                            goal[0] = "Panel";
                            if (goal.Length > 4 && !string.IsNullOrEmpty(goal[4]))
                            {
                                goal[4] = "SetOptions:" + goal[4];
                            }

                            if (goal.Length > 5 && !string.IsNullOrEmpty(goal[5]))
                            {
                                goal[5] = "SetCorrect:" + goal[5];
                            }
                        }

                        if (GName == "OpenUI")
                        {
                            goal[0] = "Panel";
                            if (goal.Length > 5 && !string.IsNullOrEmpty(goal[5]))
                            {
                                goal[4] = "SetArgs:" + goal[5];
                                goal[5] = "";
                            }
                        }
                    }
                }

                ExcelExporter.WriteExcel(fileName, tasks);
                Debug.Log("转换成功" + fileName);
            }
            catch (Exception e)
            {
                Debug.LogError(fileName + "-- taskId:" + "-- error:" + e);
            }
        }
    }
}