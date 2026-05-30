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
            this.LoadBool(Key_Enabled, true);
        }

        public void Save()
        {
            this.SaveBool(Key_Enabled, IsEnabled);
        }
        
        public void OnGUI()
        {
            IsEnabled_ComopnentIcon = EditorGUILayout.Toggle("有効化", IsEnabled_ComopnentIcon);
        }
    }
}