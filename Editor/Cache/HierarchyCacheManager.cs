
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
