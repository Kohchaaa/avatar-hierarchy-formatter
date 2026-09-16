using System.Text.RegularExpressions;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class ArmatureNameIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "ArmatureName";

        private static readonly AHFIconId IconId = new AHFIconId("armature");

        private static readonly Regex NamePattern = new Regex(
            @"^(armature|アーマチュア)(\.\d+)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (NamePattern.IsMatch(go.name))
            {
                iconId = IconId;
                return true;
            }

            iconId = default;
            return false;
        }
    }
}
