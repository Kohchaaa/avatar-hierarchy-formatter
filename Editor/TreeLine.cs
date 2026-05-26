using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class TreeLine
    {
        static TreeLine()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawTreeLine;
        }

        private static void DrawTreeLine(int instanceID, Rect selectionRect)
        {
            if (!HierarchyFormatterSettings.IsEnabled_TreeView) return;

            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(instanceID, out var cacheData))
            {
                return;
            }

            Color lineColor = HierarchyFormatterSettings.LineColor;

            float baseLeftX = 32f + 14f;

            float indentWidth = 14f;

            float y = selectionRect.y;
            float h = selectionRect.height;
            float halfH = h / 2f;

            for (int i = 0; i < cacheData.ParentLineFlags.Length; i++)
            {
                if (cacheData.ParentLineFlags[i])
                {
                    float lineX = baseLeftX + (i * indentWidth) + (indentWidth / 2f) - 1;

                    Rect verticalLineRect = new Rect(lineX, y, 1f, h);
                    EditorGUI.DrawRect(verticalLineRect, lineColor);
                }
            }

            if (cacheData.IndentLevel > 0)
            {
                int myTargetIndex = cacheData.IndentLevel - 1;
                float myLineX = baseLeftX + (myTargetIndex * indentWidth) + (indentWidth / 2f) - 1;

                if (cacheData.IsLastChild)
                {
                    Rect vLine = new Rect(myLineX, y, 1f, halfH);
                    EditorGUI.DrawRect(vLine, lineColor);
                }
                else
                {
                    Rect vLine = new Rect(myLineX, y, 1f, h);
                    EditorGUI.DrawRect(vLine, lineColor);
                }

                float hLineLength = cacheData.HasChildren ? 8f : 19f;

                Rect hLine = new Rect(myLineX, y + halfH, hLineLength, 1f);
                EditorGUI.DrawRect(hLine, lineColor);
            }
        }
    }
}