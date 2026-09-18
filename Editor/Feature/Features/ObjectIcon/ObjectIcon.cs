using System.Linq;
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

            var iconId = cacheData.OverrideIconId ?? cacheData.ObjectIconId;
            if (!iconId.HasValue) return;

            var texture = AHFIconRegistry.GetTexture(iconId.Value);
            if (texture == null) return;

            var iconSize = 16;
            Rect iconRect = new Rect(
                c.SelectionRect.x,
                c.SelectionRect.y,
                iconSize,
                iconSize
            );

            float windowWidth = AHFHierarchyWindowUtility.GetWidth() ?? 10000f;
            Rect hoverRect = new Rect(0, c.SelectionRect.y, windowWidth, c.SelectionRect.height);

            // 仮選択中(マウスダウン〜マウスアップの間)は、確定前の本来のSelectionを無視し
            // 仮選択の対象だけを選択中として扱う(排他)。Unityのネイティブ表示と同じ挙動に合わせるため。
            // ただしShift/Ctrlでの追加クリックや、複数選択中の1つを再クリックした場合は
            // 既存の選択を維持したままなので排他にしない。
            var pendingId = AHFPendingSelectionTracker.PendingSelectedInstanceID;
            bool isSelected;
            if (pendingId.HasValue && !AHFPendingSelectionTracker.PendingPreservesSelection)
            {
                isSelected = pendingId.Value == c.InstanceID;
            }
            else
            {
                isSelected = Selection.instanceIDs.Contains(c.InstanceID) || pendingId == c.InstanceID;
            }
            bool isHovered = hoverRect.Contains(Event.current.mousePosition);
            EditorGUI.DrawRect(iconRect, GetRowBackgroundColor(isSelected, isHovered));

            GUI.Box(iconRect, new GUIContent(texture), GUIStyle.none);
        }

        private static Color GetRowBackgroundColor(bool isSelected, bool isHovered)
        {
            if (EditorGUIUtility.isProSkin)
            {
                if (isSelected) return new Color32(44, 93, 135, 255);
                if (isHovered) return new Color32(68, 68, 68, 255);
                return new Color32(56, 56, 56, 255);
            }

            if (isSelected) return new Color32(58, 114, 176, 255);
            if (isHovered) return new Color32(217, 217, 217, 255);
            return new Color32(194, 194, 194, 255);
        }
    }
}