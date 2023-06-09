using System.Collections;
using System.Collections.Generic;

namespace Project1;

public static class ListExtensions
{
    public static void Replace<T>(this ICollection<T> input, ICollection<T> newItems)
    {
        if (input == null || newItems == null) { return; } input.Clear(); foreach (var item in newItems) { input.Add(item); }
    }
}