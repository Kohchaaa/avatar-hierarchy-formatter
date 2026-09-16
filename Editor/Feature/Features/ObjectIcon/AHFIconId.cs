using System;

namespace Kohcha.AvatarHierarchyFormatter
{
    public readonly struct AHFIconId : IEquatable<AHFIconId>
    {
        private readonly string _value;

        public AHFIconId(string value)
        {
            _value = value;
        }

        public bool Equals(AHFIconId other) => _value == other._value;
        public override bool Equals(object obj) => obj is AHFIconId other && Equals(other);
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value;

        public static bool operator ==(AHFIconId a, AHFIconId b) => a.Equals(b);
        public static bool operator !=(AHFIconId a, AHFIconId b) => !a.Equals(b);
    }
}
