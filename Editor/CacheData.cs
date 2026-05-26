public struct CacheData
{
    public int AvatarRootId;
    
    public int IndentLevel;
    public bool IsLastChild;

    public bool[] ParentLineFlags;

    public CacheData(int id, int indentLevel, bool isLastChild, bool[] flags)
    {
        AvatarRootId = id;
        IndentLevel = indentLevel;
        IsLastChild = isLastChild;
        ParentLineFlags = flags;
    }
}