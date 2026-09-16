using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class ArmatureIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "Armature";

        private static readonly AHFIconId IconId = new AHFIconId("armature");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            var animator = go.GetComponentInParent<Animator>(true);
            if (animator != null && animator.avatar != null && animator.avatar.isHuman)
            {
                var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
                if (hips != null && hips.parent == go.transform)
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
