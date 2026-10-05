using System;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class DevideLine : IAHFFeature
    {
        public string FeatureName => "DeivdeLine";

        // 区切り線は複数登録され、それぞれ区切る対象が違うので、条件を外から渡してもらう。
        // 設定がONでも、その行には何も描かれないことがある（コンポーネントを持たない
        // ボーンではComponentIconが1つも出ない）。設定だけで判定すると、
        // 隣に何も無い行にまで線が出るので、判定には行のキャッシュを渡す
        private readonly Func<CacheData, bool> _hasNeighbors;

        public DevideLine(Func<CacheData, bool> hasNeighbors)
        {
            _hasNeighbors = hasNeighbors;
        }

        public bool IsEnabled => GeneralSettingModule.IsEnabled_Plugin;

        public void OnGUI(ref AHFLayoutContext c)
        {
            if (!GeneralSettingModule.IsEnabled_Plugin) return;

            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(c.InstanceID, out var cacheData))
            {
                return;
            }

            // 余白を消費する前に判定する。ここで抜ければレイアウトに影響しない
            if (!_hasNeighbors(cacheData)) return;

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