
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

        private static bool _isDraggingActiveToggle = false;
        private static bool _targetActiveState = false;

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

            Event evt = Event.current;
            bool isMouseOver = toggleRect.Contains(evt.mousePosition);

            bool currentActive = obj.activeSelf;

            if (evt.rawType == EventType.MouseDown && evt.button == 0 && isMouseOver)
            {
                _isDraggingActiveToggle = true;
                _targetActiveState = !currentActive;

                ApplyActiveState(obj, _targetActiveState);
                evt.Use();
            }
            else if (_isDraggingActiveToggle && isMouseOver)
            {
                if (currentActive != _targetActiveState)
                {
                    ApplyActiveState(obj, _targetActiveState);
                }
            }

            if (evt.rawType == EventType.MouseUp && evt.button == 0)
            {
                if (_isDraggingActiveToggle)
                {
                    _isDraggingActiveToggle = false;
                }
            }

            if (evt.type == EventType.Repaint)
            {
                GUIStyle toggleStyle = EditorStyles.toggle;
                toggleStyle.Draw(toggleRect, GUIContent.none, false, false, currentActive, false);
            }
        }

        private void ApplyActiveState(GameObject go, bool active)
        {
            GameObject[] targets = Selection.gameObjects.Contains(go)
                ? Selection.gameObjects
                : new GameObject[] { go };

            Undo.RecordObjects(targets, "Toggle GameObject Active (AHF)");

            foreach (var target in targets)
            {
                target.SetActive(active);
            }

            if (targets.Length > 0 && go.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(go.scene);
            }
        }
    }
}