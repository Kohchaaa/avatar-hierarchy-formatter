using System.Linq;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class OutfitIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "Outfit";

        private static readonly AHFIconId IconId = new AHFIconId("outfit");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (HasMergeArmature(go.transform, components))
            {
                iconId = IconId;
                return true;
            }

            iconId = default;
            return false;
        }

        private static bool HasMergeArmature(Transform t, Component[] ownComponents)
        {
            if (ownComponents.Any(IsMergeArmature)) return true;

            for (int i = 0; i < t.childCount; i++)
            {
                if (t.GetChild(i).GetComponents<Component>().Any(IsMergeArmature))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsMergeArmature(Component c) =>
            HierarchyCacheManager.IsContainNamespace(c, "modular_avatar") &&
            HierarchyCacheManager.IsContainClassName(c, "MergeArmature");
    }
}
