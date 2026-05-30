using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class GeneralSettingModule : IAHFSettingModule
    {
        public string ModuleName => "全般設定";

        //=========================================================
        // 設定項目
        // 機能有効化
        private const string Key_EnabledFeatures = AHFSettings.prefix + "Enabled_Features";
        public static bool IsEnabled_Features = true;

        // テーマカラー
        private const string Key_ThemeColor = AHFSettings.prefix + "ThemeColor";
        public static Color ThemeColor = new Color32(128, 148, 174, 100);
        private const string DefaultColorHEX = "8094AE";

        public void Load()
        {
            // 機能有効化
            IsEnabled_Features = EditorPrefs.GetBool(Key_EnabledFeatures, true);

            // テーマカラー
            if (ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(Key_ThemeColor, DefaultColorHEX), out Color loadedColor))
            {
                ThemeColor = loadedColor;
            }
        }

        public void Save()
        {
            // 機能有効化
            EditorPrefs.SetBool(Key_EnabledFeatures, IsEnabled_Features);

            // テーマカラー
            EditorPrefs.GetString(Key_ThemeColor, ColorUtility.ToHtmlStringRGB(ThemeColor));
        }

        public void OnGUI()
        {
            IsEnabled_Features = EditorGUILayout.Toggle("機能の有効化", IsEnabled_Features);

            using (new EditorGUILayout.HorizontalScope())
            {
                ThemeColor = EditorGUILayout.ColorField(
                    new GUIContent("テーマカラー", "全体のテーマカラーを設定します。"),
                    ThemeColor,
                    showEyedropper: true,
                    showAlpha: false,
                    hdr: false
                );

                GUIContent refreshIcon = EditorGUIUtility.IconContent("d_Refresh");
                refreshIcon.tooltip = "テーマカラーをデフォルトに戻す";

                if (
                    GUILayout.Button(
                        refreshIcon, 
                        GUILayout.Width(24), 
                        GUILayout.Height(18), 
                        GUILayout.ExpandWidth(false)
                    )
                ) {
                    ResetToDefault();
                }
            }
        }

        //=========================================================
        // オリジナル関数
        public static void ResetToDefault()
        {
            if (ColorUtility.TryParseHtmlString("#" + DefaultColorHEX, out Color defaultColor))
            {
                ThemeColor = defaultColor;
            }
        }
    }
}