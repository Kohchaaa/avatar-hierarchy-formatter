using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    // Unityの選択確定(Selection更新)はマウスアップ時だが、ネイティブのハイライトは
    // マウスダウン時に先行表示される(ドラッグとの判別待ちのため)。それに近づけるための仮選択状態。
    // AHFFeatureManagerが全Featureの描画後に更新するため、他Featureのevt.Use()を正しく無視できる。
    internal static class AHFPendingSelectionTracker
    {
        public static int? PendingSelectedInstanceID { get; private set; }

        // 既存の選択を維持すべきクリックか。
        // (1)Shift/Ctrl(Mac:Cmd)押下 = 選択への追加操作
        // (2)クリック対象がすでに複数選択に含まれている = 複数選択ごとドラッグする可能性があり、
        //    マウスアップまでは1つに畳んではいけない
        // どちらの場合も排他表示にしてはいけない。
        public static bool PendingPreservesSelection { get; private set; }

        public static void Update(int instanceID, Rect rowRect)
        {
            Event evt = Event.current;

            if (evt.type == EventType.MouseDown && evt.button == 0 && rowRect.Contains(evt.mousePosition))
            {
                PendingSelectedInstanceID = instanceID;

                bool hasModifier = evt.shift || evt.control || evt.command;
                bool isPartOfExistingMultiSelection = Selection.instanceIDs.Length > 1
                    && Selection.instanceIDs.Contains(instanceID);

                PendingPreservesSelection = hasModifier || isPartOfExistingMultiSelection;
            }
            else if (evt.type == EventType.MouseUp && evt.button == 0)
            {
                PendingSelectedInstanceID = null;
            }
        }
    }
}
