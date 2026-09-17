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
            Judges.Add(new AvatarRootIconJudge());
            Judges.Add(new ArmatureIconJudge());
            Judges.Add(new ArmatureNameIconJudge());
            Judges.Add(new HipsIconJudge());
            Judges.Add(new HipsNameIconJudge());
            Judges.Add(new HeadIconJudge());
            Judges.Add(new HeadNameIconJudge());
            Judges.Add(new LightIconJudge());
            Judges.Add(new AudioIconJudge());
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
