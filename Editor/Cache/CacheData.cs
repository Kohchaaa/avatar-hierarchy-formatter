using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public struct CacheData
    {
        // アバター外のオブジェクトはnull
        public int? AvatarRootId;

        public AHFObjectScope Scope => ScopeOf(AvatarRootId);

        // キャッシュ構築中（CacheDataを作る前）にJudgeへ渡す時も、この1か所で導出する
        public static AHFObjectScope ScopeOf(int? avatarRootId) =>
            avatarRootId.HasValue ? AHFObjectScope.InAvatar : AHFObjectScope.OutsideAvatar;

        public int IndentLevel;
        public bool IsLastChild;

        public bool[] ParentLineFlags;

        public bool HasChildren;

        public ComponentIconInfo[] ComponentIcons;

        public AHFIconId? ObjectIconId;
        public AHFIconId? OverrideIconId;

        public CacheData(int? id, int indentLevel, bool isLastChild, bool[] flags, bool hasChildren, ComponentIconInfo[] componentIcons, AHFIconId? objectIconId, AHFIconId? overrideIconId)
        {
            AvatarRootId = id;
            IndentLevel = indentLevel;
            IsLastChild = isLastChild;
            ParentLineFlags = flags;
            HasChildren = hasChildren;
            ComponentIcons = componentIcons;
            ObjectIconId = objectIconId;
            OverrideIconId = overrideIconId;
        }
    }

    public struct ComponentIconInfo
    {
        public Texture2D Icon;
        public bool IsEnabled;
        public bool CanToggle;
        public bool IsMissing;
        //public int InstanceID;
        public int[] InstanceIDs;
    }
}


