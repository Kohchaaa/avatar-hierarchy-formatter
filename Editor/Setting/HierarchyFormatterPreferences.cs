using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Kohcha.UI;

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

                    // setting moduleを並べる～～
                    foreach (var module in AHFSettings.Modules)
                    {
                        KohchaGUI.Section(
                            module.LabelName,
                            () => module.OnGUI()
                        );

                        EditorGUILayout.Space();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        AHFSettings.SaveAll();
                        HierarchyCacheManager.CacheHierarchyObjectData();
                    }
                }
            };
        }
    }
}