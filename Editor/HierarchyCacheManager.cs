
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

        private static ComponentIconInfo getComponentIconInfo(Component c)
        {
            ComponentIconInfo info = new ComponentIconInfo();

            if (c == null)
            {
                info.IsMissing = true;
                info.CanToggle = false;
            }
            else
            {
                info.IsMissing = false;
                info.InstanceID = c.GetInstanceID();

                info.Icon = AssetPreview.GetMiniThumbnail(c);

                switch (c)
                {
                    case Renderer r: info.IsEnabled = r.enabled; info.CanToggle = true; break;
                    case Behaviour b: info.IsEnabled = b.enabled; info.CanToggle = true; break;
                    case Collider col: info.IsEnabled = col.enabled; info.CanToggle = true; break;
                    default: info.IsEnabled = true; info.CanToggle = false; break;
                }
            }

            return info;
        }

        /// <summary>
        /// 特定のオブジェクトの特定のコンポーネントの有効フラグだけをキャッシュ上で書き換え、即座に再描画する
        /// </summary>
        public static void UpdateSingleComponentCache(int gameObjectInstanceID, int componentInstanceID, bool newEnabledState)
        {
            if (ItemCaches.TryGetValue(gameObjectInstanceID, out var cacheData))
            {
                for (int i = 0; i < cacheData.ComponentIcons.Length; i++)
                {
                    if (cacheData.ComponentIcons[i].InstanceID == componentInstanceID)
                    {
                        cacheData.ComponentIcons[i].IsEnabled = newEnabledState;
                        break;
                    }
                }

                ItemCaches[gameObjectInstanceID] = cacheData;

                EditorApplication.RepaintHierarchyWindow();
            }
        }

        /// <summary>
        /// GameObjectから取得した生のコンポーネント配列を、描画用のクリーンなアイコン情報リストに変換・集約する
        /// </summary>
        public static ComponentIconInfo[] ConvertToIconInfos(Component[] rawComponents)
        {
            if (rawComponents == null || rawComponents.Length == 0)
                return Array.Empty<ComponentIconInfo>();

            List<ComponentIconInfo> finalIcons = new List<ComponentIconInfo>();
            HashSet<Component> processedComponents = new HashSet<Component>();

            // ---------------------------------------------------------
            // 処理1: グループ化（まとめ）ルールの適用
            // ---------------------------------------------------------
            foreach (var def in IconDefinitions)
            {
                List<Component> matchedComponents = new List<Component>();

                foreach (var comp in rawComponents)
                {
                    if (comp == null || !comp || processedComponents.Contains(comp) || comp is Transform) continue;

                    Type compType = comp.GetType();
                    if (compType == null) continue;

                    string className = compType.Name;
                    // フルネーム（例: nadena.dev.modular_avatar.runtime.MAMergeAnimator）を取得
                    string fullTypeName = compType.FullName ?? "";

                    bool isMatch = false;

                    // A. MAの判定：クラス名かフルネーム（名前空間）にMA関連のキーワードが入っているか
                    if (!string.IsNullOrEmpty(def.ClassNamePrefix) && def.GroupName == "ModularAvatar")
                    {
                        // 頭に「MA」が付くか、または名前空間に「nadena」か「ModularAvatar」が含まれていればMAの仲間とみなす！
                        if (className.StartsWith(def.ClassNamePrefix, StringComparison.Ordinal) ||
                            fullTypeName.Contains("nadena") ||
                            fullTypeName.Contains("ModularAvatar"))
                        {
                            isMatch = true;
                        }
                    }
                    // B. 通常の接頭辞（Prefix）判定
                    else if (!string.IsNullOrEmpty(def.ClassNamePrefix))
                    {
                        if (className.StartsWith(def.ClassNamePrefix, StringComparison.Ordinal))
                        {
                            isMatch = true;
                        }
                    }
                    // C. 従来の特定の型リスト判定（コライダー等）
                    else if (def.TargetTypes != null)
                    {
                        if (def.TargetTypes.Exists(t => t != null && (t == compType || compType.IsSubclassOf(t))))
                        {
                            isMatch = true;
                        }
                    }

                    if (isMatch)
                    {
                        matchedComponents.Add(comp);
                    }
                }

                // マッチしたコンポーネント群を1つのグループアイコンに集約
                if (matchedComponents.Count > 0)
                {
                    bool isAnyEnabled = false;
                    List<int> ids = new List<int>();

                    foreach (var c in matchedComponents)
                    {
                        // ここでも念のためオブジェクトの生存をチェック
                        if (c == null || !c) continue;

                        ids.Add(c.GetInstanceID());

                        if (c is Behaviour b && b.enabled) isAnyEnabled = true;
                        else if (c is Collider col && col.enabled) isAnyEnabled = true;
                        else if (c is Renderer r && r.enabled) isAnyEnabled = true;
                    }

                    // 万が一中身が全てお亡くなりになっていた場合の防衛
                    if (ids.Count == 0) continue;

                    Component primaryComp = matchedComponents[0];

                    ComponentIconInfo groupIcon = new ComponentIconInfo
                    {
                        InstanceID = primaryComp.GetInstanceID(),
                        MultiInstanceIDs = ids.ToArray(),
                        Icon = def.GetTexture() ?? AssetPreview.GetMiniThumbnail(primaryComp),
                        IsEnabled = isAnyEnabled,
                        CanToggle = true,
                        IsMissing = false
                    };

                    finalIcons.Add(groupIcon);

                    foreach (var c in matchedComponents) processedComponents.Add(c);
                }
            }

            // ---------------------------------------------------------
            // 処理2: 残りのコンポーネントの通常処理（前回のままでOKですが念のため再掲）
            // ---------------------------------------------------------
            foreach (var c in rawComponents)
            {
                if (c is Transform || (c != null && processedComponents.Contains(c))) continue;

                ComponentIconInfo info = new ComponentIconInfo();

                // 完全に null、または Missing スクリプトの場合
                if (c == null || !c)
                {
                    info.IsMissing = true;
                    info.CanToggle = false;
                }
                else
                {
                    info.IsMissing = false;
                    info.InstanceID = c.GetInstanceID();
                    info.MultiInstanceIDs = new int[] { c.GetInstanceID() };
                    info.Icon = AssetPreview.GetMiniThumbnail(c);

                    switch (c)
                    {
                        case Renderer r: info.IsEnabled = r.enabled; info.CanToggle = true; break;
                        case Behaviour b: info.IsEnabled = b.enabled; info.CanToggle = true; break;
                        case Collider col: info.IsEnabled = col.enabled; info.CanToggle = true; break;
                        default: info.IsEnabled = true; info.CanToggle = false; break;
                    }
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
                // 前回作った変換関数をそのまま使い、最新のコンポーネント状態（enabled）を反映したアイコン配列を作る
                cacheData.ComponentIcons = ConvertToIconInfos(rawComponents);

                // 辞書に書き戻す
                ItemCaches[gameObjectInstanceID] = cacheData;

                // ヒエラルキーの見た目を即座に更新
                EditorApplication.RepaintHierarchyWindow();
            }
        }
    }
}
