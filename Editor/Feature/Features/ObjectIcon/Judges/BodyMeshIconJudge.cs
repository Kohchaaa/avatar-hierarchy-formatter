using System.Text.RegularExpressions;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class BodyMeshIconJudge : IAHFObjectIconJudge
    {
        public string JudgeName => "BodyMesh";

        private static readonly AHFIconId BodyIconId = new AHFIconId("body");
        private static readonly AHFIconId BodyBaseIconId = new AHFIconId("body_base");

        // VRCアバターの慣例: 頭のメッシュは"Body"、体のメッシュは"BodyBase"
        // 「キャラ名_Body_Base」のように接頭辞が付くことが多いため、末尾一致で判定する。
        // "_base"に限らず"_b"や"_2"等、bodyの後に区切り文字+何かが続くケース全般を体として扱う
        // (区切り文字を必須にすることで"Bodysuit"のような偶然の一致は除外する)
        private static readonly Regex BodyBasePattern = new Regex(
            @"(^|[-_ ])body[-_ ].+$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        private static readonly Regex BodyPattern = new Regex(
            @"(^|[-_ ])body(\.\d+)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            if (BodyBasePattern.IsMatch(go.name))
            {
                iconId = BodyBaseIconId;
                return true;
            }

            if (BodyPattern.IsMatch(go.name))
            {
                iconId = BodyIconId;
                return true;
            }

            iconId = default;
            return false;
        }
    }
}
