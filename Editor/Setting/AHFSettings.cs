using System.Collections.Generic;
using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static class AHFSettings
    {
        public const string prefix = "Kohcha.AvatarHierarchyFormatter.";
        public static readonly List<IAHFSettingModule> Modules = new List<IAHFSettingModule>();

        static AHFSettings()
        {
            Modules.Add(new AvatarSurfaceSettingModule());
            Modules.Add(new TreeViewSettingModule());
        }

        [InitializeOnLoadMethod]
        private static void Init() { }

        public static void LoadAll()
        {
            foreach (var module in Modules)
            {
                module.Load();
            }
        }

        public static void SaveAll()
        {
            foreach (var module in Modules)
            {
                module.Save();
            }
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}