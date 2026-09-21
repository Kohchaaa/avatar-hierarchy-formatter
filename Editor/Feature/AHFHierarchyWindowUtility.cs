using System;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    [InitializeOnLoad]
    internal static class AHFHierarchyWindowUtility
    {
        private static EditorWindow _hierarchyWindow;

        static AHFHierarchyWindowUtility()
        {
            EditorApplication.delayCall += FindHierarchyWindow;
        }

        private static void FindHierarchyWindow()
        {
            var hierarchyWindowType = Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor");
            if (hierarchyWindowType == null)
            {
                Debug.LogWarning("[AHF] SceneHierarchyWindow の型が見つかりませんでした");
                return;
            }

            foreach (var obj in Resources.FindObjectsOfTypeAll(hierarchyWindowType))
            {
                if (obj is EditorWindow window)
                {
                    _hierarchyWindow = window;
                }
            }
        }

        /// <summary>
        /// 検出済みヒエラルキーウィンドウの現在の幅を返す。取得できていない場合はnull。
        /// </summary>
        public static float? GetWidth()
        {
            return _hierarchyWindow != null ? _hierarchyWindow.position.width : (float?)null;
        }
    }
}
