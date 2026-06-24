
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
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

            var processedRoots = new HashSet<Transform>();

            foreach (var desc in descriptors)
            {
                if (desc == null) continue;
                Transform rootXform = desc.transform;
                if (processedRoots.Contains(rootXform)) continue;

                processedRoots.Add(rootXform);
                int rootId = rootXform.gameObject.GetInstanceID();
                List<bool> currentLineStates = new List<bool>();
                BuildTreeCache(rootXform, rootId, 0, false, currentLineStates);
            }

            var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (currentStage != null && currentStage.prefabContentsRoot != null)
            {
                var prefabDescriptors = currentStage.prefabContentsRoot.GetComponentsInChildren<VRCAvatarDescriptor>(true);

                foreach (var desc in prefabDescriptors)
                {
                    if (desc == null) continue;
                    Transform rootXform = desc.transform;
                    if (processedRoots.Contains(rootXform)) continue;

                    processedRoots.Add(rootXform);
                    int rootId = rootXform.gameObject.GetInstanceID();
                    List<bool> currentLineStates = new List<bool>();
                    BuildTreeCache(rootXform, rootId, 0, false, currentLineStates);
                }
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
