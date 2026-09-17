using System.Linq;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class LightIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "Light";

        private static readonly AHFIconId IconId = new AHFIconId("light");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (components.Any(c => c is Light))
            {
                iconId = IconId;
                return true;
            }

            iconId = default;
            return false;
        }
    }
}
