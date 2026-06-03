
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public class ToggleActive : IAHFFeature
    {
        public string FeatureName => "ToggleActive";
        public bool IsEnabled => ToggleActiveSettingModule.IsEnabled;

        public void OnGUI(ref AHFLayoutContext c)
        {
            if (!ToggleActiveSettingModule.IsEnabled) return;

            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(c.InstanceID, out var cacheData))
            {
                return;
            }

            var obj = EditorUtility.InstanceIDToObject(c.InstanceID) as GameObject;
            if (obj == null) return;

            c.RightOffset.CurrentOffset += ToggleActiveSettingModule.ButtonOffset;

            Rect toggleRect = c.RightOffset.GetOffsetRect(c.SelectionRect, 16, 4);

            toggleRect.y += (toggleRect.height - 16f) / 2f;
            toggleRect.height = 16f;

            bool currentActive = obj.activeSelf;

            EditorGUI.BeginChangeCheck();
            bool newActive = EditorGUI.Toggle(toggleRect, currentActive);

            if (EditorGUI.EndChangeCheck())
            {
                GameObject[] targets = Selection.gameObjects.Contains(obj)
                    ? Selection.gameObjects
                    : new GameObject[] { obj };

                Undo.RecordObjects(targets, "Toggle GameObject Active (AHF)");

                foreach (var target in targets)
                {
                    target.SetActive(newActive);
                }

                if (targets.Length > 0 && obj.scene.IsValid())
                {
                    EditorSceneManager.MarkSceneDirty(obj.scene);
                }

                Event.current.Use();
            }
        }
    }
}