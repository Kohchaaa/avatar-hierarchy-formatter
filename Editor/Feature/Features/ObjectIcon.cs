using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public class ObjectIcon : IAHFFeature
    {
        public string FeatureName => "a";

        public bool IsEnabled => false;

        public void OnGUI(ref AHFLayoutContext context)
        {
            
        }
    }
}