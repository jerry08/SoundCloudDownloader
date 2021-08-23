using Ookii.Dialogs.Wpf;
using SoundCloudDl.Models.SoundCloud;
using SoundCloudDownloader.Common;
using SoundCloudDownloader.Services;
using SoundCloudDownloader.Utils;
using SoundCloudDownloader.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SoundCloudDownloader.ViewModels
{
    public class DownloadMultipleSetupViewModel
    {
        SettingsService _settingsService = new SettingsService();

        public string Title { get; set; } = default!;

        public string ClientId { get; set; } = default!;

        public IReadOnlyList<TrackInformation> Tracks { get; set; }

        public IReadOnlyList<TrackInformation> SelectedTracks { get; set; } = new List<TrackInformation>();

        public IReadOnlyList<string> AvailableFormats { get; set; } = new[] { "mp3", "m4a", "ogg" };

        public string SelectedFormat { get; set; }

        public void CopyTitle() => Clipboard.SetText(Title);

        public List<DownloadViewModel> Downloads = new List<DownloadViewModel>();

        //private ICommand _confirmCommand;
        //public ICommand ConfirmCommand
        //{
        //    get
        //    {
        //        return _confirmCommand ?? (_confirmCommand = new DelegateCommand(
        //                   s => Confirm(),
        //                   s => true));
        //    }
        //}

        public bool Confirm()
        {
            // Prompt for output directory path
            var dirPath = PromptDirectoryPath();
            if (string.IsNullOrWhiteSpace(dirPath))
                return false;

            _settingsService.LastFormat = SelectedFormat;

            // Make sure selected tracks are ordered in the same way as available tracks
            var orderedSelectedTracks = Tracks.Where(v => SelectedTracks.Contains(v)).ToArray();

            var downloads = new List<DownloadViewModel>();
            for (var i = 0; i < orderedSelectedTracks.Length; i++)
            {
                var track = orderedSelectedTracks[i];

                var fileName = FileNameGenerator.GenerateFileName(
                    _settingsService.FileNameTemplate,
                    track,
                    SelectedFormat!,
                    (i + 1).ToString().PadLeft(orderedSelectedTracks.Length.ToString().Length, '0')
                );

                var filePath = Path.Combine(dirPath, fileName);

                // If file exists or is no empty - either skip it or generate a unique file path, depending on user settings
                var fileInfo = new FileInfo(filePath);

                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    if (_settingsService.ShouldSkipExistingFiles)
                        continue;

                    filePath = PathEx.MakeUniqueFilePath(filePath);
                }

                // Create empty file to "lock in" the file path.
                // This is necessary as there may be other downloads with the same file name
                // which would otherwise overwrite the file.
                PathEx.CreateDirectoryForFile(filePath);
                PathEx.CreateEmptyFile(filePath);

                var download = new DownloadViewModel()
                {
                    ClientId = ClientId,
                    FilePath = filePath,
                    Track = track
                };

                downloads.Add(download);
            }

            Downloads = downloads;
            //Close(downloads);

            return true;
        }

        public string PromptDirectoryPath(string defaultDirPath = "")
        {
            // Create dialog
            var dialog = new VistaFolderBrowserDialog
            {
                SelectedPath = defaultDirPath
            };

            // Show dialog and return result
            return dialog.ShowDialog() == true ? dialog.SelectedPath : null;
        }
    }
}
