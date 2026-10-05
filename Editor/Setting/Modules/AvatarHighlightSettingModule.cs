using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class AvatarHighlightSettingModule : IAHFSettingModule
    {
        public string LabelName => "アバターハイライト設定";
        public string ModuleName => "AvatarHighlight";

        //=========================================================
        // 設定項目
        // 有効化
        private const string Key_Enabled = "Enabled";
        public static bool IsEnabled = true;

        // テーマカラー使うか
        private const string Key_UseThemeColor = "UseThemeColor";
        public static bool IsUseThemeColor = true;

        // オリジナルカラー
        private const string Key_OriginalColor = "OriginalColor";
        public static Color OriginalColor { get; private set; } = new Color32(128, 148, 174, 100);

        // アバタールートの区切り線
        private const string Key_AvatarRootLine = "AvatarRootLine";
        public static bool IsUseAvatarRootLine = true;

        // アバタールート区切り線のオフセット
        private const string Key_AvatarRootLineOffset = "AvatarRootLineOffset";
        public static float AvatarRootLineOffset = 0f;

        // アバタールート区切り線の太さ
        private const string Key_AvatarRootLineWeight = "AvatarRootLineWeight";
        public static float AvatarRootLineWeight = 1f;

        private const float StepSize = 0.5f;

        public void Load()
        {
            // 有効化
            IsEnabled = this.LoadBool(Key_Enabled);

            // テーマカラー使うか
            IsUseThemeColor = this.LoadBool(Key_UseThemeColor);

            // オリジナルカラー
            OriginalColor = this.LoadColor(Key_OriginalColor, new Color32(128, 148, 174, 100));

            // アバタールートの区切り線関係
            IsUseAvatarRootLine = this.LoadBool(Key_AvatarRootLine);
            AvatarRootLineOffset = this.LoadFloat(Key_AvatarRootLineOffset);
            AvatarRootLineWeight = this.LoadFloat(Key_AvatarRootLineWeight, 1f);
        }

        public void Save()
        {
            // 有効化
            this.SaveBool(Key_Enabled, IsEnabled);

            // テーマカラー使うか
            this.SaveBool(Key_UseThemeColor, IsUseThemeColor);

            // オリジナルカラー
            this.SaveColor(Key_OriginalColor, OriginalColor);

            // アバタールートの区切り線関係
            this.SaveBool(Key_AvatarRootLine, IsUseAvatarRootLine);
            this.SaveFloat(Key_AvatarRootLineOffset, AvatarRootLineOffset);
            this.SaveFloat(Key_AvatarRootLineWeight, AvatarRootLineWeight);
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
                    new GUIContent("アバターハイライトのカラー", "アバターを目立たせる色を設定します。"),
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

            // アバタールートの区切り線
            IsUseAvatarRootLine = EditorGUILayout.Toggle("アバタールートの区切り線", IsUseAvatarRootLine);
            using (var disabledScope = new EditorGUI.DisabledGroupScope(!IsUseAvatarRootLine))
            {
                AvatarRootLineOffset = EditorGUILayout.Slider("区切り線のオフセット", AvatarRootLineOffset, -2f, 16f);

                var weight = EditorGUILayout.Slider("区切り線の太さ", AvatarRootLineWeight, 0.5f, 3f);
                AvatarRootLineWeight = Mathf.Round(weight / StepSize) * StepSize;
                AvatarRootLineWeight = Mathf.Clamp(AvatarRootLineWeight, 0.5f, 3f);
            }
        }

        //=========================================================
        // オリジナル関数
        public static void ResetToDefault()
        {
            OriginalColor = new Color32(128, 148, 174, 100);
        }

        public static Color GetHeaderColor(Color c)
        {
            return new Color(c.r, c.g, c.b, 0.17f);
        }

        public static Color GetContentColor(Color c)
        {
            return new Color(c.r, c.g, c.b, 0.05f);
        }

        public static Color GetLineColor(Color c)
        {
            return new Color(c.r, c.g, c.b, 1f);
        }
    }
}