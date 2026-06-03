
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

        private const string Key_ButtonOffset = "ButtonOffset";
        public static float ButtonOffset = 0f;

        public void Load()
        {
            // 有効化
            IsEnabled = this.LoadBool(Key_Enabled);
            ButtonOffset = this.LoadFloat(Key_ButtonOffset);
        }

        public void Save()
        {
            // 有効化
            this.SaveBool(Key_Enabled, IsEnabled);
            this.LoadFloat(Key_ButtonOffset, ButtonOffset);
        }

        public void OnGUI()
        {
            // 有効化
            IsEnabled = EditorGUILayout.Toggle("有効化", IsEnabled);
            ButtonOffset = EditorGUILayout.FloatField("ボタンのオフセット", ButtonOffset);
        }

    }
}