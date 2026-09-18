using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class AHFIconPickerPopup : PopupWindowContent
    {
        private const int ColumnCount = 6;
        private const float CellSize = 32f;
        private const float ResetButtonHeight = 20f;
        private const int MaxVisibleRows = 3;
        private const float GridWidth = ColumnCount * CellSize;

        private const float Padding = 4f;
        private const float Spacing = 4f;

        private static float VerticalScrollbarWidth =>
            GUI.skin.verticalScrollbar.fixedWidth + GUI.skin.verticalScrollbar.margin.left;

        private readonly int _instanceId;
        private readonly List<AHFIconEntry> _entries;
        private readonly GUIContent[] _contents;
        private Vector2 _scroll;

        public AHFIconPickerPopup(int instanceId)
        {
            _instanceId = instanceId;
            _entries = AHFIconRegistry.All.ToList();
            _contents = _entries.Select(e => new GUIContent(e.GetTexture(), e.DisplayName)).ToArray();
        }

        public override Vector2 GetWindowSize()
        {
            float chromeHeight = Padding * 2f + ResetButtonHeight + Spacing;
            float gridHeight = GetGridHeight();
            float viewHeight = Mathf.Min(gridHeight, MaxVisibleRows * CellSize);

            float width = Padding * 2f + GridWidth + (gridHeight > viewHeight ? VerticalScrollbarWidth : 0f);
            return new Vector2(width, chromeHeight + viewHeight);
        }

        public override void OnGUI(Rect rect)
        {
            bool shouldClose = false;

            var innerRect = new Rect(
                rect.x + Padding,
                rect.y + Padding,
                rect.width - Padding * 2f,
                rect.height - Padding * 2f
            );

            var buttonRect = new Rect(innerRect.x, innerRect.y, innerRect.width, ResetButtonHeight);
            if (GUI.Button(buttonRect, "自動判定に戻す"))
            {
                AHFIconOverrideManager.RemoveOverride(_instanceId);
                shouldClose = true;
            }

            var scrollRect = new Rect(
                innerRect.x,
                buttonRect.yMax + Spacing,
                innerRect.width,
                innerRect.yMax - (buttonRect.yMax + Spacing)
            );

            float gridHeight = GetGridHeight();
            bool needsVerticalScrollbar = gridHeight > scrollRect.height;
            float viewWidth = scrollRect.width - (needsVerticalScrollbar ? VerticalScrollbarWidth : 0f);
            var viewRect = new Rect(0f, 0f, viewWidth, gridHeight);

            _scroll = GUI.BeginScrollView(scrollRect, _scroll, viewRect);
            int clickedIndex = GUI.SelectionGrid(viewRect, -1, _contents, ColumnCount);
            GUI.EndScrollView();

            if (clickedIndex >= 0)
            {
                AHFIconOverrideManager.SetOverride(_instanceId, _entries[clickedIndex].Id);
                shouldClose = true;
            }

            if (shouldClose)
            {
                HierarchyCacheManager.CacheHierarchyObjectData();
                EditorApplication.RepaintHierarchyWindow();
                editorWindow.Close();
            }
        }

        private float GetGridHeight()
        {
            int rowCount = Mathf.CeilToInt(_entries.Count / (float)ColumnCount);
            return rowCount * CellSize;
        }
    }
}
