using System.Text.RegularExpressions;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class HipsNameIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "HipsName";

        private static readonly AHFIconId IconId = new AHFIconId("hips");

        private static readonly Regex NamePattern = new Regex(
            @"^(hips)(\.\d+)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (NamePattern.IsMatch(go.name) && AHFUtil.HasArmatureAncestor(go.transform))
            {
                iconId = IconId;
                return true;
            }

            iconId = default;
            return false;
        }
    }
}
