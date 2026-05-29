using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static class HierarchyFormatterSettings
    {
        private const string prefix = "Kohcha.AvatarHierarchyFormatter.";

        //=========================================================
        // アバターハイライト
        private const string KeyEnabled_AvatarHighlight = prefix + "Enabled_AvatarHighlight";
        private const string KeyBaseColor = prefix + "BaseColor";

        private const string DefaultColorHEX = "8094AE";

        public static bool IsEnabled_AvatarHighlight = true;
        public static Color BaseColor = new Color32(128, 148, 174, 100);

        public static Color HeaderColor { get; private set; }
        public static Color ContentColor { get; private set; }
        public static Color LineColor { get; private set; }

        //=========================================================
        // TreeView
        private const string KeyEnabled_TreeLine = prefix + "Enabled_TreeLine";
        
        public static bool IsEnabled_TreeView = true;


        //=========================================================
        // ComponentIcon
        

        [InitializeOnLoadMethod]
        private static void LoadSettings()
        {
            //=========================================================
            // アバターハイライト
            IsEnabled_AvatarHighlight = EditorPrefs.GetBool(KeyEnabled_AvatarHighlight, true);

            string colorHex = EditorPrefs.GetString(KeyBaseColor, DefaultColorHEX);
            if (ColorUtility.TryParseHtmlString("#" + colorHex, out Color loadedColor))
            {
                BaseColor = loadedColor;
            }

            UpdateCalculatedColors();

            //=========================================================
            // TreeLine
            IsEnabled_TreeView = EditorPrefs.GetBool(KeyEnabled_TreeLine, true);
        }

        public static void SaveSettings()
        {
            //=========================================================
            // アバターハイライト
            EditorPrefs.SetBool(KeyEnabled_AvatarHighlight, IsEnabled_AvatarHighlight);
            EditorPrefs.SetString(KeyBaseColor, ColorUtility.ToHtmlStringRGB(BaseColor));

            UpdateCalculatedColors();

            //=========================================================
            // TreeLine
            EditorPrefs.SetBool(KeyEnabled_TreeLine, IsEnabled_TreeView);


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