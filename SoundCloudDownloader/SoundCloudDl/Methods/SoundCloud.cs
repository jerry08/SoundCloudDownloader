using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using SoundCloudDl.Models.SoundCloud;
using SoundCloudDl.Models.Playlist;
using SoundCloudDownloader.Utils;

namespace SoundCloudDl.Methods
{
    public class SoundCloud
    {
        private string resolveEndpoint { get; } = "https://api-v2.SoundCloud.com/resolve";
        private string trackEndpoint { get; } = "https://api-v2.SoundCloud.com/tracks";

        public TrackInformation ResolveTrackUrl(Uri trackUrl, string clientId)
        {
            return JsonConvert.DeserializeObject<TrackInformation>(ResolveSoundcloudUrl(trackUrl, clientId));
        }

        public PlaylistInformation ResolvePlaylistUrl(Uri playlistUrl, string clientId)
        {
            return JsonConvert.DeserializeObject<PlaylistInformation>(ResolveSoundcloudUrl(playlistUrl, clientId));
        }

        private string ResolveSoundcloudUrl(Uri soundcloudUrl, string clientId)
        {
            var httpManager = new Http();
            return httpManager.GetHttpResponseString(resolveEndpoint,
                $"?url={soundcloudUrl}&client_id={clientId}");
        }

        //Select at least one working transcoding
        public Uri QueryTrackTranscodings(Models.SoundCloud.Transcoding[] trackTranscodings, string clientId)
        {
            var trackUrl = trackTranscodings
                .Where(x => x.Quality == "sq" && x.Format.MimeType.Contains("ogg") && x.Format.Protocol == "hls").FirstOrDefault()
                .Url;
            return new Uri(trackUrl + $"?client_id={clientId}");
        }

        public Uri QueryTrackM3u8(Uri trackTranscoding)
        {
            var httpManager = new Http();
            var trackMedia = httpManager.GetHttpResponseString(trackTranscoding.ToString());
            return new Uri(JsonConvert.DeserializeObject<TrackMediaInformation>(trackMedia).Url);
        }

        public Uri QueryTrackMp3(Uri trackM3u8)
        {
            var html = HtmlUtil.GetHtml(trackM3u8.ToString());
            var m3u8 = html.Split(',');

            string link = "";

            var last_stream = m3u8.LastOrDefault().Split('/');
            for (int i = 0; i < last_stream.Length; i++)
            {
                if (last_stream[i] == "media")
                {
                    last_stream[i + 1] = "0";
                    link = string.Join('/', last_stream).Split('\n')[1];
                }
            }

            return new Uri(link);
        }

        public Uri QueryTrackUrl(long trackId, string clientId)
        {
            var httpManager = new Http();
            var trackInformation = httpManager.GetHttpResponseString(trackEndpoint, $"/{trackId}?client_id={clientId}");
            return JsonConvert.DeserializeObject<TrackInformation>(trackInformation).PermalinkUrl;
        }
    }
}
