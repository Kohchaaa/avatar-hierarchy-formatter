using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class ComponentIconSettingModule : IAHFSettingModule
    {
        public string LabelName => "コンポーネントアイコン";
        public string ModuleName => "ComopnentIcon";

        //=========================================================
        // キー
        private const string Key_Enabled = "Enabled";
        public static bool IsEnabled = true;

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
            IsEnabled = EditorGUILayout.Toggle("有効化", IsEnabled);
        }
    }
}