using System.Linq;
using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    internal class AHFUserIconPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            bool touched =
                importedAssets.Any(IsInIconFolder) ||
                deletedAssets.Any(IsInIconFolder) ||
                movedAssets.Any(IsInIconFolder) ||
                movedFromAssetPaths.Any(IsInIconFolder);

            if (!touched) return;

            AHFUserIconScanner.Rescan();
        }

        private static bool IsInIconFolder(string assetPath) =>
            assetPath.StartsWith(AHFUserIconFolder.Path + "/");
    }
}
