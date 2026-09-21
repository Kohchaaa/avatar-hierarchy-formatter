using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class AHFUserIconScanner
    {
        // 組み込みのIDと衝突させないための接頭辞。':'はファイル名に使えないので、
        // ファイル名から作るIDと混ざる心配がない。
        private const string IdPrefix = "user:";

        private static readonly List<AHFIconId> _registeredIds = new List<AHFIconId>();

        static AHFUserIconScanner()
        {
            // InitializeOnLoadの時点ではAssetDatabaseが使えない場合があるため、1フレーム遅らせる
            EditorApplication.delayCall += Rescan;
        }

        public static void Rescan()
        {
            foreach (var id in _registeredIds)
            {
                AHFIconRegistry.Unregister(id);
            }
            _registeredIds.Clear();

            if (AHFUserIconFolder.Exists)
            {
                foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { AHFUserIconFolder.Path }))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (!IsDirectChild(assetPath)) continue;

                    // 拡張子まで含める。Windowsでは拡張子違いの同名ファイルが共存できるので、
                    // 拡張子を落とすとIDが衝突して片方がUIから消える。
                    string fileName = Path.GetFileName(assetPath);
                    var id = new AHFIconId(IdPrefix + fileName);

                    AHFIconRegistry.Register(new AHFIconEntry
                    {
                        Id = id,
                        DisplayName = fileName,
                        IconGUID = guid,
                        IsUserDefined = true,
                    });
                    _registeredIds.Add(id);
                }
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        // サブフォルダは対象外。IDをファイル名から決めるので、階層を許すと同名衝突が起きるため
        private static bool IsDirectChild(string assetPath)
        {
            string directory = Path.GetDirectoryName(assetPath);
            return directory != null && directory.Replace('\\', '/') == AHFUserIconFolder.Path;
        }
    }
}
