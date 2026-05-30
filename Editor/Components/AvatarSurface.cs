using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class AvatarSurface
    {
        static AvatarSurface()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawAvatarSurface;
        }

        private static void DrawAvatarSurface(int instanceID, Rect selectionRect)
        {
            if (!AvatarSurfaceSettingModule.IsEnabled) return;

            if (!HierarchyCacheManager.ItemCaches.TryGetValue(instanceID, out var cacheData))
            {
                return;
            }

            Color color;
            if (AvatarSurfaceSettingModule.IsUseThemeColor)
            {
                color = GeneralSettingModule.ThemeColor;
            }
            else
            {
                color = AvatarSurfaceSettingModule.OriginalColor;
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
                EditorGUI.DrawRect(line, AvatarSurfaceSettingModule.GetLineColor(color));
                EditorGUI.DrawRect(cardRect,AvatarSurfaceSettingModule.GetHeaderColor(color));
            }
            else
            {
                EditorGUI.DrawRect(cardRect, AvatarSurfaceSettingModule.GetContentColor(color));
            }
        }
    }
}