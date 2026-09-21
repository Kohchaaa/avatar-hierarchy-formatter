using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public interface IAHFObjectIconJudge
    {
        string JudgeName { get; }

        bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId);
    }
}
