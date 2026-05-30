using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public interface IAHFSettingModule
    {
        string LabelName { get; }
        string ModuleName { get; }

        void Load();
        void Save();
        void OnGUI();


    }

    public static class AHFSettingModuleExtensions
    {
        /// <summary>
        /// モジュール名から、EditorPrefs用のキーのPrefixを自動生成
        /// </summary>
        public static string GetKeyPrefix(this IAHFSettingModule module)
        {
            return AHFSettings.prefix + module.ModuleName + "_";
        }

        //=========================================================
        // ロード
        public static bool LoadBool(this IAHFSettingModule module, string key, bool defaultValue = true)
        {
            return EditorPrefs.GetBool(AHFSettings.prefix + module.ModuleName + "_" + key, defaultValue);
        }

        public static int LoadInt(this IAHFSettingModule module, string key, int defaultValue = 0)
        {
            return EditorPrefs.GetInt(AHFSettings.prefix + module.ModuleName + "_" + key, defaultValue);
        }

        public static float LoadFloat(this IAHFSettingModule module, string key, float defaultValue = 0f)
        {
            return EditorPrefs.GetFloat(AHFSettings.prefix + module.ModuleName + "_" + key, defaultValue);
        }

        public static string LoadString(this IAHFSettingModule module, string key, string defaultValue = "")
        {

            return EditorPrefs.GetString(AHFSettings.prefix + module.ModuleName + "_" + key, defaultValue);
        }

        public static Color LoadColor(this IAHFSettingModule module, string key, Color defaultColor)
        {
            string defaultHex = "#" + ColorUtility.ToHtmlStringRGBA(defaultColor);
            string colorHex = module.LoadString(key, defaultHex);

            if (ColorUtility.TryParseHtmlString(colorHex, out var loadedColor))
            {
                return loadedColor;
            }

            return defaultColor;
        }


        //=========================================================
        // セーブ
        public static void SaveBool(this IAHFSettingModule module, string key, bool value)
        {
            EditorPrefs.SetBool(AHFSettings.prefix + module.ModuleName + "_" + key, value);
        }

        public static void SaveInt(this IAHFSettingModule module, string key, int value)
        {
            EditorPrefs.SetInt(AHFSettings.prefix + module.ModuleName + "_" + key, value);
        }

        public static void SaveFloat(this IAHFSettingModule module, string key, float value)
        {
            EditorPrefs.SetFloat(AHFSettings.prefix + module.ModuleName + "_" + key, value);
        }

        public static void SaveString(this IAHFSettingModule module, string key, string value)
        {
            EditorPrefs.SetString(AHFSettings.prefix + module.ModuleName + "_" + key, value);
        }

        public static void SaveColor(this IAHFSettingModule module, string subKey, Color value)
        {
            string colorHex = "#" + ColorUtility.ToHtmlStringRGBA(value);
            module.SaveString(subKey, colorHex);
        }
    }
}