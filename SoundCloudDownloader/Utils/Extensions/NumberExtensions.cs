namespace SoundCloudDownloader.Utils.Extensions;

public static class NumberExtensions
{
    public static string ToKiloFormat(this int value, bool applyToThousand = true)
    {
        if (value >= 1000000000)
            return (value / 1000000000D).ToString("0.#") + "B";

        if (value >= 100000000)
            return (value / 1000000D).ToString("#,0M");

        if (value >= 1000000)
            return (value / 1000000D).ToString("0.#") + "M";

        if (value >= 100000)
            return (value / 1000D).ToString("#,0K");

        if (applyToThousand)
        {
            if (value >= 1000)
                return (value / 1000D).ToString("0.#") + "K";
        }
        else
        {
            if (value >= 10000)
                return (value / 1000D).ToString("0.#") + "K";
        }

        return value.ToString("#,0");
    }
}
