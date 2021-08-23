using HtmlAgilityPack;
using SoundCloudDl.Methods;
using SoundCloudDl.Models.SoundCloud;
using SoundCloudDownloader.Models;
using SoundCloudDownloader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloudDownloader.Services
{
    public class QueryService
    {
        public string ClientId;

        public Query ParseQuery(string query)
        {
            query = query.Trim();

            // Playlist
            if (query.Contains("/sets/"))
            {
                return new Query(QueryKind.Playlist, query);
            }
            // Single track
            else
            {
                return new Query(QueryKind.Track, query);
            }
        }

        public IReadOnlyList<Query> ParseMultilineQuery(string query) =>
            query.Split(Environment.NewLine).Select(ParseQuery).ToArray();

        public async Task<ExecutedQuery> ExecuteQueryAsync(Query query)
        {
            if (query.Kind == QueryKind.Playlist)
            {
                var soundCloudManager = new SoundCloud();

                var list = new List<TrackInformation>();

                var playlistInformation = soundCloudManager.ResolvePlaylistUrl(new Uri(query.Value), ClientId);
                foreach (var track in playlistInformation.Tracks)
                {
                    var trackUrl = soundCloudManager.QueryTrackUrl(track.Id, ClientId);

                    var trackInformation = soundCloudManager.ResolveTrackUrl(trackUrl, ClientId);

                    list.Add(trackInformation);
                }

                return new ExecutedQuery(query, list);
            }
            else
            {
                var soundCloudManager = new SoundCloud();

                var trackInformation = soundCloudManager.ResolveTrackUrl(new Uri(query.Value), ClientId);

                return new ExecutedQuery(query, new[] { trackInformation });
            }

            //throw new ArgumentException($"Could not parse query '{query}'.", nameof(query));
        }

        public async Task<IReadOnlyList<ExecutedQuery>> ExecuteQueriesAsync(
            IReadOnlyList<Query> queries,
            IProgress<double> progress = null)
        {
            if (string.IsNullOrEmpty(ClientId))
            {
                var document = new HtmlDocument();
                string html = await HtmlUtil.GetHtmlAsync("https://SoundCloud.com");
                document.LoadHtml(html);

                var tt = document.DocumentNode.Descendants()
                    .Where(x => x.Name == "script").ToList();

                var script_url = tt.LastOrDefault().Attributes["src"].Value;

                html = await HtmlUtil.GetHtmlAsync(script_url);

                ClientId = html.Split(",client_id")[1].Split('"')[1];
            }

            var result = new List<ExecutedQuery>(queries.Count);

            for (var i = 0; i < queries.Count; i++)
            {
                var executedQuery = await ExecuteQueryAsync(queries[i]);
                result.Add(executedQuery);

                progress?.Report((i + 1.0) / queries.Count);
            }

            return result;
        }
    }
}