using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace DdsManipLib.Utilities;

public struct Vector4<T> : IList<T>, IEquatable<Vector4<T>> where T : unmanaged, IBinaryNumber<T> {
    public T X;
    public T Y;
    public T Z;
    public T W;

    public Vector4() { }

    public Vector4(T value) => X = Y = Z = W = value;

    public Vector4(Vector2<T> v2, T z, T w) {
        X = v2.X;
        Y = v2.Y;
        Z = z;
        W = w;
    }

    public Vector4(Vector3<T> v2, T w) {
        X = v2.X;
        Y = v2.Y;
        Z = v2.Z;
        W = w;
    }

    public Vector4(T x, T y, T z, T w) {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Vector4(IEnumerator<T> enumerator) {
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
        W = enumerator.Current;
        if (enumerator.MoveNext())
            throw new ArgumentOutOfRangeException(nameof(enumerator), enumerator, null);
    }

    public Vector4(IEnumerable<T> enumerable) : this(enumerable.GetEnumerator()) { }

    public IEnumerator<T> GetEnumerator() {
        yield return X;
        yield return Y;
        yield return Z;
        yield return W;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => Equals(item, X) || Equals(item, Y) || Equals(item, Z) || Equals(item, W);

    public void CopyTo(T[] array, int arrayIndex) {
        if (arrayIndex + 4 > array.Length)
            throw new ArgumentException(null, nameof(array));
        array[arrayIndex + 0] = X;
        array[arrayIndex + 1] = Y;
        array[arrayIndex + 2] = Z;
        array[arrayIndex + 3] = W;
    }

    public void CopyTo(Span<T> span) {
        if (span.Length < 4)
            throw new ArgumentException(null, nameof(span));
        span[0] = X;
        span[1] = Y;
        span[2] = Z;
        span[3] = W;
    }

    public bool Remove(T item) => throw new NotSupportedException();

    public int Count => 4;
    public bool IsReadOnly => false;

    public int IndexOf(T item) {
        if (Equals(item, X))
            return 0;
        if (Equals(item, Y))
            return 1;
        if (Equals(item, Z))
            return 2;
        if (Equals(item, W))
            return 3;
        return -1;
    }

    public void Insert(int index, T item) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();

    public T this[int index] {
        get => index switch {
            0 => X,
            1 => Y,
            2 => Z,
            3 => W,
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
                case 3:
                    W = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }

    public bool Equals(Vector4<T> other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    public override bool Equals(object? obj) => obj is Vector4<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    public override string ToString() => $"<{X}, {Y}, {Z}, {W}>";

    public void Deconstruct(out T x, out T y, out T z, out T w) => (x, y, z, w) = (X, Y, Z, W);

    public Vector4<T2> CastTruncating<T2>() where T2 : unmanaged, IBinaryInteger<T2> =>
        new(T2.CreateTruncating(X), T2.CreateTruncating(Y), T2.CreateTruncating(Z), T2.CreateTruncating(W));

    public Vector4<T2> CastSaturating<T2>() where T2 : unmanaged, IBinaryInteger<T2> =>
        new(T2.CreateSaturating(X), T2.CreateSaturating(Y), T2.CreateSaturating(Z), T2.CreateSaturating(W));
    
    public Vector4<T2> CastChecked<T2>() where T2 : unmanaged, IBinaryInteger<T2> =>
        new(T2.CreateChecked(X), T2.CreateChecked(Y), T2.CreateChecked(Z), T2.CreateChecked(W));

    public static Vector4<T> operator +(Vector4<T> l) => new(+l.X, +l.Y, +l.Z, +l.W);
    public static Vector4<T> operator -(Vector4<T> l) => new(-l.X, -l.Y, -l.Z, -l.W);
    public static Vector4<T> operator ~(Vector4<T> l) => new(~l.X, ~l.Y, ~l.Z, ~l.W);
    public static Vector4<T> operator +(Vector4<T> l, Vector4<T> r) => new(l.X + r.X, l.Y + r.Y, l.Z + r.Z, l.W + r.W);
    public static Vector4<T> operator -(Vector4<T> l, Vector4<T> r) => new(l.X - r.X, l.Y - r.Y, l.Z - r.Z, l.W - r.W);
    public static Vector4<T> operator *(Vector4<T> l, Vector4<T> r) => new(l.X * r.X, l.Y * r.Y, l.Z * r.Z, l.W * r.W);
    public static Vector4<T> operator /(Vector4<T> l, Vector4<T> r) => new(l.X / r.X, l.Y / r.Y, l.Z / r.Z, l.W / r.W);
    public static Vector4<T> operator %(Vector4<T> l, Vector4<T> r) => new(l.X % r.X, l.Y % r.Y, l.Z % r.Z, l.W % r.W);
    public static Vector4<T> operator &(Vector4<T> l, Vector4<T> r) => new(l.X & r.X, l.Y & r.Y, l.Z & r.Z, l.W & r.W);
    public static Vector4<T> operator |(Vector4<T> l, Vector4<T> r) => new(l.X | r.X, l.Y | r.Y, l.Z | r.Z, l.W | r.W);
    public static Vector4<T> operator ^(Vector4<T> l, Vector4<T> r) => new(l.X ^ r.X, l.Y ^ r.Y, l.Z ^ r.Z, l.W ^ r.W);
    public static Vector4<T> operator +(Vector4<T> l, T r) => new(l.X + r, l.Y + r, l.Z + r, l.W + r);
    public static Vector4<T> operator -(Vector4<T> l, T r) => new(l.X - r, l.Y - r, l.Z - r, l.W - r);
    public static Vector4<T> operator *(Vector4<T> l, T r) => new(l.X * r, l.Y * r, l.Z * r, l.W * r);
    public static Vector4<T> operator /(Vector4<T> l, T r) => new(l.X / r, l.Y / r, l.Z / r, l.W / r);
    public static Vector4<T> operator %(Vector4<T> l, T r) => new(l.X % r, l.Y % r, l.Z % r, l.W % r);
    public static Vector4<T> operator &(Vector4<T> l, T r) => new(l.X & r, l.Y & r, l.Z & r, l.W & r);
    public static Vector4<T> operator |(Vector4<T> l, T r) => new(l.X | r, l.Y | r, l.Z | r, l.W | r);
    public static Vector4<T> operator ^(Vector4<T> l, T r) => new(l.X ^ r, l.Y ^ r, l.Z ^ r, l.W ^ r);
}
