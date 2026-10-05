using System;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class DevideLine : IAHFFeature
    {
        public string FeatureName => "DeivdeLine";

        // 区切り線は複数登録され、それぞれ区切る対象が違うので、条件を外から渡してもらう。
        // 判定は設定単位で、行ごとの内容は見ない。
        // 行にコンポーネントアイコンが1つも無い時に線を消す実装を試したが、
        // ボーンの行だけ線が抜けて列が揃わず、かえって不揃いに見えたため戻した。
        // 線は「隣に何かがある印」ではなく「列の区切り」として扱う
        private readonly Func<bool> _hasNeighbors;

        public DevideLine(Func<bool> hasNeighbors)
        {
            _hasNeighbors = hasNeighbors;
        }

        public bool IsEnabled => GeneralSettingModule.IsEnabled_Plugin && _hasNeighbors();

        public void OnGUI(ref AHFLayoutContext c)
        {
            if (!GeneralSettingModule.IsEnabled_Plugin) return;

            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(c.InstanceID, out var cacheData))
            {
                return;
            }

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
                Color separatorColor = EditorGUIUtility.isProSkin
                    ? new Color(1f, 1f, 1f, 0.4f)
                    : new Color(0f, 0f, 0f, 0.4f);
                EditorGUI.DrawRect(separatorRect, separatorColor);
            }
        }
    }
}