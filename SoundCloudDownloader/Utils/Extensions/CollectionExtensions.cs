using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloudDownloader.Utils.Extensions
{
    internal static class CollectionExtensions
    {
        public static void RemoveAll<T>(this ObservableCollection<T> collection, 
            Func<T, bool> condition)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (condition(collection[i]))
                {
                    collection.RemoveAt(i);
                }
            }
        }

        public static void RemoveWhere<T>(this ICollection<T> source, Predicate<T> predicate)
        {
            foreach (var i in source.ToArray())
            {
                if (predicate(i))
                    source.Remove(i);
            }
        }
    }
}
