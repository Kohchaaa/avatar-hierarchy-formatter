
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

            var descriptors = Object.FindObjectsByType<VRCAvatarDescriptor>(FindObjectsSortMode.None);

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
            var components = current.gameObject.GetComponents<Component>()
                .Where(c => c is not Transform)
                .Where(c => c is not Animator)
                .Where(c => c is not PipelineManager)
                .ToArray();
            List<ComponentIconInfo> iconList = new List<ComponentIconInfo>();

            foreach(var c in components)
            {
                iconList.Add(getComponentIconInfo(c));
            }


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
                iconList.ToArray()
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
                // Missingスクリプトの場合
                info.IsMissing = true;
                info.CanToggle = false;
            }
            else
            {
                info.IsMissing = false;
                info.InstanceID = c.GetInstanceID();

                // Unity標準のミニアイコンをバックエンド側で一発取得
                info.Icon = AssetPreview.GetMiniThumbnail(c);

                // トグル可能なコンポーネントかどうかの判定と有効状態の取得
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
    }    
}
