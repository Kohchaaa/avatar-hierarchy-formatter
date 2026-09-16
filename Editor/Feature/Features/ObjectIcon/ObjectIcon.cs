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

            if (!cacheData.ObjectIconId.HasValue) return;

            var texture = AHFIconRegistry.GetTexture(cacheData.ObjectIconId.Value);
            if (texture == null) return;

            var iconSize = 16;
            Rect iconRect = new Rect(
                c.SelectionRect.x,
                c.SelectionRect.y,
                iconSize,
                iconSize
            );

            GUI.Box(iconRect, new GUIContent(texture), GUIStyle.none);
        }
    }
}