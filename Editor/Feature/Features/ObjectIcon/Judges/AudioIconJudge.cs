using System.Linq;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class AudioIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "Audio";

        private static readonly AHFIconId IconId = new AHFIconId("audio");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (components.Any(c => c is AudioSource))
            {
                iconId = IconId;
                return true;
            }

            iconId = default;
            return false;
        }
    }
}
