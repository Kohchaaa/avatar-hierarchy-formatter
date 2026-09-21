using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static class AHFUserIconFolder
    {
        private const string RootPath = "Assets/AvatarHierarchyFormatter";

        public const string Path = RootPath + "/Icons";

        public static bool Exists => AssetDatabase.IsValidFolder(Path);

        public static void Reveal()
        {
            // 起動時には作らない。この機能を使わないユーザーのAssetsに空フォルダを残さないため、
            // ユーザーが明示的にフォルダを開いた時だけ作る。
            if (!AssetDatabase.IsValidFolder(RootPath))
            {
                AssetDatabase.CreateFolder("Assets", "AvatarHierarchyFormatter");
            }

            if (!AssetDatabase.IsValidFolder(Path))
            {
                AssetDatabase.CreateFolder(RootPath, "Icons");
            }

            EditorUtility.RevealInFinder(Path);
        }
    }
}
