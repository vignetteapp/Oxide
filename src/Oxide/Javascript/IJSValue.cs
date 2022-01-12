using System;

namespace Oxide.Javascript
{
    public interface IJSValue : IEquatable<IJSValue>
    {
        JSContext Context { get; }
        JSType Type { get; }
        bool IsArray { get; }
        JSTypedArrayType ArrayType { get; }
    }

    public interface IJSValue<T> : IJSValue, IEquatable<IJSValue<T>>
    {
        T Value { get; }
    }
}
