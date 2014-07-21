using System.Collections.Generic;
using System.Data.Entity;

namespace FakeDbSet
{
    public static class IDbSetExtensions
    {
        public static void AddRange<T>(this IDbSet<T> dbSet, IEnumerable<T> items) where T : class
        {
            foreach (var item in items)
            {
                dbSet.Add(item);
            }
        }

        public static void Add<T>(this IDbSet<T> dbSet, params T[] items) where T : class
        {
            dbSet.AddRange(items);
        }
    }
}
