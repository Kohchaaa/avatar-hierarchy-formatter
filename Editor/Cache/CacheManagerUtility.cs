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

        /// <summary>
        /// コンポーネントが有効・無効（enabled）トグル可能かどうかを判定
        /// </summary>
        public static bool CanToggleComponent(Component comp)
        {
            if (comp == null) return false;
            return comp is Behaviour || comp is Collider || comp is Renderer;
        }

        /// <summary>
        /// コンポーネントの現在の有効・無効（enabled）状態を取得
        /// </summary>
        public static bool GetEnabledState(Component comp)
        {
            if (comp == null) return true;

            return comp switch
            {
                Behaviour b => b.enabled,
                Collider col => col.enabled,
                Renderer r => r.enabled,
                _ => true
            };
        }
    }
}