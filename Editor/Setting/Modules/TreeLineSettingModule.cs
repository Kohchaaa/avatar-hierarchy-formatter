using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class TreeLineSettingModule : IAHFSettingModule
    {
        public string LabelName => "ツリーの線";
        public string ModuleName => "TreeLine";

        //=========================================================
        // キー
        // 有効化
        private const string Key_Enabled = "Enabled";
        public static bool IsEnabled = true;

        // テーマカラー使うか
        private const string Key_UseThemeColor = "UseThemeColor";
        public static bool IsUseThemeColor = true;

        // オリジナルカラー
        private const string Key_OriginalColor = "OriginalColor";
        public static Color OriginalColor = new Color32(126, 126, 126, 255);
        private const string DefaultColorHEX = "7E7E7E";

        public void Load()
        {
            // 有効化
            IsEnabled = this.LoadBool(Key_Enabled, true);

            // テーマカラー使うか
            IsUseThemeColor = this.LoadBool(Key_UseThemeColor, true);

            // オリジナルカラー
            OriginalColor = this.LoadColor(Key_OriginalColor, new Color32(126, 126, 126, 255));
        }

        public void Save()
        {
            // 有効化
            this.SaveBool(Key_Enabled, IsEnabled);

            // テーマカラー使うか
            this.SaveBool(Key_Enabled, IsUseThemeColor);

            // オリジナルカラー
            this.SaveColor(Key_OriginalColor, OriginalColor);
        }

        public void OnGUI()
        {
            // 有効化
            IsEnabled = EditorGUILayout.Toggle("有効化", IsEnabled);

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