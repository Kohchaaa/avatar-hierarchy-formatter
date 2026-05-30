using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class TreeViewSettingModule : IAHFSettingModule
    {
        public string ModuleName => "ツリービューの線";

        //=========================================================
        // キー
        // 有効化
        private const string KeyEnabled_TreeLine = AHFSettings.prefix + "Enabled_TreeLine";
        public static bool IsEnabled_TreeView = true;

        // テーマカラー使うか
        private const string Key_UseThemeColor = AHFSettings.prefix + "UseThemeColor";
        public static bool IsUseThemeColor = true;

        // オリジナルカラー
        private const string Key_OriginalColor = AHFSettings.prefix + "OriginalColor";
        public static Color OriginalColor = new Color32(126, 126, 126, 255);
        private const string DefaultColorHEX = "7E7E7E";

        public void Load()
        {
            // 有効化
            IsEnabled_TreeView = EditorPrefs.GetBool(KeyEnabled_TreeLine, true);

            // テーマカラー使うか
            IsUseThemeColor = EditorPrefs.GetBool(Key_UseThemeColor, true);

            // オリジナルカラー
            if (ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(Key_OriginalColor, DefaultColorHEX), out Color loadedColor))
            {
                OriginalColor = loadedColor;
            }
        }

        public void Save()
        {
            // 有効化
            EditorPrefs.SetBool(KeyEnabled_TreeLine, IsEnabled_TreeView);

            // テーマカラー使うか
            EditorPrefs.SetBool(Key_UseThemeColor, IsUseThemeColor);

            // オリジナルカラー
            EditorPrefs.GetString(Key_OriginalColor, ColorUtility.ToHtmlStringRGB(OriginalColor));

        }

        public void OnGUI()
        {
            // 有効化
            IsEnabled_TreeView = EditorGUILayout.Toggle("有効化", IsEnabled_TreeView);

            // テーマカラー使うか
            IsUseThemeColor = EditorGUILayout.Toggle("テーマカラーを使う", IsUseThemeColor);

            // オリジナルカラー
            if (!IsUseThemeColor)
            {
                OriginalColor = EditorGUILayout.ColorField(
                    new GUIContent("ツリーのカラー", "ツリーの線のカラーを設定します。"),
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

        }
    }
}