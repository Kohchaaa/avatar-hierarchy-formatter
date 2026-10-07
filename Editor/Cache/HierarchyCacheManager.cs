using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    public static partial class HierarchyCacheManager
    {
        public static Dictionary<int, CacheData> ItemCaches = new Dictionary<int, CacheData>();

        // Play中はヒエラルキーに表示されるが、対象外にする
        private const string DontDestroyOnLoadSceneName = "DontDestroyOnLoad";

        static HierarchyCacheManager()
        {
            EditorApplication.hierarchyChanged += ClearCache;
        }

        // キャッシュは描画された行の分だけ遅延で作る（TryGetOrCreate）。
        // hierarchyWindowItemOnGUIは実際に描画される行にしか呼ばれないので、
        // ヒエラルキー全体を走査せずに計算対象を画面内の行に絞れる
        public static void ClearCache()
        {
            ItemCaches.Clear();
        }

        public static bool TryGetOrCreate(int instanceID, out CacheData cacheData)
        {
            if (ItemCaches.TryGetValue(instanceID, out cacheData)) return true;

            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null || go.scene.name == DontDestroyOnLoadSceneName) return false;

            cacheData = CreateCacheData(go);
            ItemCaches[instanceID] = cacheData;
            return true;
        }

        private static CacheData CreateCacheData(GameObject go)
        {
            Transform current = go.transform;
            int currentId = go.GetInstanceID();

            // アバターはヒエラルキー1層目にVRCAvatarDescriptorがあるものだけ。
            // ネストしたDescriptorは新しいアバターとみなさず、外側のアバター内の一オブジェクトとして扱う
            Transform root = current.root;
            int? avatarRootId = root.GetComponent<VRCAvatarDescriptor>() != null
                ? root.gameObject.GetInstanceID()
                : (int?)null;

            AHFObjectScope scope = CacheData.ScopeOf(avatarRootId);

            // コンポーネント収集
            var components = AHFUtil.GetFilteredComponents(go);

            ComponentIconInfo[] icons = ConvertToIconInfo(components);

            AHFIconId? objectIconId = AHFObjectIconJudgeManager.TryJudge(go, components, scope, out var judgedIconId)
                ? judgedIconId
                : (AHFIconId?)null;

            AHFIconId? overrideIconId = AHFIconOverrideManager.TryGetOverride(currentId, out var overriddenIconId)
                ? overriddenIconId
                : (AHFIconId?)null;

            int depth = 0;
            for (var p = current.parent; p != null; p = p.parent) depth++;

            // flags[i]は、深さi+1にある祖先（末尾は自分）が最後の子でないか。
            // その深さの縦線を下まで伸ばすかどうかに使う
            bool[] flags = new bool[depth];
            Transform node = current;
            for (int i = depth - 1; i >= 0; i--)
            {
                flags[i] = !IsLastChild(node);
                node = node.parent;
            }

            return new CacheData(
                avatarRootId,
                depth,
                current.parent != null && IsLastChild(current),
                flags,
                current.childCount > 0,
                icons,
                objectIconId,
                overrideIconId
            );
        }

        private static bool IsLastChild(Transform t) =>
            t.GetSiblingIndex() == t.parent.childCount - 1;
    }
}
