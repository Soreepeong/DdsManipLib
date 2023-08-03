using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DdsManipLib.Utilities;

public struct Vector3<T> : IList<T>, IEquatable<Vector3<T>> where T : unmanaged {
    public T X;
    public T Y;
    public T Z;

    public Vector3() { }

    public Vector3(T value) => X = Y = Z = value;

    public Vector3(Vector2<T> v2, T z) {
        X = v2.X;
        Y = v2.Y;
        Z = z;
    }

    public Vector3(T x, T y, T z) {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3(IEnumerator<T> enumerator) {
        if (!enumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(enumerator), enumerator, null);
        X = enumerator.Current;
        if (!enumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(enumerator), enumerator, null);
        Y = enumerator.Current;
        if (!enumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(enumerator), enumerator, null);
        Z = enumerator.Current;
        if (!enumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(enumerator), enumerator, null);
    }

    public Vector3(IEnumerable<T> enumerable) : this(enumerable.GetEnumerator()) { }

    public IEnumerator<T> GetEnumerator() {
        yield return X;
        yield return Y;
        yield return Z;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => Equals(item, X) || Equals(item, Y) || Equals(item, Z);

    public void CopyTo(T[] array, int arrayIndex) {
        if (arrayIndex + 3 > array.Length)
            throw new ArgumentException(null, nameof(array));
        array[arrayIndex + 0] = X;
        array[arrayIndex + 1] = Y;
        array[arrayIndex + 2] = Z;
    }

    public void CopyTo(Span<T> span) {
        if (span.Length < 3)
            throw new ArgumentException(null, nameof(span));
        span[0] = X;
        span[1] = Y;
        span[2] = Z;
    }

    public bool Remove(T item) => throw new NotSupportedException();

    public int Count => 3;
    public bool IsReadOnly => false;

    public int IndexOf(T item) {
        if (Equals(item, X))
            return 0;
        if (Equals(item, Y))
            return 1;
        if (Equals(item, Z))
            return 2;
        return -1;
    }

    public void Insert(int index, T item) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();

    public T this[int index] {
        get => index switch {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
        };
        set {
            switch (index) {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                case 2:
                    Z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }

    public bool Equals(Vector3<T> other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

    public override bool Equals(object? obj) => obj is Vector3<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public override string ToString() => $"<{X}, {Y}, {Z}>";

    public void Deconstruct(out T x, out T y, out T z) => (x, y, z) = (X, Y, Z);
}
