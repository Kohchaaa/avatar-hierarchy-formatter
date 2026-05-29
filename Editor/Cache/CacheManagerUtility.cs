using System;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static partial class HierarchyCacheManager
    {
        /// <summary>
        /// 特定のオブジェクトのコンポーネント状態だけをその場で再解析して、キャッシュのアイコン情報のみを更新する
        /// </summary>
        public static void UpdateGameObjectCache(int gameObjectInstanceID, Component[] rawComponents)
        {
            if (ItemCaches.TryGetValue(gameObjectInstanceID, out var cacheData))
            {
                cacheData.ComponentIcons = ConvertToIconInfos(rawComponents);

                ItemCaches[gameObjectInstanceID] = cacheData;

                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private static void RepaintInspectorWindow()
        {
            EditorApplication.delayCall += () =>
            {
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            };
        }
    }
}