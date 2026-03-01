namespace System.Runtime.CompilerServices
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Property |
        AttributeTargets.Field |
        AttributeTargets.Event |
        AttributeTargets.Parameter |
        AttributeTargets.ReturnValue |
        AttributeTargets.GenericParameter,
        AllowMultiple = false,
        Inherited = false)]
    internal sealed class NullableAttribute : Attribute
    {
        public NullableAttribute(byte value) => NullableFlags = new[] { value };

        public NullableAttribute(byte[] value) => NullableFlags = value;

        public readonly byte[] NullableFlags;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class NullableContextAttribute : Attribute
    {
        public NullableContextAttribute(byte value) => Flag = value;

        public readonly byte Flag;
    }
}
