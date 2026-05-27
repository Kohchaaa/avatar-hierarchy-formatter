using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class ComponentIcon
    {
        static ComponentIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawComponentIcon;
        }

        private static void DrawComponentIcon(int instanceID, Rect selectionRect)
        {
            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(instanceID, out var cacheData))
            {
                return;
            }

            float iconSize = 16f;
            float currentX = selectionRect.xMax - iconSize;

            foreach (var iconInfo in cacheData.ComponentIcons)
            {
                Rect iconRect = new Rect(currentX, selectionRect.y, iconSize, selectionRect.height);

                if (iconInfo.CanToggle) GUI.enabled = iconInfo.IsEnabled;

                if (!iconInfo.IsMissing)
                {
                    GUI.Box(iconRect, iconInfo.Icon, GUIStyle.none);
                }
                else
                {
                    GUI.Box(iconRect, EditorGUIUtility.IconContent("console.warnicon").image, GUIStyle.none);
                }

                GUI.enabled = true;

                if (iconInfo.CanToggle && CheckMouseDown(iconRect))
                {
                    ToggleComponentEnabled(iconInfo.InstanceID, instanceID);
                }

                currentX -= iconSize;
            }
        }

        /// <summary>
        /// 指定されたRect内でマウスのクリック（ダウンイベント）があったか判定する
        /// </summary>
        private static bool CheckMouseDown(Rect rect)
        {
            Event e = Event.current;

            if (!rect.Contains(e.mousePosition)) return false;

            if (e.type != EventType.MouseDown) return false;

            if (e.button == 0)
            {
                e.Use();
                return true;
            }

            return false;
        }

        /// <summary>
        /// インスタンスIDからコンポーネントを特定し、有効・無効を反転させる
        /// </summary>
        private static void ToggleComponentEnabled(int componentInstanceID, int gameObjectInstanceID)
        {
            Object obj = EditorUtility.InstanceIDToObject(componentInstanceID);
            if (!obj) return;

            using (SerializedObject so = new SerializedObject(obj))
            {
                SerializedProperty m_Enabled = so.FindProperty("m_Enabled");
                if (m_Enabled != null)
                {
                    m_Enabled.boolValue = !m_Enabled.boolValue;

                    so.ApplyModifiedProperties();

                    HierarchyCacheManager.UpdateSingleComponentCache(gameObjectInstanceID, componentInstanceID, m_Enabled.boolValue);
                }
            }
        }
    }
}