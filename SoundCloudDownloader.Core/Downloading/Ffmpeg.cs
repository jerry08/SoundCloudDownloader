using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using FFMpegCore;
using FFMpegCore.Enums;

namespace SoundCloudDownloader.Core.Downloading;

public class Ffmpeg
{
    /*public async Task DownloadTrack(Uri trackMediaUrl, string trackFilePath)
    {
        await FFMpegArguments.FromUrlInput(trackMediaUrl)
            .OutputToFile(trackFilePath, true, options => options
                .WithAudioCodec(AudioCodec.LibMp3Lame)
                .UsingMultithreading(true)
                .WithAudioBitrate(AudioQuality.VeryHigh))
            .ProcessAsynchronously();
    }*/

    public static async Task DownloadTrack(Uri trackMediaUrl,
        string trackFilePath, TimeSpan? duration = null,
        IProgress<double>? progress = null)
    {
        //var analysis = FFProbe.Analyse(trackMediaUrl);
        //var dur = analysis.Duration;

        string fileExt = Path.GetExtension(trackFilePath);

        var audioCodec = AudioCodec.LibMp3Lame;

        switch (fileExt.ToLower())
        {
            case ".mp3":
                audioCodec = AudioCodec.LibMp3Lame;

                //WebClient client = new WebClient();
                //client.DownloadProgressChanged += (s, e) =>
                //{
                //    if (e.ProgressPercentage <= 100)
                //    {
                //        progress.Report(e.ProgressPercentage / 100);
                //    }
                //};
                //await client.DownloadFileTaskAsync(trackMediaUrl, trackFilePath);
                //return;

                break;
            case ".ogg":
                audioCodec = AudioCodec.LibVorbis;
                break;
            case ".m4a":
                audioCodec = AudioCodec.Aac;
                break;
            default:
                audioCodec = AudioCodec.LibMp3Lame;
                break;
        }

        var processor = FFMpegArguments.FromUrlInput(trackMediaUrl)
            .OutputToFile(trackFilePath, true, options => options
                .WithAudioCodec(audioCodec)
                .UsingMultithreading(true)
                .WithAudioBitrate(AudioQuality.VeryHigh));

        if (duration != null && progress != null)
        {
            processor.NotifyOnProgress((percentageProgress) =>
            {
                if (percentageProgress <= 100)
                {
                    progress.Report(percentageProgress / 100);
                }

                //if (percentageProgress != double.NaN 
                //    && percentageProgress != double.NegativeInfinity 
                //    && percentageProgress != double.PositiveInfinity)
                //{
                //
                //}
            }, duration.Value);
        }

        await processor.ProcessAsynchronously();
    }
}