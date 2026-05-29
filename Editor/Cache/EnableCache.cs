using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static partial class HierarchyCacheManager
    {
        public static readonly HashSet<Type> ToggleBlacklist = new HashSet<Type>
        {
            typeof(VRC_AvatarDescriptor),
            typeof(PipelineManager)
        };

        private static readonly Dictionary<Type, PropertyInfo> EnablePropertyCache = new Dictionary<Type, PropertyInfo>();

        /// <summary>
        /// コンポーネントのenable PropertyInfoを取得（存在しない場合キャッシュしてから取得）
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        private static PropertyInfo GetEnabledProperty(Component comp)
        {
            if (comp == null) return null;

            Type type = comp.GetType();

            if (EnablePropertyCache.TryGetValue(type, out var prop))
            {
                return prop;
            }

            PropertyInfo pi = type.GetProperty("enabled", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            EnablePropertyCache[type] = pi;

            return pi;
        }

        /// <summary>
        /// コンポーネントがenable状態をトグル可能か取得
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool CanToggleComponent(Component comp)
        {
            if (comp == null) return false;

            Type compType = comp.GetType();

            foreach (var blacklistType in ToggleBlacklist)
            {
                if (blacklistType.IsAssignableFrom(compType))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// コンポーネントの有効状態をリフレクションで取得
        /// </summary>
        public static bool GetEnabledState(Component comp)
        {
            if (comp == null || !comp) return true;

            if (comp is Behaviour b) return b.enabled;
            if (comp is Renderer r) return r.enabled;
            if (comp is Collider col) return col.enabled;

            PropertyInfo prop = GetEnabledProperty(comp);

            if (prop != null && prop.CanRead)
            {
                return (bool)prop.GetValue(comp);
            }

            return true;
        }

        /// <summary>
        /// コンポーネントの有効状態をリフレクションで設定
        /// </summary>
        public static void SetEnabledState(Component comp, bool state)
        {
            if (comp == null || !comp) return;

            using (SerializedObject so = new SerializedObject(comp))
            {
                SerializedProperty enabledProp = so.FindProperty("m_Enabled");
                if (enabledProp != null)
                {
                    so.Update();
                    enabledProp.boolValue = state;
                    so.ApplyModifiedProperties();
                }
            }

            EditorUtility.SetDirty(comp);

            RepaintInspectorWindow();
        }
    }
}