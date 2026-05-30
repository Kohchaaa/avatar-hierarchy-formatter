using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public class AvatarHighlight : IAHFFeature
    {
        /*         static AvatarHighlight()
                {
                    EditorApplication.hierarchyWindowItemOnGUI += DrawAvatarSurface;
                } */

        public string FeatureName => "AvatarHighlight";
        public bool IsEnabled => AvatarHighlightSettingModule.IsEnabled;

        public void OnGUI(int instanceID, Rect selectionRect)
        {
            if (!AvatarHighlightSettingModule.IsEnabled) return;

            if (!HierarchyCacheManager.ItemCaches.TryGetValue(instanceID, out var cacheData))
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
            float rightEdge = selectionRect.x + selectionRect.width;

            Rect cardRect = new Rect(
                fixedX,
                selectionRect.y,
                rightEdge,
                selectionRect.height
            );

            Rect line = new Rect(
                cardRect.x,
                cardRect.y - 1,
                cardRect.width,
                2f
            );

            if (cacheData.AvatarRootId == instanceID)
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