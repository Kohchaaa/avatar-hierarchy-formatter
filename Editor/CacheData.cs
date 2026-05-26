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