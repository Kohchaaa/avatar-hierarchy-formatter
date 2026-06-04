using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public class ObjectIcon : IAHFFeature
    {
        public string FeatureName => "ObjectIcon";

        public bool IsEnabled => ObjectIconSettingModule.IsEnabled;

        public void OnGUI(ref AHFLayoutContext c)
        {
            if (!ObjectIconSettingModule.IsEnabled) return;

            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(c.InstanceID, out var cacheData))
            {
                return;
            }

            var iconSize = 16;
            Rect iconRect = new Rect(
                c.SelectionRect.x,
                c.SelectionRect.y,
                iconSize,
                iconSize
            );

            Event evt = Event.current;

            if (evt.type == EventType.Repaint)
            {
                GUIContent iconContent = new GUIContent
                {
                    image = EditorGUIUtility.IconContent("console.warnicon").image
                };

                GUI.Box(iconRect, iconContent, GUIStyle.none);
            }
        }
    }
}