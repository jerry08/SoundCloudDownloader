using System.Net.Http;
using System.Net.Http.Headers;

namespace SoundCloudDownloader.Core.Utils;

internal static class Http
{
    public static HttpClient Client { get; } =
        new()
        {
            DefaultRequestHeaders =
            {
                // Required by some of the services we're using
                UserAgent =
                {
                    new ProductInfoHeaderValue(
                        "SoundCloudDownloader",
                        typeof(Http).Assembly.GetName().Version?.ToString(3)
                    )
                }
            }
        };
}
