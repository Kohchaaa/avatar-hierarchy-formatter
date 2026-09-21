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
        private const float ButtonHeight = 20f;
        private const float HeaderHeight = 16f;
        private const float GridWidth = ColumnCount * CellSize;

        // アイコン行の最大表示数。これを超えた分は縦スクロールになる。
        // 行単位で切ることで、半端な高さの行が見切れて表示されるのを防ぐ。
        private const int MaxVisibleRows = 6;

        // ウィンドウ枠と中身の間の余白、および要素同士の間隔。
        private const float Padding = 4f;
        private const float Spacing = 4f;

        // GUI.BeginScrollViewが縦スクロールバーのために内部で差し引く幅と同じ計算式。
        // (UnityEngine.GUI.DoBeginScrollView が fixedWidth + margin.left を引いている)
        private static float VerticalScrollbarWidth =>
            GUI.skin.verticalScrollbar.fixedWidth + GUI.skin.verticalScrollbar.margin.left;

        private readonly int _instanceId;
        private readonly List<AHFIconEntry> _builtinEntries;
        private readonly List<AHFIconEntry> _userEntries;
        private readonly GUIContent[] _builtinContents;
        private readonly GUIContent[] _userContents;
        private Vector2 _scroll;

        public AHFIconPickerPopup(int instanceId)
        {
            _instanceId = instanceId;

            var all = AHFIconRegistry.All.ToList();
            _builtinEntries = all.Where(e => !e.IsUserDefined).ToList();
            _userEntries = all.Where(e => e.IsUserDefined).ToList();
            _builtinContents = ToContents(_builtinEntries);
            _userContents = ToContents(_userEntries);
        }

        private static GUIContent[] ToContents(IEnumerable<AHFIconEntry> entries) =>
            entries.Select(e => new GUIContent(e.GetTexture(), e.DisplayName)).ToArray();

        public override Vector2 GetWindowSize()
        {
            // 上下のボタン（自動判定に戻す / フォルダを開く）と余白の分。
            float chromeHeight = Padding * 2f + ButtonHeight * 2f + Spacing * 2f;

            float contentHeight = GetContentHeight();
            float viewHeight = Mathf.Min(contentHeight, GetMaxViewHeight());

            // 縦スクロールバーが出るときだけ、その分だけ窓を広げる。
            float width = Padding * 2f + GridWidth + (contentHeight > viewHeight ? VerticalScrollbarWidth : 0f);
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

            var resetRect = new Rect(innerRect.x, innerRect.y, innerRect.width, ButtonHeight);
            if (GUI.Button(resetRect, "自動判定に戻す"))
            {
                AHFIconOverrideManager.RemoveOverride(_instanceId);
                shouldClose = true;
            }

            var openFolderRect = new Rect(innerRect.x, innerRect.yMax - ButtonHeight, innerRect.width, ButtonHeight);
            if (GUI.Button(openFolderRect, "アイコンフォルダを開く"))
            {
                AHFUserIconFolder.Reveal();
            }

            float scrollTop = resetRect.yMax + Spacing;
            var scrollRect = new Rect(
                innerRect.x,
                scrollTop,
                innerRect.width,
                openFolderRect.y - Spacing - scrollTop
            );

            // GUILayoutを使うとSelectionGridのstyle marginが内容幅に加算され、
            // 実際に必要な幅が読めずに横スクロールバーが出る。
            // Rect指定にして、グリッド幅 = スクロールバーを除いた表示領域幅 に一致させる。
            float contentHeight = GetContentHeight();
            bool needsVerticalScrollbar = contentHeight > scrollRect.height;
            float viewWidth = scrollRect.width - (needsVerticalScrollbar ? VerticalScrollbarWidth : 0f);
            var viewRect = new Rect(0f, 0f, viewWidth, contentHeight);

            _scroll = GUI.BeginScrollView(scrollRect, _scroll, viewRect);

            float y = 0f;
            int clickedBuiltin = DrawSection("組み込み", _builtinContents, viewWidth, ref y);
            y += Spacing;
            int clickedUser = DrawSection("ユーザー", _userContents, viewWidth, ref y);

            GUI.EndScrollView();

            AHFIconEntry picked = null;
            if (clickedBuiltin >= 0) picked = _builtinEntries[clickedBuiltin];
            else if (clickedUser >= 0) picked = _userEntries[clickedUser];

            if (picked != null)
            {
                AHFIconOverrideManager.SetOverride(_instanceId, picked.Id);
                shouldClose = true;
            }

            if (shouldClose)
            {
                HierarchyCacheManager.CacheHierarchyObjectData();
                EditorApplication.RepaintHierarchyWindow();
                editorWindow.Close();
            }
        }

        private static int DrawSection(string header, GUIContent[] contents, float width, ref float y)
        {
            GUI.Label(new Rect(0f, y, width, HeaderHeight), header, EditorStyles.miniLabel);
            y += HeaderHeight;

            if (contents.Length == 0)
            {
                GUI.Label(new Rect(0f, y, width, HeaderHeight), "アイコンがありません", EditorStyles.centeredGreyMiniLabel);
                y += HeaderHeight;
                return -1;
            }

            float height = GetGridHeight(contents.Length);
            int clicked = GUI.SelectionGrid(new Rect(0f, y, width, height), -1, contents, ColumnCount);
            y += height;
            return clicked;
        }

        private float GetContentHeight()
        {
            float builtinHeight = HeaderHeight + GetGridHeight(_builtinContents.Length);
            float userHeight = HeaderHeight + (_userContents.Length > 0 ? GetGridHeight(_userContents.Length) : HeaderHeight);
            return builtinHeight + Spacing + userHeight;
        }

        // 行数の上限はアイコン行にだけ掛ける。見出しは常に見えるようにしたいため。
        private static float GetMaxViewHeight() => MaxVisibleRows * CellSize + HeaderHeight * 2f + Spacing;

        private static float GetGridHeight(int count)
        {
            int rowCount = Mathf.CeilToInt(count / (float)ColumnCount);
            return rowCount * CellSize;
        }
    }
}
