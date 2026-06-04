using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;

namespace Kohcha.AvatarHierarchyFormatter
{
    public class ToggleEO : IAHFFeature
    {
        public string FeatureName => "ToggleEO";

        public bool IsEnabled => ToggleEOSettingModule.IsEnabled;

        private static bool _isDraggingEditorOnly = false;
        private static bool _targetEditorOnlyState = false;

        private GUIStyle _textStyle;

        public void OnGUI(ref AHFLayoutContext c)
        {
            if (!ToggleEOSettingModule.IsEnabled) return;

            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(c.InstanceID, out var cacheData))
            {
                return;
            }

            var obj = EditorUtility.InstanceIDToObject(c.InstanceID) as GameObject;
            if (obj == null) return;

            c.RightOffset.CurrentOffset += ToggleEOSettingModule.ButtonOffset;
            Rect toggleRect = c.RightOffset.GetOffsetRect(c.SelectionRect, 16, 4);
            toggleRect.y += (toggleRect.height - 16f) / 2f;
            toggleRect.height = 16f;

            Event evt = Event.current;
            bool isMouseOver = toggleRect.Contains(evt.mousePosition);

            bool isEditorOnly = obj.CompareTag("EditorOnly");

            if (evt.rawType == EventType.MouseDown && evt.button == 0 && isMouseOver)
            {
                _isDraggingEditorOnly = true;
                _targetEditorOnlyState = !isEditorOnly;

                ApplyEditorOnly(obj, _targetEditorOnlyState);
                evt.Use();
            }
            else if (_isDraggingEditorOnly && isMouseOver)
            {
                if (isEditorOnly != _targetEditorOnlyState)
                {
                    ApplyEditorOnly(obj, _targetEditorOnlyState);
                }
            }

            if (evt.rawType == EventType.MouseUp && evt.button == 0)
            {
                if (_isDraggingEditorOnly)
                {
                    _isDraggingEditorOnly = false;
                }
            }

            if (evt.type == EventType.Repaint)
            {
                bool currentTagState = obj.CompareTag("EditorOnly");

                string text = "EO";
                if (_textStyle == null)
                {
                    _textStyle = new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontStyle = FontStyle.Bold,
                        fontSize = 9
                    };
                }

                if (currentTagState)
                {
                    _textStyle.normal.textColor = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    _textStyle.normal.textColor = new Color(1f, 1f, 1f, 0.15f);
                }

                _textStyle.Draw(toggleRect, text, false, false, false, false);
            }
        }

        private void ApplyEditorOnly(GameObject go, bool makeEditorOnly)
        {
            GameObject[] targets = Selection.gameObjects.Contains(go)
                ? Selection.gameObjects
                : new GameObject[] { go };

            Undo.RegisterCompleteObjectUndo(targets, "Change GameObject Tag (AHF)");

            string targetTag = makeEditorOnly ? "EditorOnly" : "Untagged";

            foreach (var target in targets)
            {
                if (!target.CompareTag(targetTag))
                {
                    target.tag = targetTag;
                }
            }

            if (targets.Length > 0 && go.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(go.scene);
            }
        }
    }
}