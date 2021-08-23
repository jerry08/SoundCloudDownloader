using SoundCloudDl.Methods;
using SoundCloudDl.Models.SoundCloud;
using SoundCloudDownloader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundCloudDownloader.Services
{
    public class DownloadService
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private readonly SettingsService _settingsService;

        private int _concurrentDownloadCount;

        private const int NumberOfRetries = 3;
        private const int DelayOnRetry = 1000;

        public DownloadService()
        {
            _settingsService = new SettingsService();
        }

        private async Task EnsureThrottlingAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                // Wait until other downloads finish so that the number of concurrent downloads doesn't exceed the maximum
                while (_concurrentDownloadCount >= _settingsService.MaxConcurrentDownloadCount)
                    await Task.Delay(350, cancellationToken);

                Interlocked.Increment(ref _concurrentDownloadCount);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task DownloadAsync(
            TrackInformation trackInformation,
            string clientId,
            string filePath,
            IProgress<double> progress = null,
            CancellationToken cancellationToken = default)
        {
            await EnsureThrottlingAsync(cancellationToken);

            try
            {
                var soundCloudManager = new SoundCloud();

                string trackName = trackInformation.Title;

                var trackMediaInformation = soundCloudManager.QueryTrackTranscodings(trackInformation.Media.Transcodings,
                    clientId);

                Uri trackMediaUrl = soundCloudManager.QueryTrackM3u8(trackMediaInformation);

                Uri mp3TrackMediaUrl = soundCloudManager.QueryTrackMp3(trackMediaUrl);
                long fileSize = await HtmlUtil.GetFileSizeFromUrl(mp3TrackMediaUrl.ToString());

                //7910.72 = 1 second (not precise. just an average)
                TimeSpan mp3Duration = TimeSpan.FromSeconds(fileSize / 7910.72);

                //string filePath = $"{dirPth}\\{trackName}.mp3";

                //File.Create(filePath);

                var ffmpegManager = new Ffmpeg();

                for (int i = 1; i <= NumberOfRetries; ++i)
                {
                    try
                    {
                        await ffmpegManager.DownloadTrack(mp3TrackMediaUrl, 
                            filePath, mp3Duration, progress);
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException();
                    }
                    catch
                    {
                        await Task.Delay(DelayOnRetry);
                    }
                }

                if (_settingsService.ShouldInjectTags)
                {
                    var trackMediaInformationMod = new TrackInformationMod()
                    {
                        artist = trackInformation.User.Username,
                        album = trackInformation.User.Username,
                        title = trackInformation.Title
                    };

                    var id3Manager = new Id3();
                    id3Manager.SetFileMetadata(filePath, trackMediaInformationMod);
                }
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentDownloadCount);
            }
        }
    }
}
