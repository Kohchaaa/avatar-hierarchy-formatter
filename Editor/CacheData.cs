using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public struct CacheData
    {
        public int AvatarRootId;

        public int IndentLevel;
        public bool IsLastChild;

        public bool[] ParentLineFlags;

        public bool HasChildren;

        public CacheData(int id, int indentLevel, bool isLastChild, bool[] flags, bool hasChildren)
        {
            AvatarRootId = id;
            IndentLevel = indentLevel;
            IsLastChild = isLastChild;
            ParentLineFlags = flags;
            HasChildren = hasChildren;
        }
    }

    public struct ComponentIconInfo
    {
        public Texture2D Icon;
        public bool IsEnabled;
        public bool CanToggle;
        public bool IsMissing;
        public int InstanceID;
    }
}


