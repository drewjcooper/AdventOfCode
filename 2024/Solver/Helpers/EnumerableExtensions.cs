namespace AdventOfCode.Helpers;

public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, T breakElement)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.SplitImpl(breakElement);
    }

    private static IEnumerable<IEnumerable<T>> SplitImpl<T>(this IEnumerable<T> source, T breakElement)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            yield return enumerator.GetNextGroup(breakElement).ToArray();
        }
    }

    private static IEnumerable<T> GetNextGroup<T>(this IEnumerator<T> enumerator, T breakElement)
    {
        do
        {
            if (Equals(enumerator.Current, breakElement))
            {
                yield break;
            }

            yield return enumerator.Current;
        }
        while (enumerator.MoveNext());
    }
}
