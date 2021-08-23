using SoundCloudDl.Models.SoundCloud;

namespace SoundCloudDownloader.Utils
{
    internal static class FileNameGenerator
    {
        private static string NumberToken { get; } = "$num";

        private static string TitleToken { get; } = "$title";

        public static string DefaultTemplate { get; } = $"{TitleToken}";

        public static string GenerateFileName(
            string template,
            TrackInformation track,
            string format,
            string number = null)
        {
            var result = template;

            result = result.Replace(NumberToken, !string.IsNullOrWhiteSpace(number) ? $"[{number}]" : "");
            result = result.Replace(TitleToken, track.Title);

            result = result.Trim();

            result += $".{format}";

            return PathEx.EscapeFileName(result);
        }
    }
}