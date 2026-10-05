using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class AHFIconOverrideManager
    {
        private const string FilePath = "ProjectSettings/Packages/jp.kohchaaa.avatar-hierarchy-formatter/IconOverrides.json";

        private static AHFIconOverrideData _data;

        /// <summary>
        /// 直近の読み書きが失敗した理由。成功していればnull。設定画面の警告表示に使う
        /// </summary>
        public static string LastError { get; private set; }

        static AHFIconOverrideManager()
        {
            Load();
        }

        // 静的コンストラクタから呼ばれるため、ここで例外を逃がすと型が失敗状態でキャッシュされ、
        // 以降この型へのアクセスがすべてTypeInitializationExceptionになる。
        // TryGetOverrideはキャッシュ構築の中で全オブジェクト分呼ばれるので、そうなると
        // アイコンに限らずHierarchyの描画が丸ごと止まる。必ず握って空データで続行する。
        private static void Load()
        {
            _data = new AHFIconOverrideData();
            LastError = null;

            if (!File.Exists(FilePath)) return;

            try
            {
                string json = File.ReadAllText(FilePath);
                JsonUtility.FromJsonOverwrite(json, _data);
            }
            catch (System.Exception e)
            {
                // 部分的に読み込まれている可能性があるので作り直す
                _data = new AHFIconOverrideData();
                LastError = $"アイコンの上書き設定を読み込めませんでした: {e.Message}";
                Debug.LogWarning($"[AHF] {LastError}\n{FilePath}");
            }
        }

        // 一時ファイルへ書いてから差し替えることで、書き込みが中断しても
        // 既存のファイルが壊れないようにする（成功か失敗かのどちらかに倒す）
        private static bool Save()
        {
            LastError = null;

            string tempPath = FilePath + ".tmp";

            try
            {
                string directory = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(tempPath, json);

                if (File.Exists(FilePath))
                {
                    File.Replace(tempPath, FilePath, null);
                }
                else
                {
                    File.Move(tempPath, FilePath);
                }

                return true;
            }
            catch (System.Exception e)
            {
                LastError = $"アイコンの上書き設定を保存できませんでした: {e.Message}";
                Debug.LogWarning($"[AHF] {LastError}\n{FilePath}");

                try
                {
                    if (File.Exists(tempPath)) File.Delete(tempPath);
                }
                catch (System.Exception)
                {
                    // 一時ファイルを消せなくても次回の書き込みで上書きされるので、ここは無視してよい
                }

                return false;
            }
        }

        private static string GetGid(int instanceId) => GlobalObjectId.GetGlobalObjectIdSlow(instanceId).ToString();

        public static bool TryGetOverride(int instanceId, out AHFIconId iconId)
        {
            string id = GetGid(instanceId);
            var entry = _data.Entries.FirstOrDefault(e => e.GlobalObjectId == id);

            if (entry != null)
            {
                iconId = new AHFIconId(entry.IconId);
                return true;
            }

            iconId = default;
            return false;
        }

        public static void SetOverride(int instanceId, AHFIconId iconId)
        {
            string id = GetGid(instanceId);

            var entry = _data.Entries.FirstOrDefault(e => e.GlobalObjectId == id);
            if (entry != null)
            {
                entry.IconId = iconId.ToString();
            }
            else
            {
                _data.Entries.Add(new AHFIconOverrideEntry { GlobalObjectId = id, IconId = iconId.ToString() });
            }

            Save();
        }

        public static int Count => _data.Entries.Count;

        public static int CountForAssetGuids(ICollection<string> assetGuids) =>
            _data.Entries.Count(e => MatchesAssetGuid(e, assetGuids));

        public static int ClearForAssetGuids(ICollection<string> assetGuids)
        {
            int removed = _data.Entries.RemoveAll(e => MatchesAssetGuid(e, assetGuids));
            if (removed > 0) Save();

            return removed;
        }

        public static int ClearAll()
        {
            int removed = _data.Entries.Count;
            _data.Entries.Clear();
            if (removed > 0) Save();

            return removed;
        }

        // 保存しているIDから、それがどのシーン/PrefabのオブジェクトかをassetGUIDで判別する。
        // オブジェクトを解決できるかで判定すると、開いていないシーンのものも「解決できない」に
        // なるため、消してはいけない分まで対象に入ってしまう。
        private static bool MatchesAssetGuid(AHFIconOverrideEntry entry, ICollection<string> assetGuids) =>
            GlobalObjectId.TryParse(entry.GlobalObjectId, out var globalObjectId)
            && assetGuids.Contains(globalObjectId.assetGUID.ToString());

        public static void RemoveOverride(int instanceId)
        {
            string id = GetGid(instanceId);

            _data.Entries.RemoveAll(e => e.GlobalObjectId == id);

            Save();
        }
    }
}
