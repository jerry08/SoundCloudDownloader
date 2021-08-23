using SoundCloudDl.Models.SoundCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCloudDownloader.Models
{
    public class ExecutedQuery
    {
        public Query Query { get; }

        public IReadOnlyList<TrackInformation> TrackInformations { get; }

        public ExecutedQuery(Query query, IReadOnlyList<TrackInformation> trackInformations)
        {
            Query = query;
            TrackInformations = trackInformations;
        }
    }
}
