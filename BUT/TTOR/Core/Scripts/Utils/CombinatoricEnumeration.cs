using System.Collections.Generic;
using System.Linq;

namespace BUT.TTOR.Core.Utils
{
    public static class CombinatoricEnumeration
    {
        public static IEnumerable<T[]> Combinations<T>(this IEnumerable<T> items, int r)
        {
            int n = items.Count();

            if (r > n) yield break;

            T[] pool = items.ToArray();
            int[] indices = Enumerable.Range(0, r).ToArray();

            T[] combination = new T[r];
            for (int j = 0; j < r; j++)
            {
                combination[j] = pool[indices[j]];
            }
            yield return combination;

            while (true)
            {
                int i = indices.Length - 1;
                while (i >= 0 && indices[i] == i + n - r)
                    i -= 1;

                if (i < 0) yield break;

                indices[i] += 1;

                for (int j = i + 1; j < r; j += 1)
                    indices[j] = indices[j - 1] + 1;

                for (int j = 0; j < r; j++)
                {
                    combination[j] = pool[indices[j]];
                }
                yield return combination;
            }
        }
    }
}
