using System.Collections.Generic;
using UnityEngine;
using VRC.Core; // PipelineManagerのため

namespace Kohcha.AvatarHierarchyFormatter
{
    public static partial class AHFUtil
    {
        // GetComponents<T>()（引数なし）は呼ぶたびに配列を新規確保する。
        // この関数はキャッシュ構築でヒエラルキーの全オブジェクトについて呼ばれるので、
        // リストに詰めるオーバーロードを使い回して確保を避ける。
        // 使い回しなので、この関数の実行中に同じ関数を呼ぶ形（再帰）にしてはいけない
        private static readonly List<Component> _componentBuffer = new List<Component>();

        /// <summary>
        /// GameObjectから不要なコンポーネント（Transform, Animator, PipelineManager）を除外して取得する共通関数
        /// </summary>
        public static Component[] GetFilteredComponents(GameObject go)
        {
            if (go == null) return System.Array.Empty<Component>();

            _componentBuffer.Clear();
            go.GetComponents(_componentBuffer);

            // 必要な大きさの配列を1つだけ作るために、先に件数を数える
            int count = 0;
            for (int i = 0; i < _componentBuffer.Count; i++)
            {
                if (IsTargetComponent(_componentBuffer[i])) count++;
            }

            // ボーンのようにTransformしか持たないオブジェクトが大半なので、
            // ここで確保せずに返せる経路があることの効果が大きい
            if (count == 0) return System.Array.Empty<Component>();

            var result = new Component[count];
            int written = 0;
            for (int i = 0; i < _componentBuffer.Count; i++)
            {
                Component c = _componentBuffer[i];
                if (IsTargetComponent(c)) result[written++] = c;
            }

            return result;
        }

        // nullはここを通す。スクリプトが欠損したコンポーネントはnullになり、
        // 以前のWhereによる除外でも残っていたので、挙動を変えないため
        private static bool IsTargetComponent(Component c) =>
            c is not Transform && c is not Animator && c is not PipelineManager;
    }
}
