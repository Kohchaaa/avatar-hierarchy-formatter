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
            //追加していってね

            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowGUI;
        }

        private static void OnHierarchyWindowGUI(int instanceID, Rect selectionRect)
        {
            if (!GeneralSettingModule.IsEnabled_Plugin) return;

            foreach (var feature in Features)
            {
                if (feature.IsEnabled)
                {
                    feature.OnGUI(instanceID, selectionRect);
                }
            }
        }
    }
}