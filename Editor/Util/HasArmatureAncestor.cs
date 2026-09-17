using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static partial class AHFUtil
    {
        /// <summary>
        /// 祖先のいずれかがArmature名にマッチするかどうか
        /// </summary>
        public static bool HasArmatureAncestor(Transform t)
        {
            for (var parent = t.parent; parent != null; parent = parent.parent)
            {
                if (ArmatureNameIconJudge.IsArmatureName(parent.name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
