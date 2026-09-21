using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class ObjectIconSettingModule : IAHFSettingModule
    {
        public string LabelName => "オブジェクトアイコン";
        public string ModuleName => "ObjectIcon";

        public const int MaxIconSize = 16;
        private const int MinIconSize = 8;

        //=========================================================
        // キー
        // 有効化
        private const string Key_Enabled = "Enabled";
        public static bool IsEnabled = true;

        // アイコンのサイズ
        private const string Key_IconSize = "IconSize";
        public static int IconSize = MaxIconSize;

        // テーマカラー使うか
        private const string Key_UseThemeColor = "UseThemeColor";
        public static bool IsUseThemeColor = false;

        // オリジナルカラー
        private const string Key_OriginalColor = "OriginalColor";
        public static Color OriginalColor = Color.white;

        public void Load()
        {
            // 有効化
            IsEnabled = this.LoadBool(Key_Enabled);

            // アイコンのサイズ
            IconSize = this.LoadInt(Key_IconSize, MaxIconSize);

            // テーマカラー使うか
            IsUseThemeColor = this.LoadBool(Key_UseThemeColor, false);

            // オリジナルカラー
            OriginalColor = this.LoadColor(Key_OriginalColor, Color.white);
        }

        public void Save()
        {
            // 有効化
            this.SaveBool(Key_Enabled, IsEnabled);

            // アイコンのサイズ
            this.SaveInt(Key_IconSize, IconSize);

            // テーマカラー使うか
            this.SaveBool(Key_UseThemeColor, IsUseThemeColor);

            // オリジナルカラー
            this.SaveColor(Key_OriginalColor, OriginalColor);
        }

        public void OnGUI()
        {
            // 有効化
            IsEnabled = EditorGUILayout.Toggle("有効化", IsEnabled);

            // アイコンのサイズ
            IconSize = EditorGUILayout.IntSlider(
                new GUIContent("アイコンのサイズ", "ヒエラルキーの行の高さが上限です。"),
                IconSize,
                MinIconSize,
                MaxIconSize
            );

            // テーマカラー使うか
            IsUseThemeColor = EditorGUILayout.Toggle("テーマカラーを使う", IsUseThemeColor);

            // オリジナルカラー
            if (!IsUseThemeColor)
            {
                OriginalColor = EditorGUILayout.ColorField(
                    new GUIContent("アイコンのカラー", "アイコンに掛ける色を設定します。"),
                    OriginalColor,
                    showEyedropper: true,
                    showAlpha: false,
                    hdr: false
                );
            }
            else
            {
                EditorGUILayout.HelpBox("現在、全般設定のテーマカラーが適用されています。", MessageType.None);
            }

            EditorGUILayout.Space();

            // 手動で設定したアイコンの管理
            DrawOverrideManagement();
        }

        private static void DrawOverrideManagement()
        {
            int total = AHFIconOverrideManager.Count;
            EditorGUILayout.LabelField("手動で設定したアイコン", total + " 件");

            var targets = GetOpenTargets();
            var guids = targets.Select(t => t.Guid).ToList();
            int openCount = guids.Count == 0 ? 0 : AHFIconOverrideManager.CountForAssetGuids(guids);

            using (new EditorGUI.DisabledScope(openCount == 0))
            {
                if (GUILayout.Button("開いているシーンの分を削除（" + openCount + " 件）"))
                {
                    string names = string.Join("\n", targets.Select(t => "・" + t.Name));
                    bool agreed = EditorUtility.DisplayDialog(
                        "手動アイコンの削除",
                        "次の対象に設定された手動アイコン " + openCount + " 件を削除します。\n\n"
                        + names
                        + "\n\n削除した分は自動判定に戻ります。元に戻すには、各オブジェクトを右クリックして設定し直す必要があります。",
                        "削除する",
                        "キャンセル"
                    );

                    if (agreed) ApplyRemoval(AHFIconOverrideManager.ClearForAssetGuids(guids));
                }
            }

            using (new EditorGUI.DisabledScope(total == 0))
            {
                if (GUILayout.Button("すべて削除（" + total + " 件）"))
                {
                    bool agreed = EditorUtility.DisplayDialog(
                        "手動アイコンの削除",
                        "開いていないシーンやPrefabの分も含めて、保存されている手動アイコン " + total + " 件をすべて削除します。\n\n"
                        + "削除した分は自動判定に戻ります。元に戻すには、各オブジェクトを右クリックして設定し直す必要があります。",
                        "すべて削除する",
                        "キャンセル"
                    );

                    if (agreed) ApplyRemoval(AHFIconOverrideManager.ClearAll());
                }
            }
        }

        private static void ApplyRemoval(int removed)
        {
            if (removed <= 0) return;

            HierarchyCacheManager.CacheHierarchyObjectData();
            EditorApplication.RepaintHierarchyWindow();
        }

        private static List<OverrideTarget> GetOpenTargets()
        {
            var targets = new List<OverrideTarget>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                // ヒエラルキーに出ないシーンは対象外。NDMFのプレビューのように、実体のある
                // シーンアセットを追加で開いて裏で使うツールがあり、isSubSceneがその印。
                // (EditorSceneManager.IsPreviewSceneでは判別できない。あれはNewPreviewSceneで
                //  作られたシーン専用で、OpenSceneで開かれたものはfalseになる)
                if (scene.isSubScene) continue;

                // Prefabステージの中身は下で明示的に拾うので、ここでは除外する
                if (EditorSceneManager.IsPreviewScene(scene)) continue;

                // 未保存のシーンはアセットとして存在しないので、GUIDで突き合わせられない
                if (string.IsNullOrEmpty(scene.path)) continue;

                string guid = AssetDatabase.AssetPathToGUID(scene.path);
                if (string.IsNullOrEmpty(guid)) continue;

                targets.Add(new OverrideTarget(scene.name, guid));
            }

            // Prefabステージ編集中のオブジェクトは、シーンではなくPrefabアセットに属する
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                string guid = AssetDatabase.AssetPathToGUID(prefabStage.assetPath);
                if (!string.IsNullOrEmpty(guid))
                {
                    string name = Path.GetFileNameWithoutExtension(prefabStage.assetPath);
                    targets.Add(new OverrideTarget(name + "（Prefab編集中）", guid));
                }
            }

            return targets;
        }

        private readonly struct OverrideTarget
        {
            public readonly string Name;
            public readonly string Guid;

            public OverrideTarget(string name, string guid)
            {
                Name = name;
                Guid = guid;
            }
        }
    }
}
