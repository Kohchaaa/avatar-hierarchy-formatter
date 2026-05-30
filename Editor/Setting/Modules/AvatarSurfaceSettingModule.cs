using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class AvatarSurfaceSettingModule : IAHFSettingModule
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

        // 計算値
        public static Color HeaderColor { get; private set; }
        public static Color ContentColor { get; private set; }
        public static Color LineColor { get; private set; }



        public void Load()
        {
            // 有効化
            IsEnabled = this.LoadBool(Key_Enabled);

            // テーマカラー使うか
            IsUseThemeColor = this.LoadBool(Key_UseThemeColor);

            // オリジナルカラー
            OriginalColor = this.LoadColor(Key_OriginalColor, new Color32(128, 148, 174, 100));
            UpdateCalculatedColors();
        }

        public void Save()
        {
            // 有効化
            this.SaveBool(Key_Enabled, IsEnabled);

            // テーマカラー使うか
            this.SaveBool(Key_UseThemeColor, IsUseThemeColor);

            // オリジナルカラー
            this.SaveColor(Key_OriginalColor, OriginalColor);
            UpdateCalculatedColors();
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
        }

        //=========================================================
        // オリジナル関数
        private static void UpdateCalculatedColors()
        {
            HeaderColor = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0.17f);
            ContentColor = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0.05f);
            LineColor = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 1f);
        }

        public static void ResetToDefault()
        {
            if (ColorUtility.TryParseHtmlString("#" + ColorUtility.ToHtmlStringRGB(new Color32(128, 148, 174, 100)), out Color defaultColor))
            {
                OriginalColor = defaultColor;
            }
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