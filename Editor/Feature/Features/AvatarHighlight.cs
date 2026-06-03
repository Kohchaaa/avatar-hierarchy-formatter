using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public class AvatarHighlight : IAHFFeature
    {
        public string FeatureName => "AvatarHighlight";
        public bool IsEnabled => AvatarHighlightSettingModule.IsEnabled;

        public void OnGUI(AHFLayoutContext c)
        {
            if (!AvatarHighlightSettingModule.IsEnabled) return;

            if (!HierarchyCacheManager.ItemCaches.TryGetValue(c.InstanceID, out var cacheData))
            {
                return;
            }

            Color color;
            if (AvatarHighlightSettingModule.IsUseThemeColor)
            {
                color = GeneralSettingModule.ThemeColor;
            }
            else
            {
                color = AvatarHighlightSettingModule.OriginalColor;
            }

            float fixedX = 32f;
            float rightEdge = c.SelectionRect.x + c.SelectionRect.width;

            Rect cardRect = new Rect(
                fixedX,
                c.SelectionRect.y,
                rightEdge,
                c.SelectionRect.height
            );

            Rect line = new Rect(
                cardRect.x,
                cardRect.y - 1,
                cardRect.width,
                2f
            );

            if (cacheData.AvatarRootId == c.InstanceID)
            {
                EditorGUI.DrawRect(line, AvatarHighlightSettingModule.GetLineColor(color));
                EditorGUI.DrawRect(cardRect, AvatarHighlightSettingModule.GetHeaderColor(color));
            }
            else
            {
                EditorGUI.DrawRect(cardRect, AvatarHighlightSettingModule.GetContentColor(color));
            }
        }
    }
}