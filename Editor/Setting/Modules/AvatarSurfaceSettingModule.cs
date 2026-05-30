using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class AvatarSurfaceSettingModule : IAHFSettingModule
    {
        public string ModuleName => "アバターハイライト設定";


        //=========================================================
        // 設定項目
        // 有効化
        private const string KeyEnabled_AvatarHighlight = AHFSettings.prefix + "Enabled_AvatarHighlight";
        public static bool IsEnabled_AvatarHighlight = true;

        // カラー
        private const string KeyBaseColor = AHFSettings.prefix + "BaseColor";
        public static Color BaseColor = new Color32(128, 148, 174, 100);

        
        // 計算値
        public static Color HeaderColor { get; private set; }
        public static Color ContentColor { get; private set; }
        public static Color LineColor { get; private set; }

        private const string DefaultColorHEX = "8094AE";

        public void Load()
        {
            IsEnabled_AvatarHighlight = EditorPrefs.GetBool(KeyEnabled_AvatarHighlight, true);

            string colorHex = EditorPrefs.GetString(KeyBaseColor, DefaultColorHEX);
            if (ColorUtility.TryParseHtmlString("#" + colorHex, out Color loadedColor))
            {
                BaseColor = loadedColor;
            }

            UpdateCalculatedColors();
        }

        public void Save()
        {
            EditorPrefs.SetBool(KeyEnabled_AvatarHighlight, IsEnabled_AvatarHighlight);
            EditorPrefs.SetString(KeyBaseColor, ColorUtility.ToHtmlStringRGB(BaseColor));

            UpdateCalculatedColors();
        }

        public void OnGUI()
        {
            IsEnabled_AvatarHighlight = EditorGUILayout.Toggle("有効化", IsEnabled_AvatarHighlight);

            using (new EditorGUI.DisabledScope(!IsEnabled_AvatarHighlight))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    BaseColor = EditorGUILayout.ColorField(
                        new GUIContent("ベースカラー", "アバター全体のテーマ色となるベースの色を選択します。"),
                        BaseColor,
                        showEyedropper: true,
                        showAlpha: false,
                        hdr: false
                    );

                    GUIContent refreshIcon = EditorGUIUtility.IconContent("d_Refresh");
                    refreshIcon.tooltip = "ベースカラーをデフォルトに戻す";

                    if (GUILayout.Button(refreshIcon, GUILayout.Width(24), GUILayout.Height(18), GUILayout.ExpandWidth(false)))
                    {
                        ResetToDefault();
                    }
                }
            }
        }

        //=========================================================
        // オリジナル関数
        private static void UpdateCalculatedColors()
        {
            HeaderColor = new Color(BaseColor.r, BaseColor.g, BaseColor.b, 0.17f);

            ContentColor = new Color(BaseColor.r, BaseColor.g, BaseColor.b, 0.05f);

            LineColor = new Color(BaseColor.r, BaseColor.g, BaseColor.b, 1f);
        }

        public static void ResetToDefault()
        {
            if (ColorUtility.TryParseHtmlString("#" + DefaultColorHEX, out Color defaultColor))
            {
                BaseColor = defaultColor;
            }
        }
    }
}