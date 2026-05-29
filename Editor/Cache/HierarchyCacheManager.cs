
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.Core;
using VRC.SDK3.Avatars.Components;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static partial class HierarchyCacheManager
    {
        public static Dictionary<int, CacheData> ItemCaches = new Dictionary<int, CacheData>();

        static HierarchyCacheManager()
        {
            EditorApplication.hierarchyChanged += CacheHierarchyObjectData;

            CacheHierarchyObjectData();
        }

        private static void CacheHierarchyObjectData()
        {
            ItemCaches.Clear();

            var descriptors = UnityEngine.Object.FindObjectsByType<VRCAvatarDescriptor>(FindObjectsSortMode.None);

            foreach (var desc in descriptors)
            {
                Transform rootXform = desc.transform;
                int rootId = rootXform.gameObject.GetInstanceID();

                List<bool> currentLineStates = new List<bool>();

                BuildTreeCacheRecursive(rootXform, rootId, 0, false, currentLineStates);
            }
        }

        private static void BuildTreeCacheRecursive(Transform current, int rootId, int depth, bool isLastChild, List<bool> lineStates)
        {
            int currentId = current.gameObject.GetInstanceID();

            // コンポーネント収集
            var components = AHFUtil.GetFilteredComponents(current.gameObject);

            ComponentIconInfo[] icons = ConvertToIconInfos(components);

            int childCount = current.childCount;
            bool hasChildren = (childCount > 0);

            bool[] flags = new bool[depth];
            for (int i = 0; i < depth; i++)
            {
                flags[i] = lineStates[i];
            }

            ItemCaches[currentId] = new CacheData(
                rootId,
                depth,
                isLastChild,
                flags,
                hasChildren,
                icons
            );

            for (int i = 0; i < childCount; i++)
            {
                Transform child = current.GetChild(i);

                bool childIsLast = (i == childCount - 1);

                lineStates.Add(!childIsLast);

                BuildTreeCacheRecursive(child, rootId, depth + 1, childIsLast, lineStates);

                lineStates.RemoveAt(lineStates.Count - 1);
            }
        }

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
                    // アイコンの有効状態の判定 && アイコンにどのコンポーネントが含まれてるかIDを収集
                    bool isAnyEnabled = false;
                    List<int> ids = new List<int>();

                    foreach (var c in groupComponents)
                    {
                        ids.Add(c.GetInstanceID());

                        if (c is Behaviour b && b.enabled) isAnyEnabled = true;
                        else if (c is Collider col && col.enabled) isAnyEnabled = true;
                        else if (c is Renderer r && r.enabled) isAnyEnabled = true;
                    }

                    Component primaryComp = groupComponents[0];

                    ComponentIconInfo groupIcon = new ComponentIconInfo
                    {
                        InstanceIDs = ids.ToArray(),
                        Icon = def.GetTexture() ?? AssetPreview.GetMiniThumbnail(primaryComp),
                        IsEnabled = isAnyEnabled,
                        CanToggle = true,
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

        /// <summary>
        /// 特定のオブジェクトのコンポーネント状態だけをその場で再解析して、キャッシュのアイコン情報のみを更新する
        /// </summary>
        public static void UpdateSingleObjectCacheDirect(int gameObjectInstanceID, Component[] rawComponents)
        {
            if (ItemCaches.TryGetValue(gameObjectInstanceID, out var cacheData))
            {
                cacheData.ComponentIcons = ConvertToIconInfos(rawComponents);

                ItemCaches[gameObjectInstanceID] = cacheData;

                EditorApplication.RepaintHierarchyWindow();
            }
        }

        /// <summary>
        /// コンポーネントが有効・無効（enabled）トグル可能かどうかを判定
        /// </summary>
        public static bool CanToggleComponent(Component comp)
        {
            if (comp == null) return false;
            return comp is Behaviour || comp is Collider || comp is Renderer;
        }

        /// <summary>
        /// コンポーネントの現在の有効・無効（enabled）状態を取得
        /// </summary>
        public static bool GetEnabledState(Component comp)
        {
            if (comp == null) return true;

            return comp switch
            {
                Behaviour b => b.enabled,
                Collider col => col.enabled,
                Renderer r => r.enabled,
                _ => true
            };
        }
    }
}
