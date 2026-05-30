using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class ComponentIconSettingModule : IAHFSettingModule
    {
        public string LabelName => "コンポーネントアイコン";

        //=========================================================
        // キー
        private const string KeyEnabled_ComponentIcon = AHFSettings.prefix + "Enabled_ComponentIcon";
        public static bool IsEnabled_ComopnentIcon = true;

        public void Load()
        {
            IsEnabled_ComopnentIcon = EditorPrefs.GetBool(KeyEnabled_ComponentIcon, true);
        }
        public void Save()
        {
            EditorPrefs.SetBool(KeyEnabled_ComponentIcon, IsEnabled_ComopnentIcon);
        }
        public void OnGUI()
        {
            IsEnabled_ComopnentIcon = EditorGUILayout.Toggle("有効化", IsEnabled_ComopnentIcon);
        }
    }
}