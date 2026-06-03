using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public interface IAHFFeature
    {
        string FeatureName { get; }

        bool IsEnabled { get; }

        void OnGUI(AHFLayoutContext context);
    }
}