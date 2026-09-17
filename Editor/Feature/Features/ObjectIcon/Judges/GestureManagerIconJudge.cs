using System.Linq;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class GestureManagerIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "GestureManager";

        private static readonly AHFIconId IconId = new AHFIconId("gesture_manager");

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (components.Any(c => HierarchyCacheManager.IsContainNamespace(c, "GestureManager")))
            {
                iconId = IconId;
                return true;
            }

            iconId = default;
            return false;
        }
    }
}
