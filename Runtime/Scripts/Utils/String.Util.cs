using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringUtil
{
    /// <summary>
    /// 字符串转数组
    /// </summary>
    public static string[] String2Array(string str)
    {
        List<string> array = new List<string>();
        var ms = str.Split(',');
        foreach (var item in ms)
        {
            array.Add(item.ToString());
        }

        ;
        return array.ToArray();
    }

    
    public static T[] String2Array<T>(string str)
    {
        List<T> array = new List<T>();
        var ms = str.Split(',');
        foreach (var item in ms)
        {
            array.Add(GetValue<T>(item));
        }

        return array.ToArray();
    }
    
    
    private static T GetValue<T>(object value)
    {
        var t = typeof(T).Name;
        return (T)Convert.ChangeType(value, typeof(T));
    }
    
    /// <summary>
    /// {字符串} 转数组
    /// </summary>
    public static string[] String2Table(string str)
    {
        List<string> array = new List<string>();

        Regex r = new Regex(@"[^{^}]+");
        var ms = r.Matches(str);
        foreach (var item in ms)
        {
            array.Add(item.ToString());
        }

        ;
        return array.ToArray();
    }

    /// <summary>
    /// [字符串] 转数组
    /// </summary>
    public static string[] String2Table2(string str)
    {
        List<string> array = new List<string>();

        Regex r = new Regex(@"[^\[^\]]+");
        var ms = r.Matches(str);
        foreach (var item in ms)
        {
            array.Add(item.ToString());
        }

        ;
        return array.ToArray();
    }

    /// <summary>
    /// string[]转string
    /// </summary>
    /// <returns>string</returns>
    public static string Array2String(string[] array)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in array)
        {
            sb.Append(item + ",");
        }

        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }


    /// <summary>
    /// 字符串转 pos+rot
    /// </summary>
    /// <returns></returns>
    public static bool String2Vector3(string str,out Vector3 pos,out Vector3 rot)
    {
        var arr1 = String2Table2(str);
        var arr = arr1[0].Split('|');
        if (arr.Length < 3)
        {
            Debug.LogError("位置参数长度不够");
            pos = Vector3.zero;
            rot = Vector3.zero;
            return false;
        }

        pos = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
        if (arr.Length < 6)
        {
            rot = Vector3.zero;
            return true;
        }
        
        rot = new Vector3(float.Parse(arr[3]), float.Parse(arr[4]), float.Parse(arr[5]));
        return true;
    }
}