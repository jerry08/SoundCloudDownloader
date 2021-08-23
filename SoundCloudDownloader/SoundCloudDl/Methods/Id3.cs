using System.Collections.Generic;

using SoundCloudDl.Models.SoundCloud;

namespace SoundCloudDl.Methods
{
    public class Id3
    {
        public void SetFileMetadata(string filePath, TrackInformationMod trackInformation)
        {
            var fileClient = TagLib.File.Create(filePath);
            fileClient.Tag.Title = trackInformation.title;
            fileClient.Tag.Performers = (new List<string>() { trackInformation.artist }).ToArray();
            fileClient.Tag.Album = trackInformation.album;
            fileClient.Save();
        }
    }
}
