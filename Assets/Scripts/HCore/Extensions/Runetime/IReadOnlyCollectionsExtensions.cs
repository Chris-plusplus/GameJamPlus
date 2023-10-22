using System.Collections.Generic;

namespace HCore.Extensions
{
    public static class IReadOnlyCollectionsExtensions
    {
        public static bool Contains<T>(this IReadOnlyCollection<T> list, T item)
        {
            foreach (var s in list)
            {
                if (ReferenceEquals(s, item))
                    return true;
            }
            return false;
        }
    }
}