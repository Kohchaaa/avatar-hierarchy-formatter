using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class TreeViewSettingModule : IAHFSettingModule
    {
        public string ModuleName => "ツリービューの線";

        //=========================================================
        // キー
        private const string KeyEnabled_TreeLine = AHFSettings.prefix + "Enabled_TreeLine";
        
        public static bool IsEnabled_TreeView = true;

        public void Load()
        {
            IsEnabled_TreeView = EditorPrefs.GetBool(KeyEnabled_TreeLine, true);
        }

        public void Save()
        {
            EditorPrefs.SetBool(KeyEnabled_TreeLine, IsEnabled_TreeView);
        }

        public void OnGUI()
        {
            IsEnabled_TreeView = EditorGUILayout.Toggle("有効化", IsEnabled_TreeView);
        }
    }
}