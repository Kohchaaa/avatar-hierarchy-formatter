using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public interface IAHFObjectIconJudge
    {
        string JudgeName { get; }

        // 判定する対象。アバター外では誤判定しやすいもの（名前・Animator由来）はInAvatarに絞る
        AHFObjectScope Scope { get; }

        bool TryJudge(GameObject go, Component[] components, out AHFIconId iconId);
    }
}
