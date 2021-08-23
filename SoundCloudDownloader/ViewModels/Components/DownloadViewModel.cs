using Gress;
using SoundCloudDl.Models.SoundCloud;
using SoundCloudDownloader.Common;
using SoundCloudDownloader.Services;
using SoundCloudDownloader.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoundCloudDownloader.ViewModels.Components
{
    public class DownloadViewModel : PropertyChangedBase
    {
        public string ClientId { get; set; }

        private DownloadService _downloadService = new DownloadService();

        private CancellationTokenSource _cancellationTokenSource;

        public TrackInformation Track { get; set; } = default!;

        public string FilePath { get; set; } = default!;

        public string FileName => Path.GetFileName(FilePath);

        public string Format { get; set; } = default!;

        public IProgressManager ProgressManager { get; set; }

        public IProgressOperation ProgressOperation { get; private set; }

        public bool IsActive { get; private set; }

        public bool IsSuccessful { get; private set; }

        public bool IsCanceled { get; private set; }

        public bool IsFailed { get; private set; }

        public string FailReason { get; private set; }

        public bool CanStart => !IsActive;

        public void Start()
        {
            if (!CanStart)
                return;

            IsActive = true;
            IsSuccessful = false;
            IsCanceled = false;
            IsFailed = false;

            Task.Run(async () =>
            {
                _cancellationTokenSource = new CancellationTokenSource();
                ProgressOperation = ProgressManager?.CreateOperation();

                try
                {
                    //Download
                    await _downloadService.DownloadAsync(Track,
                        ClientId, FilePath, ProgressOperation, _cancellationTokenSource.Token);

                    IsSuccessful = true;
                }
                catch (OperationCanceledException)
                {
                    IsCanceled = true;
                }
                catch (Exception ex)
                {
                    IsFailed = true;

                    FailReason = ex.Message;
                }
                finally
                {
                    IsActive = false;
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = null;
                    ProgressOperation?.Dispose();

                    //Update "IsSuccessful" property
                    //OnPropertyChanged(nameof(IsSuccessful));

                    //Update all properties
                    OnPropertyChanged(null);
                }
            });
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ??
                    (_cancelCommand = new CommandHandler((s) => Cancel(), () => CanCancel));
            }
        }

        public bool CanCancel => IsActive && !IsCanceled;

        public void Cancel()
        {
            if (!CanCancel)
                return;

            _cancellationTokenSource?.Cancel();
        }

        private ICommand _showFileCommand;
        public ICommand ShowFileCommand
        {
            get
            {
                return _showFileCommand ??
                    (_showFileCommand = new CommandHandler((s) => ShowFile(), () => CanShowFile));
            }
        }

        public bool CanShowFile => IsSuccessful;

        public void ShowFile()
        {
            if (!CanShowFile)
                return;

            // Open explorer, navigate to the output directory and select the file
            Process.Start("explorer", $"/select, \"{FilePath}\"");
        }

        private ICommand _openFileCommand;
        public ICommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ??
                    (_openFileCommand = new CommandHandler((s) => OpenFile(), () => CanOpenFile));
            }
        }

        public bool CanOpenFile => IsSuccessful;

        public void OpenFile()
        {
            if (!CanOpenFile)
                return;

            ProcessEx.StartShellExecute(FilePath);
        }

        private ICommand _restartCommand;
        public ICommand RestartCommand
        {
            get
            {
                return _restartCommand ??
                    (_restartCommand = new CommandHandler((s) => Restart(), () => CanRestart));
            }
        }

        public bool CanRestart => CanStart && !IsSuccessful;

        public void Restart() => Start();
    }
}