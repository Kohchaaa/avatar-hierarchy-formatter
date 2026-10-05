using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class AHFFeatureManager
    {
        public static readonly List<IAHFFeature> Features = new List<IAHFFeature>();

        static AHFFeatureManager()
        {
            Features.Add(new ObjectIcon());
            Features.Add(new AvatarHighlight());
            Features.Add(new TreeLine());

            // 右側のウィジェットは AHFRightOffsetEngine で右端から詰めていくので、
            // 先に登録したものほど右に出る。実際の並びは左から
            // [ComponentIcon][DevideLine2][ToggleEO][ToggleActive][DevideLine1]
            Features.Add(new DevideLine(_ => HasToggles));
            Features.Add(new ToggleActive());
            Features.Add(new ToggleEO());
            Features.Add(new DevideLine(cache => HasToggles && HasComponentIcons(cache)));
            Features.Add(new ComponentIcon());


            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowGUI;
        }

        private static bool HasToggles =>
            ToggleActiveSettingModule.IsEnabled || ToggleEOSettingModule.IsEnabled;

        // 設定がONでも、その行にアイコンが1つも無ければComponentIconは何も描かない
        private static bool HasComponentIcons(CacheData cache) =>
            ComponentIconSettingModule.IsEnabled
            && cache.ComponentIcons != null
            && cache.ComponentIcons.Length > 0;

        private static void OnHierarchyWindowGUI(int instanceID, Rect selectionRect)
        {
            if (!GeneralSettingModule.IsEnabled_Plugin) return;

            var context = new AHFLayoutContext(instanceID, selectionRect, 4f);

            foreach (var feature in Features)
            {
                if (feature.IsEnabled)
                {
                    feature.OnGUI(ref context);
                }
            }

            // 全Feature処理後(evt.Use()が呼ばれた後)に判定することで、
            // 他Featureが消費したクリックをObjectIconの仮選択として誤検知しないようにする。
            float windowWidth = AHFHierarchyWindowUtility.GetWidth() ?? 10000f;
            Rect rowRect = new Rect(0, selectionRect.y, windowWidth, selectionRect.height);
            AHFPendingSelectionTracker.Update(instanceID, rowRect);
        }
    }
}