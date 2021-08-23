using System;
using Newtonsoft.Json;

namespace SoundCloudDl.Models.SoundCloud
{
    public partial class TrackInformation
    {
        [JsonProperty("artwork_url")]
        public Uri ArtworkUrl { get; set; }

        [JsonProperty("caption")]
        public object Caption { get; set; }

        [JsonProperty("commentable")]
        public bool Commentable { get; set; }

        [JsonProperty("comment_count")]
        public string CommentCount { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("downloadable")]
        public bool Downloadable { get; set; }

        [JsonProperty("download_count")]
        public long DownloadCount { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("full_duration")]
        public long FullDuration { get; set; }

        [JsonProperty("embeddable_by")]
        public string EmbeddableBy { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("has_downloads_left")]
        public bool HasDownloadsLeft { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("label_name")]
        public object LabelName { get; set; }

        [JsonProperty("last_modified")]
        public DateTimeOffset LastModified { get; set; }

        [JsonProperty("license")]
        public string License { get; set; }

        [JsonProperty("likes_count")]
        public long LikesCount { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("permalink_url")]
        public Uri PermalinkUrl { get; set; }

        [JsonProperty("playback_count")]
        public long PlaybackCount { get; set; }

        [JsonProperty("public")]
        public bool Public { get; set; }

        [JsonProperty("publisher_metadata")]
        public PublisherMetadata PublisherMetadata { get; set; }

        [JsonProperty("purchase_title")]
        public object PurchaseTitle { get; set; }

        [JsonProperty("purchase_url")]
        public object PurchaseUrl { get; set; }

        [JsonProperty("release_date")]
        public object ReleaseDate { get; set; }

        [JsonProperty("reposts_count")]
        public long RepostsCount { get; set; }

        [JsonProperty("secret_token")]
        public object SecretToken { get; set; }

        [JsonProperty("sharing")]
        public string Sharing { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("streamable")]
        public bool Streamable { get; set; }

        [JsonProperty("tag_list")]
        public string TagList { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("urn")]
        public string Urn { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("visuals")]
        public object Visuals { get; set; }

        [JsonProperty("waveform_url")]
        public Uri WaveformUrl { get; set; }

        [JsonProperty("display_date")]
        public DateTimeOffset DisplayDate { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("monetization_model")]
        public string MonetizationModel { get; set; }

        [JsonProperty("policy")]
        public string Policy { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

    public partial class Media
    {
        [JsonProperty("transcodings")]
        public Transcoding[] Transcodings { get; set; }
    }

    public partial class Transcoding
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("preset")]
        public string Preset { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("snipped")]
        public bool Snipped { get; set; }

        [JsonProperty("format")]
        public Format Format { get; set; }

        [JsonProperty("quality")]
        public string Quality { get; set; }
    }

    public partial class Format
    {
        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
    }

    public partial class PublisherMetadata
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("urn")]
        public string Urn { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("contains_music")]
        public bool ContainsMusic { get; set; }
    }

    public partial class User
    {
        [JsonProperty("avatar_url")]
        public Uri AvatarUrl { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("last_modified")]
        public DateTimeOffset LastModified { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("permalink_url")]
        public Uri PermalinkUrl { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("urn")]
        public string Urn { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("badges")]
        public Badges Badges { get; set; }
    }

    public partial class Badges
    {
        [JsonProperty("pro_unlimited")]
        public bool ProUnlimited { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }
    }
}
