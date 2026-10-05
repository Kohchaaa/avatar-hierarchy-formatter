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

        // 読み込みの失敗と保存の失敗は、ユーザーにとって全く違う状況（前者は保存済みの分が
        // 消えている、後者はこれからの分が残っていない）なので、1つの変数にまとめない。
        // まとめると、保存が成功した時に読み込み失敗の記録まで消えてしまう

        /// <summary>
        /// 保存されていた内容を読み込めなかった理由。読み込めていればnull
        /// </summary>
        public static string LoadError { get; private set; }

        /// <summary>
        /// 保存できなかった理由。保存できていればnull
        /// </summary>
        public static string SaveError { get; private set; }

        /// <summary>
        /// 保存に失敗して、メモリ上にしか存在しない変更があるかどうか
        /// </summary>
        public static bool HasUnsavedChanges => SaveError != null;

        static AHFIconOverrideManager()
        {
            Load();

            // メモリ上の変更はドメインリロード（スクリプト再コンパイル、Playモード入り）で
            // 消える。Load()が走り直してディスクの内容に戻るため、ユーザーが気づく機会が無い。
            // 消える直前がやり直す最後のタイミングなので、ここで書き戻しを試す
            AssemblyReloadEvents.beforeAssemblyReload -= FlushIfNeeded;
            AssemblyReloadEvents.beforeAssemblyReload += FlushIfNeeded;

            EditorApplication.quitting -= FlushIfNeeded;
            EditorApplication.quitting += FlushIfNeeded;
        }

        // 保存が失敗したまま変更が残っている場合に、もう一度だけ書き込みを試す。
        // ファイルの一時的なロック（ウイルス対策ソフト等）はこれで吸収できる
        private static void FlushIfNeeded()
        {
            if (!HasUnsavedChanges) return;

            if (Save()) return;

            // ここで失敗した時点で変更は本当に失われる。警告ではなくエラーで出す
            Debug.LogError(
                $"[AHF] アイコンの上書き設定を保存できなかったため、この変更は失われます。\n{SaveError}\n{FilePath}"
            );
        }

        // 静的コンストラクタから呼ばれるため、ここで例外を逃がすと型が失敗状態でキャッシュされ、
        // 以降この型へのアクセスがすべてTypeInitializationExceptionになる。
        // TryGetOverrideはキャッシュ構築の中で全オブジェクト分呼ばれるので、そうなると
        // アイコンに限らずHierarchyの描画が丸ごと止まる。必ず握って空データで続行する。
        private static void Load()
        {
            _data = new AHFIconOverrideData();
            LoadError = null;

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
                LoadError = $"アイコンの上書き設定を読み込めませんでした: {e.Message}";
                Debug.LogWarning($"[AHF] {LoadError}\n{FilePath}");

                // この後ユーザーがアイコンを1つ設定すると、Saveが「0件＋その1件」で
                // ファイル全体を上書きし、読めなかった内容を復旧する手段が無くなる。
                // JSONが途中で切れているだけなら手で直せることもあるので、退避しておく
                BackupUnreadableFile();
            }
        }

        private static void BackupUnreadableFile()
        {
            try
            {
                string backupPath = FilePath + ".corrupt";

                // 退避は最後の復旧手段なので、既にある退避ファイルを上書きして
                // データを減らすことがあってはいけない。
                // 1回目の破損を退避 → 空に近い状態で保存し直し → 2回目の破損 で上書きすると、
                // 価値のある1回目の退避が少ない内容で潰れる。
                // サイズで比べて大きい方を残す（同じ内容なら等しいので、
                // 破損したままリロードを繰り返しても無駄な書き込みが起きない）
                if (File.Exists(backupPath)
                    && new FileInfo(backupPath).Length >= new FileInfo(FilePath).Length)
                {
                    return;
                }

                File.Copy(FilePath, backupPath, true);
            }
            catch (System.Exception)
            {
                // 退避できなくても読み込み自体は空データで続行する。
                // ここで投げると静的コンストラクタまで巻き込むので絶対に逃がさない
            }
        }

        // 一時ファイルへ書いてから差し替えることで、書き込みが中断しても
        // 既存のファイルが壊れないようにする（成功か失敗かのどちらかに倒す）
        private static bool Save()
        {
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

                SaveError = null;
                return true;
            }
            catch (System.Exception e)
            {
                SaveError = $"アイコンの上書き設定を保存できませんでした: {e.Message}";

                // 操作した直後に、操作した本人が見る場所へ出す。
                // 設定画面のHelpBoxは後から気づくための補助で、これの代わりにはならない
                Debug.LogWarning($"[AHF] {SaveError}\n{FilePath}");

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
