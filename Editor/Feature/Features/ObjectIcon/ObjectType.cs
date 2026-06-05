namespace Kohcha.AvatarHierarchyFormatter
{
    public enum AHFObjectType : byte
    {
        Default = 0,

        AvatarRoot,
        Armature,
        Hips,
        Head,

        BodyMesh,
        Outfit,
        Gimmick,

        VRCSystem,
        Light,
        Audio,

        GestureManager,
        FaceEmo
    }
}

// TODO:後で拡張メソッドかなんかでテクスチャ取得できる機能をAHFObjectTypeに付ける