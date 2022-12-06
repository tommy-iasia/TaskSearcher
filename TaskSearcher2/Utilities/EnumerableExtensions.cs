namespace TaskSearcher2.Utilities
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> items)
            => items.Select((item, index) => (item, index));
    }
}
