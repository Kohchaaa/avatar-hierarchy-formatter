using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class AHFIconOverrideManager
    {
        private const string FilePath = "ProjectSettings/Packages/jp.kohchaaa.avatar-hierarchy-formatter/IconOverrides.json";

        private static AHFIconOverrideData _data;

        static AHFIconOverrideManager()
        {
            Load();
        }

        private static void Load()
        {
            _data = ScriptableObject.CreateInstance<AHFIconOverrideData>();

            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                JsonUtility.FromJsonOverwrite(json, _data);
            }
        }

        private static void Save()
        {
            string directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(FilePath, json);
        }

        private static string GetId(int instanceId) => GlobalObjectId.GetGlobalObjectIdSlow(instanceId).ToString();

        public static bool TryGetOverride(int instanceId, out AHFIconId iconId)
        {
            string id = GetId(instanceId);
            var entry = _data.Entries.FirstOrDefault(e => e.GlobalObjectId == id);

            if (entry != null)
            {
                iconId = new AHFIconId(entry.IconId);
                return true;
            }

            iconId = default;
            return false;
        }

        public static void SetOverride(int instanceId, AHFIconId iconId)
        {
            string id = GetId(instanceId);

            Undo.RecordObject(_data, "Set Custom Icon");

            var entry = _data.Entries.FirstOrDefault(e => e.GlobalObjectId == id);
            if (entry != null)
            {
                entry.IconId = iconId.ToString();
            }
            else
            {
                _data.Entries.Add(new AHFIconOverrideEntry { GlobalObjectId = id, IconId = iconId.ToString() });
            }

            Save();
        }

        public static void RemoveOverride(int instanceId)
        {
            string id = GetId(instanceId);

            Undo.RecordObject(_data, "Remove Custom Icon");

            _data.Entries.RemoveAll(e => e.GlobalObjectId == id);

            Save();
        }
    }
}
