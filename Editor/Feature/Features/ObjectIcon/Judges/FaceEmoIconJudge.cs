using System.Linq;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class FaceEmoIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "FaceEmo";

        private static readonly AHFIconId IconId = new AHFIconId("face_emo");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (components.Any(c => HierarchyCacheManager.IsContainNamespace(c, "FaceEmo")))
            {
                iconId = IconId;
                return true;
            }

            iconId = default;
            return false;
        }
    }
}
