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
        public static ComponentIconInfo[] ConvertToIconInfo(Component[] rawComponents)
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
                    if (comp == null || !comp || comp is Transform) continue;

                    if (processedComponents.Contains(comp)) continue;

                    if (def.IsMatch(comp))
                    {
                        groupComponents.Add(comp);
                    }
                }

                // アイコン生成
                if (groupComponents.Count > 0)
                {
                    if (!ComponentIconSettingModule.IsAllowGrouping && groupComponents.Count > 1)
                    {
                        foreach (var comp in groupComponents)
                        {
                            ComponentIconInfo singleIcon = new ComponentIconInfo
                            {
                                InstanceIDs = new int[] { comp.GetInstanceID() },
                                Icon = def.GetIconTexture(comp),
                                IsEnabled = GetEnabledState(comp),
                                CanToggle = CanToggleComponent(comp),
                                IsMissing = false
                            };
                            finalIcons.Add(singleIcon);
                        }
                    }
                    else
                    {
                        int[] ids = groupComponents.Select(c => c.GetInstanceID()).ToArray();
                        bool isAnyEnabled = groupComponents.Any(c => GetEnabledState(c));
                        Component primaryComp = groupComponents[0];

                        Type firstType = primaryComp.GetType();
                        bool isSameTypeAll = groupComponents.All(c => c.GetType() == firstType);
                        bool canGroupToggle = isSameTypeAll && CanToggleComponent(primaryComp);

                        ComponentIconInfo groupIcon = new ComponentIconInfo
                        {
                            InstanceIDs = ids,
                            Icon = def.GetIconTexture(primaryComp),
                            IsEnabled = isAnyEnabled,
                            CanToggle = canGroupToggle,
                            IsMissing = false
                        };
                        finalIcons.Add(groupIcon);
                    }

                    foreach (var c in groupComponents)
                    {
                        processedComponents.Add(c);
                    }
                }
            }

            return finalIcons.ToArray();
        }
    }
}