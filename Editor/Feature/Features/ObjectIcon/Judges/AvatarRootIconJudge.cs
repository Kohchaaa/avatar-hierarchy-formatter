using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class AvatarRootIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "AvatarRoot";

        private static readonly AHFIconId IconId = new AHFIconId("avatar_root");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (components.Any(c => c is VRCAvatarDescriptor))
            {
                iconId = IconId;
                return true;
            }

            iconId = default;
            return false;
        }
    }
}
