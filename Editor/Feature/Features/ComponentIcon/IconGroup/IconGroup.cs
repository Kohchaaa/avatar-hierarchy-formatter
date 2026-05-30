
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class IconGroup
    {
        public string GroupName;
        public List<Type> TargetTypes;
        public string IconPathOrName;
        public string IconGUID;
        public bool IsCustomTexture;
        private Texture2D _cachedTexture;

        public Func<Component, bool> GudgeIsInclude;

        public bool IsMatch(Component comp)
        {
            if (comp == null || !comp) return false;

            if (GudgeIsInclude != null)
                return GudgeIsInclude(comp);

            if (TargetTypes != null)
            {
                Type compType = comp.GetType();
                return TargetTypes.Exists(t => t != null && (t == compType || compType.IsSubclassOf(t)));
            }

            return false;
        }

        public Texture2D GetTexture()
        {
            if (_cachedTexture != null) return _cachedTexture;

            if (IsCustomTexture && !string.IsNullOrEmpty(IconGUID))
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


        public Texture2D GetIconTexture(Component primaryComp)
        {
            if (ComponentIconSettingModule.IsAllowCustomIcon)
            {
                Texture2D customTex = GetTexture();
                if (customTex != null) return customTex;
            }

            return AssetPreview.GetMiniThumbnail(primaryComp);
        }
    }
}