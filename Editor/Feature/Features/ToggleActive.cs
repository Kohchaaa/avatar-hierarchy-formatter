
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public class ToggleActive : IAHFFeature
    {
        public string FeatureName => "ToggleActive";

        public bool IsEnabled { get; }

        public void OnGUI(int instanceID, Rect selectionRect)
        {
            
        }
    }
}