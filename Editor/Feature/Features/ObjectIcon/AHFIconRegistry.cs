using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class AHFIconRegistry
    {
        private static readonly Dictionary<AHFIconId, AHFIconEntry> _entries = new Dictionary<AHFIconId, AHFIconEntry>();

        static AHFIconRegistry()
        {
            Register(new AHFIconEntry { Id = new AHFIconId("avatar_root"), DisplayName = "Avatar",    IconGUID = "8c85a76a80726744c95180990acb729b" });
            Register(new AHFIconEntry { Id = new AHFIconId("armature"),    DisplayName = "Armature",  IconGUID = "daaaab69ab3686c469b40ad3943df7ca" });
            Register(new AHFIconEntry { Id = new AHFIconId("hips"),        DisplayName = "Hips",      IconGUID = "e2e398e99297566408d0b000ac479471" });
            Register(new AHFIconEntry { Id = new AHFIconId("head"),        DisplayName = "Head",      IconGUID = "618135973beea5648ba0f5dd3f387042" });
            Register(new AHFIconEntry { Id = new AHFIconId("body"),        DisplayName = "Body",      IconGUID = "54521feb44cabec4bad28d6e3b3a3c8c" });
            Register(new AHFIconEntry { Id = new AHFIconId("body_base"),   DisplayName = "Body Base", IconGUID = "90a3d412681c05c40aee60cf6e443f90" });
            Register(new AHFIconEntry { Id = new AHFIconId("gimmick"),     DisplayName = "Gimmick",   IconGUID = "62b196d14d1513647bd3a333756bd05d" });
            Register(new AHFIconEntry { Id = new AHFIconId("outfit"),      DisplayName = "Outfit",    IconGUID = "93a8a3efa371c7547ae7932e9cbf03d0" });
            Register(new AHFIconEntry { Id = new AHFIconId("light"),       DisplayName = "Light",     IconGUID = "8db52861ea0364142aecd47469923147" });

            // 暫定的にUnity組み込みアイコン
            Register(new AHFIconEntry { Id = new AHFIconId("vrc_system"),      DisplayName = "VRC System",      IconPathOrName = "EditorSettings Icon" });
            Register(new AHFIconEntry { Id = new AHFIconId("audio"),           DisplayName = "Audio",           IconPathOrName = "AudioSource Icon" });
            Register(new AHFIconEntry { Id = new AHFIconId("gesture_manager"), DisplayName = "Gesture Manager", IconPathOrName = "GameObject Icon" });
            Register(new AHFIconEntry { Id = new AHFIconId("face_emo"),        DisplayName = "FaceEmo",         IconPathOrName = "GameObject Icon" });
        }

        public static void Register(AHFIconEntry entry) => _entries[entry.Id] = entry;
        public static void Unregister(AHFIconId id) => _entries.Remove(id);
        public static bool TryGetEntry(AHFIconId id, out AHFIconEntry entry) => _entries.TryGetValue(id, out entry);
        public static Texture2D GetTexture(AHFIconId id) => TryGetEntry(id, out var entry) ? entry.GetTexture() : null;
        public static IReadOnlyCollection<AHFIconEntry> All => _entries.Values;
    }
}
