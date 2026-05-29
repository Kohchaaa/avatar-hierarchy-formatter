using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.SDKBase;
using System;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static partial class HierarchyCacheManager
    {
        private static readonly List<IconGroup> IconGroup = new List<IconGroup>
        {
            // Unity
            new IconGroup
            {
                GroupName = "Colliders",
                TargetTypes = new List<Type> { typeof(BoxCollider), typeof(SphereCollider), typeof(CapsuleCollider), typeof(MeshCollider) },
                IsCustomTexture = false,
            },

            // VRC
            new IconGroup
            {
                GroupName = "AvatarDescriptor",
                TargetTypes = new List<Type> { typeof(VRC_AvatarDescriptor) },
                IconGUID = "9214e184413843044bce548e02774e35",
                IsCustomTexture = true,
            },
            new IconGroup
            {
                GroupName = "PhysBone",
                TargetTypes = new List<Type> { typeof(VRCPhysBone) },
                IconGUID = "78013448b2cd2b949b5ba0e7118d7ab0",
                IsCustomTexture = true,
            },
            new IconGroup
            {
                GroupName = "PhysBoneCollider",
                TargetTypes = new List<Type> { typeof(VRCPhysBoneCollider) },
                IconGUID = "cc70b3e0ffd1f414c84561a0dc2f0b50",
                IsCustomTexture = true,
            },

            // Package
            new IconGroup
            {
                GroupName = "ModularAvatar",
                IsCustomTexture = false,
                GudgeIsInclude = c => 
                    IsContainNamespace(c, "modular_avatar") ||
                    IsContainNamespace(c, "MA") || 
                    IsContainNamespace(c, "ModularAvatar")
            },
            new IconGroup
            {
                GroupName = "AvatarOptimizer",
                IconGUID = "7caa3db0ca3a1de4c9a56c6a55b8fc42",
                IsCustomTexture = true,
                GudgeIsInclude = (c) => 
                    IsContainNamespace(c, "AvatarOptimizer") ||
                    IsContainNamespace(c, "AAO")
            },
        };

        /// <summary>
        /// コンポーネントが、キーワードを含む名前空間に属しているかチェック
        /// </summary>
        /// <param name="c">調べるコンポーネント</param>
        /// <param name="keyword">キーワード</param>
        /// <returns>含んでいるかの真偽値</returns>
        public static bool IsContainNamespace(Component c, string keyword)
        {
            if (c == null) return false;

            Type type = c.GetType();
            string ns = type.Namespace ?? "";

            return ns.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// コンポーネントが、キーワードを含むクラス名かどうかチェック
        /// </summary>
        /// <param name="c"></param>
        /// <param name="keyword"></param>
        /// <returns>含んでいるかの真偽値</returns>
        public static bool IsContainClassName(Component c, string keyword)
        {
            if (c == null) return false;

            Type type = c.GetType();
            string cn = type.Name;

            return cn.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }
    }
}