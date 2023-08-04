using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace DdsManipLib.Utilities;

public struct Vector2<T> : IList<T>, IEquatable<Vector2<T>> where T : unmanaged, IBinaryInteger<T> {
    public T X;
    public T Y;

    public Vector2() { }

    public Vector2(T value) => X = Y = value;

    public Vector2(T x, T y) {
        X = x;
        Y = y;
    }

    public Vector2(IEnumerator<T> enumerator) {
        if (!enumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(enumerator), enumerator, null);
        X = enumerator.Current;
        if (!enumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(enumerator), enumerator, null);
        Y = enumerator.Current;
        if (!enumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(enumerator), enumerator, null);
    }

    public Vector2(IEnumerable<T> enumerable) : this(enumerable.GetEnumerator()) { }

    public IEnumerator<T> GetEnumerator() {
        yield return X;
        yield return Y;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => Equals(item, X) || Equals(item, Y);

    public void CopyTo(T[] array, int arrayIndex) {
        if (arrayIndex + 2 > array.Length)
            throw new ArgumentException(null, nameof(array));
        array[arrayIndex + 0] = X;
        array[arrayIndex + 1] = Y;
    }

    public void CopyTo(Span<T> span) {
        if (span.Length < 2)
            throw new ArgumentException(null, nameof(span));
        span[0] = X;
        span[1] = Y;
    }

    public bool Remove(T item) => throw new NotSupportedException();

    public int Count => 3;
    public bool IsReadOnly => false;

    public int IndexOf(T item) {
        if (Equals(item, X))
            return 0;
        if (Equals(item, Y))
            return 1;
        return -1;
    }

    public void Insert(int index, T item) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();

    public T this[int index] {
        get => index switch {
            0 => X,
            1 => Y,
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }

    public bool Equals(Vector2<T> other) =>
        X.Equals(other.X) && Y.Equals(other.Y);

    public override bool Equals(object? obj) => obj is Vector2<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"<{X}, {Y}>";

    public void Deconstruct(out T x, out T y) => (x, y) = (X, Y);
    
    public Vector2<T2> CastTruncating<T2>() where T2 : unmanaged, IBinaryInteger<T2> =>
        new(T2.CreateTruncating(X), T2.CreateTruncating(Y));

    public Vector2<T2> CastSaturating<T2>() where T2 : unmanaged, IBinaryInteger<T2> =>
        new(T2.CreateSaturating(X), T2.CreateSaturating(Y));
    
    public Vector2<T2> CastChecked<T2>() where T2 : unmanaged, IBinaryInteger<T2> =>
        new(T2.CreateChecked(X), T2.CreateChecked(Y));
    
    public static Vector2<T> operator +(Vector2<T> l) => new(+l.X, +l.Y);
    public static Vector2<T> operator -(Vector2<T> l) => new(-l.X, -l.Y);
    public static Vector2<T> operator ~(Vector2<T> l) => new(~l.X, ~l.Y);
    public static Vector2<T> operator +(Vector2<T> l, Vector2<T> r) => new(l.X + r.X, l.Y + r.Y);
    public static Vector2<T> operator -(Vector2<T> l, Vector2<T> r) => new(l.X - r.X, l.Y - r.Y);
    public static Vector2<T> operator *(Vector2<T> l, Vector2<T> r) => new(l.X * r.X, l.Y * r.Y);
    public static Vector2<T> operator /(Vector2<T> l, Vector2<T> r) => new(l.X / r.X, l.Y / r.Y);
    public static Vector2<T> operator %(Vector2<T> l, Vector2<T> r) => new(l.X % r.X, l.Y % r.Y);
    public static Vector2<T> operator &(Vector2<T> l, Vector2<T> r) => new(l.X & r.X, l.Y & r.Y);
    public static Vector2<T> operator |(Vector2<T> l, Vector2<T> r) => new(l.X | r.X, l.Y | r.Y);
    public static Vector2<T> operator ^(Vector2<T> l, Vector2<T> r) => new(l.X ^ r.X, l.Y ^ r.Y);
    public static Vector2<T> operator << (Vector2<T> l, Vector2<int> r) => new(l.X << r.X, l.Y << r.Y);
    public static Vector2<T> operator >> (Vector2<T> l, Vector2<int> r) => new(l.X >> r.X, l.Y >> r.Y);
    public static Vector2<T> operator >>> (Vector2<T> l, Vector2<int> r) => new(l.X >>> r.X, l.Y >>> r.Y);
    public static Vector2<T> operator +(Vector2<T> l, T r) => new(l.X + r, l.Y + r);
    public static Vector2<T> operator -(Vector2<T> l, T r) => new(l.X - r, l.Y - r);
    public static Vector2<T> operator *(Vector2<T> l, T r) => new(l.X * r, l.Y * r);
    public static Vector2<T> operator /(Vector2<T> l, T r) => new(l.X / r, l.Y / r);
    public static Vector2<T> operator %(Vector2<T> l, T r) => new(l.X % r, l.Y % r);
    public static Vector2<T> operator &(Vector2<T> l, T r) => new(l.X & r, l.Y & r);
    public static Vector2<T> operator |(Vector2<T> l, T r) => new(l.X | r, l.Y | r);
    public static Vector2<T> operator ^(Vector2<T> l, T r) => new(l.X ^ r, l.Y ^ r);
    public static Vector2<T> operator << (Vector2<T> l, int r) => new(l.X << r, l.Y << r);
    public static Vector2<T> operator >> (Vector2<T> l, int r) => new(l.X >> r, l.Y >> r);
    public static Vector2<T> operator >>> (Vector2<T> l, int r) => new(l.X >>> r, l.Y >>> r);
}
