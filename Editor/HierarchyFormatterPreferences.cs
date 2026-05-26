using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static class HierarchyFormatterPreferences
    {
        [SettingsProvider]
        public static SettingsProvider CreatePreferencesProvider()
        {
            return new SettingsProvider("Preferences/Avatar Hierarchy Formatter", SettingsScope.User)
            {
                guiHandler = (searchContext) =>
                {
                    EditorGUI.BeginChangeCheck();

                    //===================================================
                    // アバターハイライト設定
                    HierarchyFormatterSettings.IsEnabled_AvatarHighlight = EditorGUILayout.Toggle("アバターハイライト", HierarchyFormatterSettings.IsEnabled_AvatarHighlight);

                    using (new EditorGUI.DisabledScope(!HierarchyFormatterSettings.IsEnabled_AvatarHighlight))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            HierarchyFormatterSettings.BaseColor = EditorGUILayout.ColorField(
                                new GUIContent("ベースカラー", "アバター全体のテーマ色となるベースの色を選択します。"),
                                HierarchyFormatterSettings.BaseColor,
                                showEyedropper: true,
                                showAlpha: false,
                                hdr: false
                            );

                            GUIContent refreshIcon = EditorGUIUtility.IconContent("d_Refresh");
                            refreshIcon.tooltip = "ベースカラーをデフォルトに戻す";

                            if (GUILayout.Button(refreshIcon, GUILayout.Width(24), GUILayout.Height(18), GUILayout.ExpandWidth(false)))
                            {
                                HierarchyFormatterSettings.ResetToDefault();
                            }
                        }
                    }
                    

                    if (EditorGUI.EndChangeCheck())
                    {
                        HierarchyFormatterSettings.SaveSettings();
                    }
                }
            };
        }
    }
}