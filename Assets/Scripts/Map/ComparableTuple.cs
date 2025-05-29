using System;
using UnityEngine;
[Serializable]
public class ComparableTuple<T, U> : IComparable<ComparableTuple<T, U>> where T : IComparable<T> where U : IComparable<U>
{
    public T Item1;
    public U Item2;
    public int CompareTo(ComparableTuple<T, U> other)
    {
        int result = 0;
        result += Item1.CompareTo(other.Item1) * 3;
        result += Item2.CompareTo(other.Item2);
        return result;
    }

    public ComparableTuple(T item1, U item2)
    {
        Item1 = item1;
        Item2 = item2;
    }
}
