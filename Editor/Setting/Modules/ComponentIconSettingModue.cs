using NUnit.Framework;
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

        private const string Key_IconOffset = "IconOffset";
        public static float IconOffset = 0f;

        private const string Key_AllowToggleFromIcon = "AllowToggleFromIcon";
        public static bool IsAllowToggleFromIcon = true;

        private const string Key_AllowGrouping = "AllowGrouping";
        public static bool IsAllowGrouping = true;

        private const string Key_AllowCustomIcon = "AllowCustomIcon";
        public static bool IsAllowCustomIcon = true;

        public void Load()
        {
            IsEnabled = this.LoadBool(Key_Enabled);
            IconOffset = this.LoadFloat(Key_IconOffset);
            IsAllowToggleFromIcon = this.LoadBool(Key_AllowToggleFromIcon);
            IsAllowGrouping = this.LoadBool(Key_AllowGrouping);
            IsAllowCustomIcon = this.LoadBool(Key_AllowCustomIcon);
        }

        public void Save()
        {
            this.SaveBool(Key_Enabled, IsEnabled);
            this.SaveFloat(Key_IconOffset, IconOffset);
            this.SaveBool(Key_AllowToggleFromIcon, IsAllowToggleFromIcon);
            this.SaveBool(Key_AllowGrouping, IsAllowGrouping);
            this.SaveBool(Key_AllowCustomIcon, IsAllowCustomIcon);
        }

        public void OnGUI()
        {
            IsEnabled = EditorGUILayout.Toggle("有効化", IsEnabled);
            IconOffset = EditorGUILayout.FloatField("アイコンのオフセット", IconOffset);
            IsAllowToggleFromIcon = EditorGUILayout.Toggle("アイコンクリックでトグルを許可", IsAllowToggleFromIcon);
            IsAllowGrouping = EditorGUILayout.Toggle("アイコンのグループ化を許可", IsAllowGrouping);
            IsAllowCustomIcon = EditorGUILayout.Toggle("カスタムアイコンを使用", IsAllowCustomIcon);
        }
    }
}