using System;
using System.Collections.Generic;
using Quick;
using TF.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

public static class GameUtil
{
    public static db_goal_data GetGoalClass(EGoalType goalType, params string[] args)
    {
        switch (goalType)
        {
            case EGoalType.Panel:
                return new panel_goal_data(args);
            case EGoalType.Touch:
                return new touch_goal_data(args);
            case EGoalType.Hilight:
                return new hilight_goal_data(args);
            case EGoalType.Teleport:
                return new teleport_goal_data(args);
            case EGoalType.Audio:
                return new audio_goal_data(args);
            case EGoalType.Object:
                return new object_goal_data(args);
            default:
                throw new ArgumentOutOfRangeException(nameof(goalType), goalType, null);
        }
    }

    public static string[] TryGetSplit(this string[] args, int index, char separator = '|')
    {
        if (args.Length <= index)
        {
            return Array.Empty<string>();
        }

        var array = args[index].Split(separator);
        if (array.Length == 1 && string.IsNullOrEmpty(array[0]))
        {
            return Array.Empty<string>();
        }

        return array;
    }

    public static int[] TryGetIntSplit(this string[] args, int index, char separator = '|')
    {
        if (args.Length <= index)
        {
            return Array.Empty<int>();
        }

        var array = args[index].SplitToInt(separator);
        return array;
    }

    public static string TryGetValue(this string[] args, int index)
    {
        if (args.Length <= index)
        {
            return string.Empty;
        }

        return args[index];
    }

    public static string TryGetValue(this object[] args, int index)
    {
        if (args.Length <= index)
        {
            return string.Empty;
        }

        return args[index].ToString();
    }

    public static int TryGetIntValue(this object[] args, int index)
    {
        if (args.Length <= index)
        {
            return 0;
        }

        var arg = args[index].ToString();
        if (string.IsNullOrEmpty(arg))
        {
            return 0;
        }

        return Convert.ToInt32(arg);
    }

    public static float TryGetFloatValue(this object[] args, int index)
    {
        if (args.Length <= index)
        {
            return 0;
        }

        return Convert.ToSingle(args[index]);
    }

    public static int TryGet(this int[] args, int index)
    {
        if (args.Length <= index)
        {
            return 0;
        }

        return args[index];
    }

    public static bool TryGetBoolValue(this object[] args, int index)
    {
        if (args.Length <= index)
        {
            return false;
        }

        return Convert.ToBoolean(args[index]);
    }

    public static int[] SplitToInt(this string value, params char[] chars)
    {
        var arr = value.Split(chars);
        return arr.ToIntArr();
    }

    public static Vector3 SplitToVector3(this string value, params char[] chars)
    {
        var arr = value.Split(chars);
        var intArr = arr.ToFloatArr();

        var ret = new Vector3();
        if (arr.Length == 1)
        {
            ret.x = intArr[0];
            ret.y = ret.x;
            ret.z = ret.x;
        }
        else if (arr.Length == 2)
        {
            ret.x = intArr[0];
            ret.y = intArr[1];
            ret.z = ret.y;
        }
        else if (arr.Length == 3)
        {
            ret.x = intArr[0];
            ret.y = intArr[1];
            ret.z = intArr[2];
        }

        return ret;
    }

    public static int[] ToIntArr(this string[] args)
    {
        if (args.Length == 0)
        {
            return Array.Empty<int>();
        }

        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            return Array.Empty<int>();
        }

        var arr = new int[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            arr[i] = Int32.Parse(args[i]);
        }

        return arr;
    }

    public static float[] ToFloatArr(this string[] args)
    {
        if (args.Length == 0)
        {
            return Array.Empty<float>();
        }

        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            return Array.Empty<float>();
        }

        var arr = new float[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            arr[i] = float.Parse(args[i]);
        }

        return arr;
    }

    public static int ToInt(this string args)
    {
        if (int.TryParse(args, out int value))
        {
            return value;
        }

        return 0;
    }

    public static Color ToColor(this string name)
    {
        switch (name)
        {
            case "red":
                return Color.red;
            case "green":
                return Color.green;
            case "blue":
                return Color.blue;
            case "white":
                return Color.white;
            case "black":
                return Color.black;
            case "yellow":
                return Color.yellow;
            case "cyan":
                return Color.cyan;
            case "magenta":
                return Color.magenta;
            case "gray":
                return Color.gray;
            case "grey":
                return Color.grey;
        }

        return Color.green;
    }

    public static T ToEnum<T>(this string name) where T : struct, Enum
    {
        if (Enum.TryParse(name, true, out T State))
        {
            return State;
        }

        Debug.LogError("枚举解析失败:" + name);
        return default;
    }

    public static void AddHilight(GameObject go, int width, Color color)
    {
        var outline = go.GetOrAddComponent<Outline>();
        outline.OutlineWidth = width;
        outline.OutlineColor = color;
    }

    public static void RemoveHilight(GameObject go)
    {
        var outline = go.GetComponent<Outline>();
        Object.Destroy(outline);
    }

    public static bool IsNullOrEmpty<T>(this IList<T> list)
    {
        return list == null || list.Count == 0;
    }
}