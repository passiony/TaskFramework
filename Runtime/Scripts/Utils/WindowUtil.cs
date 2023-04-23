using UnityEngine;

public static class WindowUtil
{
    public static T SetPosition<T>(this T t, Vector3 pos) where T : MonoBehaviour
    {
        t.transform.position = pos;
        return t;
    }

    public static T SetLocalPosition<T>(this T t, Vector3 pos) where T : MonoBehaviour
    {
        t.transform.localPosition = pos;
        return t;
    }

    public static T SetPosition<T>(this T t, float x, float y, float z) where T : MonoBehaviour
    {
        t.transform.position = new Vector3(x, y, z);
        return t;
    }

    public static T SetLocalPosition<T>(this T t, float x, float y, float z) where T : MonoBehaviour
    {
        t.transform.localPosition = new Vector3(x, y, z);
        return t;
    }

    public static T SetRotation<T>(this T t, Vector3 eulerAngles) where T : MonoBehaviour
    {
        t.transform.eulerAngles = eulerAngles;
        return t;
    }

    public static T SetLocalRotation<T>(this T t, Vector3 eulerAngles) where T : MonoBehaviour
    {
        t.transform.localEulerAngles = eulerAngles;
        return t;
    }

    public static T SetRotation<T>(this T t, float x, float y, float z) where T : MonoBehaviour
    {
        t.transform.eulerAngles = new Vector3(x, y, z);
        return t;
    }

    public static T SetLocalRotation<T>(this T t, float x, float y, float z) where T : MonoBehaviour
    {
        t.transform.localEulerAngles = new Vector3(x, y, z);
        return t;
    }

    public static T AddToPoint<T>(this T t, Transform point) where T : MonoBehaviour
    {
        t.transform.SetParent(point, false);
        t.transform.localPosition = Vector3.zero;
        t.transform.localEulerAngles = Vector3.zero;
        return t;
    }

    public static T AddToPoint<T>(this T t, TF.Runtime.Point point) where T : MonoBehaviour
    {
        t.transform.position = point.Position;
        t.transform.eulerAngles = point.Rotation;
        return t;
    }
    
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var t = gameObject.GetComponent<T>();
        if (t == null)
        {
            t = gameObject.AddComponent<T>();
        }

        return t;
    }
}