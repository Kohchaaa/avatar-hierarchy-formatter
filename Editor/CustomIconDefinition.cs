using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class CustomIconDefinition
    {
        public string GroupName;
        public List<Type> TargetTypes;
        public string ClassNamePrefix;
        public string IconPathOrName;
        public string IconGUID;
        public bool IsCustomTexture;
        private Texture2D _cachedTexture;

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
    }
    public static partial class HierarchyCacheManager
    {
        private static readonly List<CustomIconDefinition> IconDefinitions = new List<CustomIconDefinition>
            {
                new CustomIconDefinition
                {
                    GroupName = "ModularAvatar",
                    ClassNamePrefix = "MA",
                    IsCustomTexture = false
                },
                new CustomIconDefinition
                {
                    GroupName = "Colliders",
                    TargetTypes = new List<Type> { typeof(BoxCollider), typeof(SphereCollider), typeof(CapsuleCollider), typeof(MeshCollider) },
                    IsCustomTexture = false
                },

                new CustomIconDefinition
                {
                    GroupName = "PhysBone",
                    TargetTypes = new List<Type> { typeof(VRCPhysBone) },
                    IconGUID = "78013448b2cd2b949b5ba0e7118d7ab0",
                    IsCustomTexture = true
                }
            };
    }
}