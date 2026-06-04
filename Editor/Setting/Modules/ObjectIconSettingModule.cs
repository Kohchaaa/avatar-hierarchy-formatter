using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class ObjectIconSettingModule : IAHFSettingModule
    {
        public string LabelName => "オブジェクトアイコン";
        public string ModuleName => "ObjectIcon";

        //=========================================================
        // キー
        // 有効化
        private const string Key_Enabled = "Enabled";
        public static bool IsEnabled = true;

        public void Load(){
            // 有効化
            IsEnabled = this.LoadBool(Key_Enabled);
        }

        public void Save(){
            // 有効化
            this.SaveBool(Key_Enabled, IsEnabled);
        }

        public void OnGUI(){
            // 有効化
            IsEnabled = EditorGUILayout.Toggle("有効化", IsEnabled);
        }

    }
}