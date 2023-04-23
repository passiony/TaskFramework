using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace TF.Editor
{

    public static class ExcelTool
    {
        public static string GetExcelValue(string type, object value)
        {
            switch (type)
            {
                case "string":
                case "int":
                case "float":
                case "bool":
                    return value?.ToString();
                case "string[]":
                {
                    var array = (string[])value;
                    string str = "{" + string.Join(",", array.ToArray()) + "}";
                    return str;
                }
                case "string[][]":
                {
                    var array2 = (string[][])value;
                    var sb = "";
                    foreach (var array in array2)
                    {
                        sb += "{" + string.Join(",", array.ToArray()) + "}\n";
                    }

                    sb = sb.TrimEnd('\n');
                    return sb;
                }
            }

            return "";
        }

        public static JToken GetJArrayValue(string type, string value)
        {
            switch (type)
            {
                case "string":
                    return ToValue<string>(value);
                case "int":
                    return ToValue<int>(value);
                case "float":
                    return ToValue<float>(value);
                case "bool":
                    return ToValue<bool>(value);
                case "string[]":
                    return new JArray(ToRegexArray<string>(value));
                case "string[][]":
                    return new JArray(ToRegexJArray<string>(value));
                case "int[]":
                    return new JArray(ToRegexArray<int>(value));
                case "int[][]":
                    return new JArray(ToRegexJArray<int>(value));
                case "float[]":
                    return new JArray(ToRegexArray<float>(value));
                case "float[][]":
                    return new JArray(ToRegexJArray<float>(value));
                case "bool[]":
                    return new JArray(ToRegexArray<bool>(value));
                case "bool[][]":
                    return new JArray(ToRegexJArray<bool>(value));
            }

            return "";
        }

        private static T[] ToRegexArray<T>(string str)
        {
            Regex r = new Regex(@"[^{^}]+");
            var ms = r.Matches(str);

            List<T> array = new List<T>();
            foreach (var item in ms)
            {
                var childs = item.ToString().Split(',');
                foreach (var child in childs)
                {
                    array.Add(ToValue<T>(child));
                }
            }

            return array.ToArray();
        }

        private static JArray[] ToRegexJArray<T>(string str)
        {
            Regex r = new Regex(@"[^{^}]+");
            var ms = r.Matches(str);

            List<JArray> array2 = new List<JArray>();
            foreach (var item in ms)
            {
                var line = item.ToString();
                var array1 = StringUtil.String2Array<T>(line);
                array2.Add(new JArray(array1));
            }

            return array2.ToArray();
        }

        /// <summary>
        /// 字符串拆分列表
        /// </summary>
        private static T[] SplitTo<T>(this string str, char spliteChar)
        {
            string[] ss = str.Split(spliteChar);
            int length = ss.Length;
            List<T> arry = new List<T>(ss.Length);
            for (int i = 0; i < length; i++)
            {
                arry.Add(ss[i].ToValue<T>());
            }

            return arry.ToArray();
        }

        private static T ToValue<T>(this object value)
        {
            var t = typeof(T).Name;
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}