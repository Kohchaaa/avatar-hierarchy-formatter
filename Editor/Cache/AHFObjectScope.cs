using System;

namespace Kohcha.AvatarHierarchyFormatter
{
    /// <summary>
    /// オブジェクトがアバターに属するかどうか。Judgeは対象にするものをマスクで指定する
    /// </summary>
    [Flags]
    public enum AHFObjectScope
    {
        None = 0,
        InAvatar = 1 << 0,
        OutsideAvatar = 1 << 1,
        All = InAvatar | OutsideAvatar,
    }
}
