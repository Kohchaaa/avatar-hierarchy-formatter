using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public class ComponentIcon : IAHFFeature
    {
        public string FeatureName => "ComponentIcon";
        public bool IsEnabled => ComponentIconSettingModule.IsEnabled;

        public void OnGUI(AHFLayoutContext c)
        {
            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(c.InstanceID, out var cacheData))
            {
                return;
            }

            if (!ComponentIconSettingModule.IsEnabled)
            {
                return;
            }

            float iconSize = 16f;
            float offsetX = c.SelectionRect.xMax - iconSize - ComponentIconSettingModule.IconOffset;

            foreach (var icon in cacheData.ComponentIcons)
            {
                Rect iconRect = new Rect(offsetX, c.SelectionRect.y, iconSize, c.SelectionRect.height);

                string tooltipText = "";

                if (icon.IsMissing)
                {
                    tooltipText = "Missing Script";
                }
                else if (icon.InstanceIDs != null && icon.InstanceIDs.Length > 0)
                {
                    var componentNames = getComponentNames(icon.InstanceIDs);
                    tooltipText = string.Join("\n", componentNames);
                }

                GUIContent iconContent = new GUIContent
                {
                    image = icon.IsMissing ? EditorGUIUtility.IconContent("console.warnicon").image : icon.Icon,
                    tooltip = tooltipText
                };


                if (icon.CanToggle) GUI.enabled = icon.IsEnabled;

                GUI.Box(iconRect, iconContent, GUIStyle.none);

                GUI.enabled = true;


                if (ComponentIconSettingModule.IsAllowToggleFromIcon)
                {
                    if (CheckMouseDown(iconRect))
                    {
                        if (icon.CanToggle)
                        {
                            ToggleComponentEnabled(icon.InstanceIDs, c.InstanceID);
                        }
                    }
                }

                offsetX -= iconSize + 2f;
            }
        }

        private static List<string> getComponentNames(int[] IDs)
        {
            List<string> names = new List<string>();
            foreach (int id in IDs)
            {
                var comp = EditorUtility.InstanceIDToObject(id) as Component;
                if (comp != null)
                {
                    names.Add(comp.GetType().Name);
                }
            }

            return names;
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
        private static void ToggleComponentEnabled(int[] instanceIDs, int gameObjectInstanceID)
        {
            if (instanceIDs == null || instanceIDs.Length == 0) return;


            List<Component> validComponents = new List<Component>();
            foreach (int id in instanceIDs)
            {
                Component comp = EditorUtility.InstanceIDToObject(id) as Component;
                if (comp != null) validComponents.Add(comp);
            }

            if (validComponents.Count == 0) return;


            bool currentStatus = HierarchyCacheManager.GetEnabledState(validComponents[0]);
            bool targetStatus = !currentStatus;

            foreach (Component comp in validComponents)
            {
                HierarchyCacheManager.SetEnabledState(comp, targetStatus);
            }

            // キャッシュに通知して一部だけ再描画
            GameObject go = EditorUtility.InstanceIDToObject(gameObjectInstanceID) as GameObject;
            if (go != null)
            {
                Component[] filteredComponents = AHFUtil.GetFilteredComponents(go);
                HierarchyCacheManager.UpdateGameObjectCache(gameObjectInstanceID, filteredComponents);
            }
        }
    }
}