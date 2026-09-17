using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class HipsIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "Hips";

        private static readonly AHFIconId IconId = new AHFIconId("hips");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            var animator = go.GetComponentInParent<Animator>(true);
            if (animator != null && animator.avatar != null && animator.avatar.isHuman)
            {
                var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
                if (hips == go.transform)
                {
                    iconId = IconId;
                    return true;
                }
            }

            iconId = default;
            return false;
        }
    }
}
