using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class DevideLine : IAHFFeature
    {
        public string FeatureName => "DeivdeLine";

        public bool IsEnabled => GeneralSettingModule.IsEnabled_Plugin;

        public void OnGUI(ref AHFLayoutContext c)
        {
            if (!GeneralSettingModule.IsEnabled_Plugin) return;

            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(c.InstanceID, out var cacheData))
            {
                return;
            }

            if(!ToggleActiveSettingModule.IsEnabled && !ToggleEOSettingModule.IsEnabled) return;

            Event evt = Event.current;

            float lineWidth = 1f;
            float padding = 4f;
            float paddingAfter = 4f;

            c.RightOffset.CurrentOffset += padding;
            Rect separatorRect = c.RightOffset.GetOffsetRect(c.SelectionRect, lineWidth, padding + paddingAfter);

            separatorRect.y += 1f;
            separatorRect.height = 14f;

            if (evt.type == EventType.Repaint)
            {
                Color separatorColor = new Color(1f, 1f, 1f, 0.4f);
                EditorGUI.DrawRect(separatorRect, separatorColor);
            }
        }
    }
}