using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class GeneralSettingModule : IAHFSettingModule
    {
        public string LabelName => "全般設定";
        public string ModuleName => "General";

        //=========================================================
        // 設定項目
        // 機能有効化
        private const string Key_EnabledPlugin = "Enabled_Plugin";
        public static bool IsEnabled_Plugin = true;

        // アバター外のオブジェクトにも表示
        private const string Key_ShowOutsideAvatar = "ShowOutsideAvatar";
        public static bool IsShowOutsideAvatar = true;

        // テーマカラー
        private const string Key_ThemeColor = "ThemeColor";
        public static Color ThemeColor = new Color32(128, 148, 174, 100);

        public void Load()
        {
            // 機能有効化
            IsEnabled_Plugin = this.LoadBool(Key_EnabledPlugin, true);
            IsShowOutsideAvatar = this.LoadBool(Key_ShowOutsideAvatar, true);

            // テーマカラー
            ThemeColor = this.LoadColor(Key_ThemeColor, new Color32(128, 148, 174, 100));
        }

        public void Save()
        {
            // 機能有効化
            this.SaveBool(Key_EnabledPlugin, IsEnabled_Plugin);
            this.SaveBool(Key_ShowOutsideAvatar, IsShowOutsideAvatar);

            // テーマカラー
            this.SaveColor(Key_ThemeColor, ThemeColor);
        }

        public void OnGUI()
        {
            IsEnabled_Plugin = EditorGUILayout.Toggle("機能の有効化", IsEnabled_Plugin);
            IsShowOutsideAvatar = EditorGUILayout.Toggle(
                new GUIContent("アバター外にも表示", "アバター以外のオブジェクトにも各機能を表示します。アバターの背景色は対象外です。"),
                IsShowOutsideAvatar
            );

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
            ThemeColor = new Color32(128, 148, 174, 100);
        }
    }
}