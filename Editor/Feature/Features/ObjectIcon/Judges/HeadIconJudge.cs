using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class HeadIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "Head";

        private static readonly AHFIconId IconId = new AHFIconId("head");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            var animator = go.GetComponentInParent<Animator>(true);
            if (animator != null && animator.avatar != null && animator.avatar.isHuman)
            {
                var head = animator.GetBoneTransform(HumanBodyBones.Head);
                if (head == go.transform)
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
