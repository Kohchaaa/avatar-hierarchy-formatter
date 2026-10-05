using System.Text.RegularExpressions;

namespace Kohcha.AvatarHierarchyFormatter
{
    public static partial class AHFUtil
    {
        // Unityで複製したオブジェクトは"Body (1)"のように末尾が" (n)"になる。
        // 複製しただけで判定結果が変わるのはおかしいので、名前ベースのJudgeは
        // この接尾辞を落としてから判定する。
        // FBX由来の".001"は各Judgeのパターン側が受けているのでここでは扱わない
        private static readonly Regex DuplicateSuffixPattern = new Regex(
            @"\s*\(\d+\)$",
            RegexOptions.Compiled
        );

        /// <summary>
        /// Unityの複製で付く末尾の" (n)"を取り除いた名前を返す
        /// </summary>
        public static string StripDuplicateSuffix(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            return DuplicateSuffixPattern.Replace(name, "");
        }
    }
}
