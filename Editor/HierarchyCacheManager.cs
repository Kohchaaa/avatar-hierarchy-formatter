
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class HierarchyCacheManager
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
                int rootID = desc.gameObject.GetInstanceID();

                Transform[] allChildren = desc.GetComponentsInChildren<Transform>(true);

                foreach (Transform child in allChildren)
                {
                    int childId = child.gameObject.GetInstanceID();
                    ItemCaches[childId] = new CacheData(rootID);
                }
            }
        }
    }
}
