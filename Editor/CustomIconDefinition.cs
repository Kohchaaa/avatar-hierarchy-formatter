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
        public string IconPathOrName;
        public bool IsCustomTexture;
        private Texture2D _cachedTexture;

        public Texture2D GetTexture()
        {
            if (_cachedTexture != null) return _cachedTexture;

            if (IsCustomTexture)
            {
                _cachedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPathOrName);
            }
            else
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
                GroupName = "Colliders",
                TargetTypes = new List<Type> { typeof(BoxCollider), typeof(SphereCollider), typeof(CapsuleCollider), typeof(MeshCollider) },
                IconPathOrName = "Assets/Kohcha/AvatarHierarchyFormatter/Icons/CustomColliderIcon.png",
                IsCustomTexture = true
            },

            new CustomIconDefinition
            {
                GroupName = "PhysBone",
                TargetTypes = new List<Type> { typeof(VRCPhysBone) },
                IconPathOrName = "d_PhysicsMaterial Icon", // Unity内部のカッコいいアイコン名を指定
                IsCustomTexture = false
            }
        };
    }
}