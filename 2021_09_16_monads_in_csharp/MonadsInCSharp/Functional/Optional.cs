namespace MonadsInCSharp;

public partial class Optional<T>
{
    public bool HasValue { get; }
    public T Value { get; }
    public Optional() => (HasValue, Value) = (false, default!);
    public Optional(T value) => (HasValue, Value) = (true, value);

    public static Optional<T> None { get; } = new Optional<T>();
}