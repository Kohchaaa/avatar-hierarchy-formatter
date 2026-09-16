using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class AHFObjectIconJudgeManager
    {
        public static readonly List<IAHFObjectIconJudge> Judges = new List<IAHFObjectIconJudge>();

        static AHFObjectIconJudgeManager()
        {
            // 個別のJudgeはここに追加していく
        }

        public static bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId)
        {
            foreach (var judge in Judges)
            {
                if (judge.TryJudge(go, components, out iconId))
                {
                    return true;
                }
            }

            iconId = default;
            return false;
        }
    }
}
