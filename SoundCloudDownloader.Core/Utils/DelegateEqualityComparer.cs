using System;
using System.Collections.Generic;

namespace SoundCloudDownloader.Core.Utils;

internal class DelegateEqualityComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
    : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (ReferenceEquals(x, null))
            return false;
        if (ReferenceEquals(y, null))
            return false;
        if (x.GetType() != y.GetType())
            return false;

        return equals(x, y);
    }

    public int GetHashCode(T obj) => getHashCode(obj);
}
