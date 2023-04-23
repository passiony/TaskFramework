using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TF.Editor
{
    public class Unlocker
    {
        private const string FILE_SUFFIX = ".temp";

        [MenuItem("Assets/Unlock")]
        private static void UnlockSelect()
        {
            string[] strs = Selection.assetGUIDs;
            for (var i = 0; i < strs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(strs[i]);
                path = path.Substring(6, path.Length - 6);
                path = $"{Application.dataPath}{path}";
                if (Directory.Exists(path))
                {
                    UnlockDirectory(path);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        string sourceFile = path.Replace("/", "\\");
                        string destFile = sourceFile + FILE_SUFFIX;
                        UnlockFile(sourceFile, destFile);
                    }
                }
            }
        }

        private static void UnlockDirectory(string dirPath)
        {
            try
            {
                string[] files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
                EditorUtility.DisplayProgressBar("Unlock", "unlock files", 0);
                float total = files.Length;
                for (var i = 0; i < files.Length; i++)
                {
                    if (files[i].EndsWith(".meta"))
                    {
                        continue;
                    }

                    EditorUtility.DisplayProgressBar("Unlock", Path.GetFileName(files[i]), i / total);

                    string sourceFile = files[i].Replace("/", "\\");
                    string destFile = sourceFile + FILE_SUFFIX;
                    UnlockFile(sourceFile, destFile);
                }
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", e.Message, "确定");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void UnlockFile(string sourceFile, string destFile)
        {
            CopyFile(sourceFile, destFile);
            File.Delete(sourceFile);
            ReNameFile(destFile, sourceFile);
        }

        private static void CopyFile(string source, string target)
        {
            using (FileStream fsRead = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsWrite = new FileStream(target, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buffer = new byte[1024 * 1024 * 5];

                    while (true)
                    {
                        int r = fsRead.Read(buffer, 0, buffer.Length);
                        if (r == 0)
                        {
                            break;
                        }

                        fsWrite.Write(buffer, 0, r);
                    }
                }
            }
        }

        private static void ReNameFile(string sourceFile, string destFile)
        {
            Process p = new Process();
            p.StartInfo.FileName = Application.dataPath + "/../Tools/Unlock.exe";
            StringBuilder sb = new StringBuilder();
            sb.Append($" -sourcePath=\"{sourceFile}\"");
            sb.Append($" -destPath=\"{destFile}\"");
            p.StartInfo.Arguments = sb.ToString();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    UnityEngine.Debug.Log(e.Data);
                }
            };
            p.Start();
            p.BeginOutputReadLine();
        }
    }

}