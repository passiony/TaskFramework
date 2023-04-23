using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using Debug = UnityEngine.Debug;

namespace TF.Editor
{
    public class ExcelExporter : EditorWindow
    {
        private static EditorWindow instance;

        // 表格的存放位置
        private const string ConfigFolder = "Config/";

        // 后缀名
        private const string ExtName = ".xlsx";

        // 模板存放位置
        private const string JsonPath = "/Resources/Config/";
        private const string ScriptsPath = "/TaskFramework/Runtime/Scripts/Config/";

        // 导出文件存储
        private const string ExportFileName = "export_files.json";

        private Vector2 scrollPosition;

        private static bool selectAll = false;
        private static bool exportEntity = false;

        private List<ExportFile> exportFiles;

        // 表格数据列表
        private List<TableData> dataList = new List<TableData>();

        //重名文件
        private static string[] DuplicateNameTables =
        {
            "TaskConfig"
        };

        static string ConfigPath
        {
            get
            {
                var root = Directory.GetCurrentDirectory();
                var path = root + "/" + ConfigFolder;
                return path;
            }
        }

        private static EditorWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = EditorWindow.GetWindow(typeof(ExcelExporter));
                }

                return instance;
            }
        }

        [MenuItem("Tools/TaskFramework/Excel Window")]
        public static void Init()
        {
            instance = EditorWindow.GetWindow(typeof(ExcelExporter));
            instance.autoRepaintOnSceneChange = true;
        }

        private void OnEnable()
        {
            exportFiles = SafeReadExportFiles();
        }

        void OnGUI()
        {
            DrawTop();
            DrawTitles();
            DrawExcelsInfo();
        }

        #region Draws

        void DrawTop()
        {
            GUILayout.BeginHorizontal();
            var select = GUILayout.Toggle(selectAll, "全选", GUILayout.Width(50));
            if (select != selectAll)
            {
                selectAll = select;

                for (int i = 0; i < exportFiles.Count; i++)
                {
                    exportFiles[i].export = selectAll;
                }

                SafeWriteExportFiles();
            }

            int count = 0;
            float total = exportFiles.Count;
            foreach (var file in exportFiles)
            {
                if (file.export)
                {
                    count++;
                }
            }

            GUILayout.Label($"{count}/{total}", GUILayout.Width(50));

            GUILayout.Space(100);

            var export = GUILayout.Toggle(exportEntity, "导出实体类", GUILayout.Width(100));
            if (export != exportEntity)
            {
                exportEntity = export;
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("刷新", GUILayout.Width(100)))
            {
                SafeClearExportFiles();
                exportFiles = SafeReadExportFiles();
                Repaint();
            }

            if (GUILayout.Button("打开配置文件夹", GUILayout.Width(100)))
            {
                OpenConfigPath();
            }


            if (GUILayout.Button("导出选中", GUILayout.Width(100)))
            {
                int num = 0;
                string fileName = "";
                EditorUtility.DisplayProgressBar("export excel", "", 0);
                try
                {
                    foreach (var file in exportFiles)
                    {
                        num++;
                        fileName = file.fileName;
                        if (file.export)
                        {
                            ExportExcel(file.fileName);
                        }

                        EditorUtility.DisplayProgressBar("export excel", file.fileName, num / total);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(fileName + ": " + e);
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        public static void OpenConfigPath()
        {
            EditorUtil.ExplorerFolder(ConfigPath);
        }

        void DrawTitles()
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("编号", GUILayout.Width(25));
            GUILayout.Label("是否导出", GUILayout.Width(25));
            GUILayout.Label("表名", GUILayout.Width(100));
            GUILayout.Label("描述", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            GUILayout.Label("单个导出", GUILayout.Width(100));

            EditorGUILayout.EndHorizontal();
        }


        void DrawExcelsInfo()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginVertical();
            for (int i = 0; i < exportFiles.Count; i++)
            {
                var file = exportFiles[i];
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(i.ToString(), GUILayout.Width(25));
                var export = GUILayout.Toggle(file.export, "", GUILayout.Width(25));
                if (export != file.export)
                {
                    file.export = export;
                    SafeWriteExportFiles();
                }

                GUILayout.Label(file.fileName, GUILayout.Width(100));
                string desc = GUILayout.TextField(file.fileDesc, GUILayout.Width(100));
                if (desc != file.fileDesc)
                {
                    file.fileDesc = desc;
                    SafeWriteExportFiles();
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("导出", GUILayout.Width(100)))
                {
                    ExportExcel(file.fileName);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region ExportFiles

        private void SafeClearExportFiles()
        {
            string configJsonPath = ConfigPath + ExportFileName;
            FileUtility.SafeDeleteFile(configJsonPath);
        }

        private void SafeWriteExportFiles()
        {
            string configJsonPath = ConfigPath + ExportFileName;

            var exportJson = JsonConvert.SerializeObject(exportFiles);
            FileUtility.SafeWriteAllText(configJsonPath, exportJson);
        }

        private List<ExportFile> SafeReadExportFiles()
        {
            string configJsonPath = ConfigPath + ExportFileName;
            string exportJson;
            List<ExportFile> exports;

            if (!File.Exists(configJsonPath))
            {
                var files = FileUtility.GetSpecifyFilesInFolder(ConfigPath, "*.xlsx");

                exports = new List<ExportFile>();
                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (!fileName.StartsWith("~"))
                    {
                        var export = new ExportFile(fileName, false, false);
                        exports.Add(export);
                    }
                }

                exportJson = JsonConvert.SerializeObject(exports);
                FileUtility.SafeWriteAllText(configJsonPath, exportJson);
                return exports;
            }

            exportJson = FileUtility.SafeReadAllText(configJsonPath);
            exports = JsonConvert.DeserializeObject<List<ExportFile>>(exportJson);
            return exports;
        }

        #endregion

        #region Excel

        public static void ExportExcel(string fileName)
        {
            string filePath = ConfigPath + fileName + ExtName;
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var workbook = WorkbookFactory.Create(stream);
                ISheet sheet = workbook.GetSheetAt(0);

                if (exportEntity)
                {
                    CreateTemplate(sheet, fileName);
                }

                CreateJson(sheet, fileName);
            }

            Debug.LogWarning($"导出{fileName}.json成功");
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成json文件
        /// </summary>
        private static void CreateJson(ISheet result, string fileName)
        {
            var json = ReadJson(result);

            fileName = "db_" + fileName.Replace(ExtName, "");
            var savePath = Application.dataPath + JsonPath;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            File.WriteAllText(savePath + fileName + ".json", json);
        }

        static string ReadJson(ISheet result)
        {
            // 获取表格有多少列 
            int columns = result.GetRow(3).LastCellNum;
            // 获取表格有多少行 
            int rows = result.LastRowNum + 1;

            List<TableData> dataList = new List<TableData>();
            TableData tempData;
            JArray array = new JArray();

            //第一行为表头，第二行为字段名 第三行为类型 ，不读取
            for (int i = 4; i < rows; i++)
            {
                var first = result.GetRow(i)?.GetCell(0);
                if (first == null)
                {
                    continue;
                }

                for (int j = 0; j < columns; j++)
                {
                    // 获取表格中指定行指定列的数据 
                    var cell = result.GetRow(i)?.GetCell(j);
                    if (cell == null)
                    {
                        continue;
                    }

                    tempData = new TableData();
                    tempData.desc = result.GetRow(1).GetCell(j).ToString();
                    tempData.fieldName = result.GetRow(2).GetCell(j).ToString();
                    tempData.type = result.GetRow(3).GetCell(j).ToString();
                    tempData.value = cell.ToString().Replace("\n", "");

                    dataList?.Add(tempData);
                }

                if (dataList != null && dataList.Count > 0)
                {
                    JObject tempo = new JObject();
                    foreach (var item in dataList)
                    {
                        tempo[item.fieldName] = ExcelTool.GetJArrayValue(item.type, item.value);
                    }

                    array.Add(tempo);
                    dataList.Clear();
                }
            }

            return array.ToString();
        }


        public static string ReadExcel(string fileName)
        {
            string filePath = ConfigPath + fileName + ExtName;
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var workbook = WorkbookFactory.Create(stream);
                ISheet sheet = workbook.GetSheetAt(0);

                return ReadJson(sheet);
            }
        }

        public static void WriteExcel(string fileName, List<TaskStruct> array)
        {
            string filePath = ConfigPath + fileName + ExtName;
            IWorkbook workbook;
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                workbook = WorkbookFactory.Create(stream);
                ISheet sheet = workbook.GetSheetAt(0);

                int columns = sheet.GetRow(3).LastCellNum;
                for (int i = 0; i < array.Count; i++)
                {
                    var data = array[i];
                    for (int j = 0; j < columns; j++)
                    {
                        var fieldName = sheet.GetRow(2).GetCell(j).ToString();
                        var type = sheet.GetRow(3).GetCell(j).ToString();
                        var row = sheet.GetRow(i + 4);
                        if (row == null)
                        {
                            row = sheet.CreateRow(i + 4);
                        }

                        var cell = row.GetCell(j);
                        if (cell == null)
                        {
                            cell = row.CreateCell(j);
                        }

                        var field = data.GetType().GetField(fieldName);
                        var value = field?.GetValue(data);
                        if (value != null)
                        {
                            var str = ExcelTool.GetExcelValue(type, value);
                            cell.SetCellValue(str);
                        }
                    }
                }

                //删除多余行
                var dataCount = array.Count + 4;
                int shift = sheet.LastRowNum + 1 - dataCount;
                if (shift > 0)
                {
                    sheet.ShiftRows(sheet.LastRowNum + 1, sheet.LastRowNum + shift, -shift);
                }
            }

            using (FileStream fileWriter =
                   new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                workbook.Write(fileWriter);
                Debug.LogWarning($"保存{fileName + ExtName}成功");
            }
        }

        #endregion

        #region Templete

        /// <summary>
        /// 生成实体类模板
        /// </summary>
        private static void CreateTemplate(ISheet result, string fileName)
        {
            for (int i = 0; i < DuplicateNameTables.Length; i++)
            {
                if (fileName.Contains(DuplicateNameTables[i]))
                {
                    fileName = (DuplicateNameTables[i]);
                    break;
                }
            }

            string savePath = Application.dataPath + ScriptsPath;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string field = "";
            for (int i = 0; i < result.GetRow(3).LastCellNum; i++)
            {
                string typeStr = result.GetRow(3).GetCell(i).ToString();
                typeStr = typeStr.ToLower();


                // if (typeStr.Contains("[]"))
                // {
                //     typeStr = typeStr.Replace("[]", "");
                //     typeStr = string.Format(" {0}[]", typeStr);
                // }

                string nameStr = result.GetRow(2).GetCell(i).ToString();
                if (nameStr == "id" || nameStr == "key")
                    continue;
                if (string.IsNullOrEmpty(typeStr) || string.IsNullOrEmpty(nameStr))
                    continue;

                field += "public " + typeStr + " " + nameStr + " { get; set; }\r\t";
            }

            fileName = "db_" + fileName.Replace(".xlsx", "");
            string tempStr = TableData.EntityTemplete;
            tempStr = tempStr.Replace("@Name", fileName);
            tempStr = tempStr.Replace("@File1", field);
            File.WriteAllText(savePath + fileName + ".cs", tempStr);
        }

        #endregion
    }


    public class ExportFile
    {
        public string fileName;
        public string fileDesc;
        public bool export;

        public ExportFile(string _fileName, bool _exprot, bool _split)
        {
            this.fileName = _fileName;
            this.export = _exprot;
        }
    }

    public struct TableData
    {
        public string fieldName;
        public string type;
        public string value;
        public string desc;

        /// <summary>
        /// 实体类模板
        /// </summary>
        public const string EntityTemplete =
            "using System;\r" +
            "using UnityEngine;\r" +
            "/// <summary>\r" +
            "/// this class file is auto generate by tools, don't modify it\r" +
            "/// </summary>\r" +
            "public class @Name :IConfig " +
            "{\r\r\t" +
            "public static string configName = \"@Name\";\r\t" +
            "public string version { get; set; }\r\t" +
            "@File1 \r" +
            "}";

        public override string ToString()
        {
            return string.Format("fieldName:{0} type:{1} value:{2}", fieldName, type, value);
        }
    }

}