using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static class ComponentIcon
    {
        static ComponentIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawComponentIcon;
        }

        private static void DrawComponentIcon(int instanceID, Rect selectionRect)
        {
            if (HierarchyCacheManager.ItemCaches == null || !HierarchyCacheManager.ItemCaches.TryGetValue(instanceID, out var cacheData))
            {
                return;
            }

            float iconSize = 16f;
            float currentX = selectionRect.xMax - iconSize;

            foreach (var icon in cacheData.ComponentIcons)
            {
                Rect iconRect = new Rect(currentX, selectionRect.y, iconSize, selectionRect.height);

                string tooltipText = "";

                if (icon.IsMissing)
                {
                    tooltipText = "Missing Script";
                }
                else if (icon.InstanceIDs != null && icon.InstanceIDs.Length > 0)
                {
                    var componentNames = getComponentNames(icon.InstanceIDs);
                    tooltipText = string.Join("\n", componentNames);
                }

                GUIContent iconContent = new GUIContent
                {
                    image = icon.IsMissing ? EditorGUIUtility.IconContent("console.warnicon").image : icon.Icon,
                    tooltip = tooltipText
                };


                if (icon.CanToggle) GUI.enabled = icon.IsEnabled;

                GUI.Box(iconRect, iconContent, GUIStyle.none);

                GUI.enabled = true;


                if (icon.CanToggle && CheckMouseDown(iconRect))
                {
                    ToggleComponentEnabled(icon.InstanceIDs, instanceID);
                }

                currentX -= iconSize;
            }
        }

        private static List<string> getComponentNames(int[] IDs)
        {
            List<string> names = new List<string>();
            foreach (int id in IDs)
            {
                var comp = EditorUtility.InstanceIDToObject(id) as Component;
                if (comp != null)
                {
                    names.Add(comp.GetType().Name);
                }
            }

            return names;
        }

        /// <summary>
        /// 指定されたRect内でマウスのクリック（ダウンイベント）があったか判定する
        /// </summary>
        private static bool CheckMouseDown(Rect rect)
        {
            Event e = Event.current;

            if (!rect.Contains(e.mousePosition)) return false;

            if (e.type != EventType.MouseDown) return false;

            if (e.button == 0)
            {
                e.Use();
                return true;
            }

            return false;
        }

        /// <summary>
        /// インスタンスIDからコンポーネントを特定し、有効・無効を反転させる
        /// </summary>
        private static void ToggleComponentEnabled(int[] multiInstanceIDs, int gameObjectInstanceID)
        {
            if (multiInstanceIDs == null || multiInstanceIDs.Length == 0) return;

            // 1. 内包されているすべてのコンポーネントの実体を逆引きしてリスト化
            List<Component> validComponents = new List<Component>();
            foreach (int id in multiInstanceIDs)
            {
                Component comp = EditorUtility.InstanceIDToObject(id) as Component;
                if (comp != null) validComponents.Add(comp);
            }

            if (validComponents.Count == 0) return;

            // 2. 一括トグル対象かどうかの判定
            Type firstType = validComponents[0].GetType();
            bool isSameTypeAll = validComponents.All(c => c.GetType() == firstType);

            // ★ 修正：型がバラバラ（MAなどの複合グループ）なら、クリックされても何もせず終了！
            if (!isSameTypeAll)
            {
                return;
            }

            // 3. 実際の反転処理（全員が同じ型であることが確定しているので、一括で処理を回す）
            List<Component> targetsToToggle = validComponents; // 全員が対象

            bool currentStatus = GetEnabledState(targetsToToggle[0]);
            bool targetStatus = !currentStatus;

            // まとめて元に戻せるように、対象全員をUndoに記録
            Undo.RecordObjects(targetsToToggle.ToArray(), "Toggle Components State");

            foreach (Component comp in targetsToToggle)
            {
                SetEnabledState(comp, targetStatus);
            }

            // 4. キャッシュ側へ通知して部分リビルド
            GameObject go = EditorUtility.InstanceIDToObject(gameObjectInstanceID) as GameObject;
            if (go != null)
            {
                Component[] filteredComponents = AHFUtil.GetFilteredComponents(go);

                // フィルター済みの綺麗な配列をキャッシュマネージャーに渡す
                HierarchyCacheManager.UpdateSingleObjectCacheDirect(gameObjectInstanceID, filteredComponents);
            }
        }

        // コンポーネントの有効状態を安全に取得するヘルパー関数
        private static bool GetEnabledState(Component c)
        {
            if (c is Behaviour b) return b.enabled;
            if (c is Collider col) return col.enabled;
            if (c is Renderer r) return r.enabled;
            return true;
        }

        // コンポーネントの有効状態を安全に設定するヘルパー関数
        private static void SetEnabledState(Component c, bool state)
        {
            if (c is Behaviour b) b.enabled = state;
            if (c is Collider col) col.enabled = state;
            if (c is Renderer r) r.enabled = state;
        }
    }
}