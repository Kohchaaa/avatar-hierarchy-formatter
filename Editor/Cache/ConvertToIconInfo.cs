using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Kohcha.AvatarHierarchyFormatter
{
    public static partial class HierarchyCacheManager
    {
        /// <summary>
        /// コンポーネント配列を、描画用のアイコン情報リストに変換して集約
        /// </summary>
        public static ComponentIconInfo[] ConvertToIconInfos(Component[] rawComponents)
        {
            if (rawComponents == null || rawComponents.Length == 0)
                return Array.Empty<ComponentIconInfo>();

            List<ComponentIconInfo> finalIcons = new List<ComponentIconInfo>();
            HashSet<Component> processedComponents = new HashSet<Component>();

            //========================================================
            // グループごとにまとめる
            foreach (var def in IconGroupList)
            {
                List<Component> groupComponents = new List<Component>();

                foreach (var comp in rawComponents)
                {
                    if (comp == null || !comp || processedComponents.Contains(comp) || comp is Transform) continue;

                    if (def.IsMatch(comp))
                    {
                        groupComponents.Add(comp);
                    }
                }

                // マッチしたコンポーネント群を1つのグループアイコンに集約
                if (groupComponents.Count > 0)
                {
                    // アイコンにどのコンポーネントが含まれてるかIDを収集
                    int[] ids = groupComponents.Select(c => c.GetInstanceID()).ToArray();

                    // アイコンの有効状態の判定
                    bool isAnyEnabled = groupComponents.Any(c => GetEnabledState(c));

                    Component primaryComp = groupComponents[0];

                    //トグル可否の決定
                    Type firstType = primaryComp.GetType();
                    bool isSameTypeAll = groupComponents.All(c => c.GetType() == firstType);

                    // 全部同じ型で、そのコンポーネントがトグル可能だったらtrue
                    bool canGroupToggle = isSameTypeAll && CanToggleComponent(primaryComp);

                    ComponentIconInfo groupIcon = new ComponentIconInfo
                    {
                        InstanceIDs = ids,
                        Icon = def.GetTexture() ?? AssetPreview.GetMiniThumbnail(primaryComp),
                        IsEnabled = isAnyEnabled,
                        CanToggle = canGroupToggle,
                        IsMissing = false
                    };

                    finalIcons.Add(groupIcon);

                    foreach (var c in groupComponents) processedComponents.Add(c);
                }
            }

            //========================================================
            // グルーピングされないコンポーネントの処理
            foreach (var c in rawComponents)
            {
                if (c is Transform || (c != null && processedComponents.Contains(c))) continue;

                ComponentIconInfo info = new ComponentIconInfo();

                if (c == null || !c)
                {
                    info.IsMissing = true;
                    info.CanToggle = false;
                }
                else
                {
                    info.IsMissing = false;
                    info.InstanceIDs = new int[] { c.GetInstanceID() };
                    info.Icon = AssetPreview.GetMiniThumbnail(c);
                    info.IsEnabled = GetEnabledState(c);
                    info.CanToggle = CanToggleComponent(c);
                }

                finalIcons.Add(info);
            }

            return finalIcons.ToArray();
        }
    }
}