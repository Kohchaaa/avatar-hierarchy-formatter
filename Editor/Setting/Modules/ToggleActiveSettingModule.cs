
using UnityEditor;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class ToggleActiveSettingModule : IAHFSettingModule
    {
        public string LabelName => "アクティブ状態切替ボタン";
        public string ModuleName => "ToggleActive";

        //=========================================================
        // キー
        // 有効化
        private const string Key_Enabled = "Enabled";
        public static bool IsEnabled = true;

        public void Load()
        {
            // 有効化
            IsEnabled = this.LoadBool(Key_Enabled, true);
        }

        public void Save()
        {
            // 有効化
            this.SaveBool(Key_Enabled, IsEnabled);
        }

        public void OnGUI()
        {
            // 有効化
            IsEnabled = EditorGUILayout.Toggle("有効化", IsEnabled);
        }

    }
}