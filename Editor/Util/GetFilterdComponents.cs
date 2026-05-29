using System.Linq;
using UnityEngine;
using VRC.Core; // LINQを使うために必要

namespace Kohcha.AvatarHierarchyFormatter
{
    public static partial class AHFUtil
    {
        /// <summary>
        /// GameObjectから不要なコンポーネント（Transform, Animator, PipelineManager）を除外して取得する共通関数
        /// </summary>
        public static Component[] GetFilteredComponents(GameObject go)
        {
            if (go == null) return System.Array.Empty<Component>();

            return go.GetComponents<Component>()
                .Where(c => c is not Transform)
                .Where(c => c is not Animator)
                .Where(c => c is not PipelineManager)
                .ToArray();
        }
    }
}