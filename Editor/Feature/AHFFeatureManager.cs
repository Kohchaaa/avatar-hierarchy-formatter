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
            Features.Add(new AvatarHighlight());
            Features.Add(new TreeLine());

            Features.Add(new ToggleActive());
            Features.Add(new ComponentIcon());


            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowGUI;
        }

        private static void OnHierarchyWindowGUI(int instanceID, Rect selectionRect)
        {
            if (!GeneralSettingModule.IsEnabled_Plugin) return;

            var context = new AHFLayoutContext(instanceID, selectionRect, 16f);

            foreach (var feature in Features)
            {
                if (feature.IsEnabled)
                {
                    feature.OnGUI(context);
                }
            }
        }
    }
}