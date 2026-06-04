
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

        public static void CacheHierarchyObjectData()
        {
            ItemCaches.Clear();

            var descriptors = UnityEngine.Object.FindObjectsByType<VRCAvatarDescriptor>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (var desc in descriptors)
            {
                Transform rootXform = desc.transform;
                int rootId = rootXform.gameObject.GetInstanceID();

                List<bool> currentLineStates = new List<bool>();

                BuildTreeCache(rootXform, rootId, 0, false, currentLineStates);
            }
        }

        private static void BuildTreeCache(Transform current, int rootId, int depth, bool isLastChild, List<bool> lineStates)
        {
            int currentId = current.gameObject.GetInstanceID();

            // コンポーネント収集
            var components = AHFUtil.GetFilteredComponents(current.gameObject);

            ComponentIconInfo[] icons = ConvertToIconInfo(components);

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

                BuildTreeCache(child, rootId, depth + 1, childIsLast, lineStates);

                lineStates.RemoveAt(lineStates.Count - 1);
            }
        }
    }
}
