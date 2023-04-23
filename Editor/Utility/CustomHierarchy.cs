using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using TF.Runtime;
using Object = UnityEngine.Object;

namespace TF.Editor
{
    [InitializeOnLoad]
    public class CustomHierarchy
    {
        // 总的开关用于开启或关闭 显示图标以及彩色文字
        public static bool EnableCustomHierarchy = true;
        public static bool EnableCustomHierarchyLabel = true;

        static CustomHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchWindowOnGui;
        }

        // 用于覆盖原有文字的LabelStyle
        private static GUIStyle LabelStyle(Color color)
        {
            var style = new GUIStyle("Label")
            {
                padding =
                {
                    left = EditorStyles.label.padding.left,
                    top = EditorStyles.label.padding.top + 0
                },
                normal =
                {
                    textColor = color
                }
            };
            return style;
        }

        // 绘制Rect
        private static Rect CreateRect(Rect selectionRect, int index)
        {
            var rect = new Rect(selectionRect);
            rect.x += rect.width - 20 - (20 * index);
            rect.width = 18;
            return rect;
        }

        private static void DrawIcon<T>(Rect rect)
        {
            Texture icon = EditorGUIUtility.ObjectContent(null, typeof(T)).image;
            GUI.Label(rect, icon);
        }

        // 综合以上，根据类型，绘制图标和文字
        private static bool DrawRectIcon<T>(Rect selectionRect, GameObject go, Color textColor, ref int order,
            ref GUIStyle style) where T : Component
        {
            bool selfHasComponent = go.HasComponent<T>(false);
            if (go.HasComponent<T>(true))
            {
                // 绘制新的Label覆盖原有名字
                if (EnableCustomHierarchyLabel)
                {
                    // 字体样式
                    style = LabelStyle(textColor);
                }

                // 图标的绘制排序
                order += 1;
                var rect = CreateRect(selectionRect, order);

                // 绘制图标
                DrawIcon<T>(rect);
            }

            return selfHasComponent;
        }

        private static void HierarchWindowOnGui(int instanceId, Rect selectionRect)
        {
            if (!EnableCustomHierarchy)
            {
                return;
            }

            try
            {
                // 通过ID获得Obj
                Object obj = EditorUtility.InstanceIDToObject(instanceId);
                GameObject go = (GameObject)obj;

                // 绘制Checkbox 
                Rect rectCheck = new Rect(selectionRect);
                rectCheck.x += rectCheck.width - 20;
                rectCheck.width = 18;
                var active = GUI.Toggle(rectCheck, go.activeSelf, string.Empty);
                if (go.activeSelf != active)
                {
                    go.SetActive(active);
                    EditorUtility.SetDirty(go);
                }

                // 图标的序列号
                int index = 0;
                GUIStyle style = null;

                if (go.isStatic)
                {
                    index += 1;
                    var rectIcon = CreateRect(selectionRect, index);
                    var colorTemp = GUI.color;
                    GUI.color = Color.magenta;
                    GUI.Label(rectIcon, "S");
                    GUI.color = colorTemp;
                }

                // 文字颜色定义 
                Color32 colorSceneObject = new Color32(255, 126, 0, 255);
                Color32 colorMain = Color.yellow;
                if (!go.activeSelf)
                {
                    colorSceneObject.a = 100;
                }

                DrawRectIcon<Main>(selectionRect, go, colorMain, ref index, ref style);
                bool selfHasComponent =
                    DrawRectIcon<ISceneObject>(selectionRect, go, colorSceneObject, ref index, ref style);
                if (style != null)
                {
                    selectionRect.x += 17;
                    string showInfo = selfHasComponent ? go.name : $"{go.name}    (↓)";
                    GUI.Label(selectionRect, showInfo, style);
                }
            }
            catch (Exception)
            {
            }
        }
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// 检测是否含有组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="checkChildren">是否检测子层级</param>
        /// <returns></returns>
        public static bool HasComponent<T>(this GameObject go, bool checkChildren) where T : Component
        {
            if (!checkChildren)
            {
                T temp = go.GetComponent<T>();
                return temp != null;
            }

            return go.GetComponentsInChildren<T>(true).FirstOrDefault() != null;
        }
    }
}