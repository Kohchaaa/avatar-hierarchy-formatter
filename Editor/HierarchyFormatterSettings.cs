using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static class HierarchyFormatterSettings
    {
        private const string KeyEnabled_AvatarHighlight = "Kohcha.AvatarHierarchyFormatter.Enabled_AvatarHighlight";
        private const string KeyBaseColor = "Kohcha.AvatarHierarchyFormatter.BaseColor";

        private const string DefaultColorHEX = "8094AE";

        public static bool IsEnabled_AvatarHighlight = true;
        public static Color BaseColor = new Color32(128, 148, 174, 100);

        public static Color HeaderColor { get; private set; }
        public static Color ContentColor { get; private set; }
        public static Color LineColor { get; private set; }

        [InitializeOnLoadMethod]
        private static void LoadSettings()
        {
            IsEnabled_AvatarHighlight = EditorPrefs.GetBool(KeyEnabled_AvatarHighlight, true);

            string colorHex = EditorPrefs.GetString(KeyBaseColor, DefaultColorHEX);
            if (ColorUtility.TryParseHtmlString("#" + colorHex, out Color loadedColor))
            {
                BaseColor = loadedColor;
            }

            UpdateCalculatedColors();
        }

        public static void SaveSettings()
        {
            EditorPrefs.SetBool(KeyEnabled_AvatarHighlight, IsEnabled_AvatarHighlight);
            EditorPrefs.SetString(KeyBaseColor, ColorUtility.ToHtmlStringRGB(BaseColor));

            UpdateCalculatedColors();

            EditorApplication.RepaintHierarchyWindow();
        }

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

            SaveSettings();
        }
    }
}