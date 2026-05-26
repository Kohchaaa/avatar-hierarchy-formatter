using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class AvatarSurface
    {
        private readonly static Color avatarHeaderColor = new Color32(128, 148, 174, 45);
        private readonly static Color avatarHeaderLineColor = new Color32(128, 148, 174, 100);
        private readonly static Color avatarContentColor = new Color32(128, 148, 174, 15);
        
        static AvatarSurface()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawAvatarSurface;
        }

        private static void DrawAvatarSurface(int instanceID, Rect selectionRect)
        {
            if (!HierarchyCacheManager.ItemCaches.TryGetValue(instanceID, out var cacheData))
            {
                return;
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
                EditorGUI.DrawRect(line, avatarHeaderLineColor);
                EditorGUI.DrawRect(cardRect, avatarHeaderColor);
            }
            else
            {
                EditorGUI.DrawRect(cardRect, avatarContentColor);
            }
        }
    }
}