using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class AHFIconEntry
    {
        public AHFIconId Id;
        public string DisplayName;
        public string IconGUID;
        public string IconPathOrName;
        private Texture2D _cachedTexture;

        public Texture2D GetTexture()
        {
            if (_cachedTexture != null) return _cachedTexture;

            if (!string.IsNullOrEmpty(IconGUID))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(IconGUID);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    _cachedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                }
            }
            else if (!string.IsNullOrEmpty(IconPathOrName))
            {
                var content = EditorGUIUtility.IconContent(IconPathOrName);
                _cachedTexture = content != null ? content.image as Texture2D : null;
            }

            return _cachedTexture;
        }
    }
}
