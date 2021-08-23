using Newtonsoft.Json;

namespace SoundCloudDl.Models.SoundCloud
{
    public partial class TrackMediaInformation
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
