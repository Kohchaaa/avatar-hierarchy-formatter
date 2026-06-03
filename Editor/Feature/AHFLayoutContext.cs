using UnityEngine;

namespace Kohcha.AvatarHierarchyFormatter
{
    public struct AHFLayoutContext
    {
        public readonly int InstanceID;
        public readonly Rect SelectionRect;

        public AHFRightOffsetEngine RightOffset;

        public AHFLayoutContext(int instanceID, Rect selectionRect, float initialOffset)
        {
            InstanceID = instanceID;
            SelectionRect = selectionRect;

            RightOffset = new AHFRightOffsetEngine(initialOffset);
        }
    }

    // 右から並べる用のオフセットと計算できるやつ持ってる構造体
    public struct AHFRightOffsetEngine
    {
        public float CurrentOffset { get; private set; }

        public AHFRightOffsetEngine(float initialOffset)
        {
            CurrentOffset = initialOffset;
        }

        public Rect GetOffsetRect(Rect selectionRect, float width, float paddingAfter)
        {
            Rect rect = new Rect(
                selectionRect.xMax - CurrentOffset - width,
                selectionRect.y,
                width,
                selectionRect.height
            );

            CurrentOffset += width + paddingAfter;
            return rect;
        }
    }
}