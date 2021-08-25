using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Gress;
using HtmlAgilityPack;
using MaterialDesignThemes.Wpf;
using SoundCloudDownloader.Common;
using SoundCloudDownloader.Models;
using SoundCloudDownloader.Services;
using SoundCloudDownloader.Utils;
using SoundCloudDownloader.Utils.Extensions;
using SoundCloudDownloader.ViewModels.Components;
using SoundCloudDownloader.Views.Dialogs;
using Tyrrrz.Extensions;

namespace SoundCloudDownloader.ViewModels
{
    public class RootViewModel : PropertyChangedBase
    {
        public ISnackbarMessageQueue Notifications { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));

        private readonly QueryService _queryService;

        public ObservableCollection<DownloadViewModel> Downloads { get; set; } = new();

        public IProgressManager ProgressManager { get; } = new ProgressManager();

        public bool IsBusy { get; set; }

        public bool IsProgressIndeterminate { get; set; }

        public string Query { get; set; }

        public string DisplayName { get; set; }

        #region Commands
        //private ICommand _processQueryCommand;
        //public ICommand ProcessQueryCommand
        //{
        //    get
        //    {
        //        return _processQueryCommand ?? (_processQueryCommand = new DelegateCommand(
        //                   s => { Task.Run(() => ProcessQuery()); },
        //                   s => CanProcessQuery));
        //    }
        //}

        private ICommand _processQueryCommand;
        public ICommand ProcessQueryCommand
        {
            get
            {
                return _processQueryCommand ??
                    (_processQueryCommand = new CommandHandler((s) => { Task.Run(() => ProcessQuery()); }, () => CanProcessQuery));
            }
        }

        private ICommand _showSettingsCommand;
        public ICommand ShowSettingsCommand
        {
            get
            {
                return _showSettingsCommand ??
                    (_showSettingsCommand = new CommandHandler((s) => ShowSettings(), () => CanExecute));
            }
        }

        private ICommand _onViewLoadedCommand;
        public ICommand OnViewLoadedCommand
        {
            get
            {
                return _onViewLoadedCommand ??
                    (_onViewLoadedCommand = new CommandHandler((s) => OnViewLoaded(), () => true));
            }
        }

        public bool CanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
                //return true / false;
                return true;
            }
        }
        #endregion

        private readonly SettingsService _settingsService;

        private readonly UpdateService _updateService;

        public RootViewModel()
        {
            _settingsService = new SettingsService();
            _updateService = new UpdateService(_settingsService);
            _queryService = new QueryService();

            // Title
            DisplayName = $"{App.Name} v{App.VersionString}";

            ProgressManager.PropertyChanged += (s, e) =>
            {
                IsProgressIndeterminate =
                    ProgressManager.IsActive && ProgressManager.Progress.IsEither(0, 1);

                OnPropertyChanged(null);
            };

            App.Current.MainWindow.Closing += delegate { OnClose(); };
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                // Check for updates
                var updateVersion = await _updateService.CheckForUpdatesAsync();
                if (updateVersion is null)
                    return;

                // Notify user of an update and prepare it
                Notifications.Enqueue($"Downloading update to {App.Name} v{updateVersion}...");
                await _updateService.PrepareUpdateAsync(updateVersion);

                // Prompt user to install update (otherwise install it when application exits)
                Notifications.Enqueue(
                    "Update has been downloaded and will be installed when you exit",
                    "INSTALL NOW", () =>
                    {
                        _updateService.FinalizeUpdate(true);
                        if (MessageBox.Show("Do you want to install it now?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information)
                            == MessageBoxResult.Yes)
                        {
                            App.Current.MainWindow.Close();
                        }
                    });
            }
            catch
            {
                // Failure to update shouldn't crash the application
                Notifications.Enqueue("Failed to perform application update");
            }
        }

        public void OnClose()
        {
            _settingsService.Save();

            // Cancel all downloads
            foreach (var download in Downloads)
                download.Cancel();

            _updateService.FinalizeUpdate(false);
        }

        void ShowSettings()
        {
            //var window = new SettingsView();
            //window.ShowDialog();
        }

        public async void OnViewLoaded()
        {
            _settingsService.Load();

            if (_settingsService.IsDarkModeEnabled)
            {
                App.SetDarkTheme();
            }
            else
            {
                App.SetLightTheme();
            }

            await CheckForUpdatesAsync();
        }

        private void EnqueueDownload(DownloadViewModel download)
        {
            // Cancel and remove downloads with the same file path
            var existingDownloads = Downloads.Where(d => d.FilePath == download.FilePath).ToArray();
            foreach (var existingDownload in existingDownloads)
            {
                existingDownload.Cancel();
                Downloads.Remove(existingDownload);
            }

            // Bind progress manager
            download.ProgressManager = ProgressManager;
            download.Start();

            Downloads.Insert(0, download);
        }

        public bool CanShowSettings => !IsBusy;

        public bool CanProcessQuery => !IsBusy && !string.IsNullOrWhiteSpace(Query);

        public async void ProcessQuery()
        {
            // Small operation weight to not offset any existing download operations
            var operation = ProgressManager.CreateOperation(0.01);

            IsBusy = true;

            try
            {
                var parsedQueries = _queryService.ParseMultilineQuery(Query!);
                var executedQueries = await _queryService.ExecuteQueriesAsync(parsedQueries, operation);

                var tracks = executedQueries.SelectMany(q => q.TrackInformations)
                    .Distinct(v => v.Id).ToArray();

                // No tracks found
                if (tracks.Length <= 0)
                {
                    MessageBox.Show("Nothing found", "Couldn't find any tracks based on the query or URL you provided");
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var model = new DownloadMultipleSetupViewModel()
                    {
                        Title = "Tracks",
                        ClientId = _queryService.ClientId,
                        Tracks = tracks.ToList(),
                        SelectedTracks = tracks.ToList(),
                    };

                    model.SelectedFormat = model.AvailableFormats[0];

                    var dialog = new DownloadMultipleSetupView();
                    dialog.DataContext = model;
                    dialog.confirmBtn.Click += (s, e) =>
                    {
                        if (model.Confirm())
                        {
                            dialog.Close();
                        }
                    };

                    dialog.ShowDialog();

                    foreach (var download in model.Downloads)
                    {
                        EnqueueDownload(download);
                    }
                });
            }
            catch (Exception ex)
            {
                //var dialog = _viewModelFactory.CreateMessageBoxViewModel(
                //    "Error",
                //    // Short error message for expected errors, full for unexpected
                //    ex is YoutubeExplodeException
                //        ? ex.Message
                //        : ex.ToString()
                //);
                //
                //await _dialogManager.ShowDialogAsync(dialog);

                MessageBox.Show(ex.Message);
            }
            finally
            {
                operation.Dispose();
                IsBusy = false;

                //operation.Report(0);
                IsProgressIndeterminate = false;
                OnPropertyChanged(null);
            }
        }

        private ICommand _removeDownloadCommand;
        public ICommand RemoveDownloadCommand
        {
            get
            {
                return _removeDownloadCommand ??
                    (_removeDownloadCommand = new CommandHandler((s) => RemoveDownload((DownloadViewModel)s), () => CanRemoveDownload));
            }
        }

        public bool CanRemoveDownload => true;

        public void RemoveDownload(DownloadViewModel download)
        {
            download.Cancel();
            Downloads.Remove(download);
        }

        private ICommand _removeInactiveDownloadsCommand;
        public ICommand RemoveInactiveDownloadsCommand
        {
            get
            {
                return _removeInactiveDownloadsCommand ??
                    (_removeInactiveDownloadsCommand = new CommandHandler((s) => RemoveInactiveDownloads(), () => CanRemoveInactiveDownloads));
            }
        }

        public bool CanRemoveInactiveDownloads => true;

        public void RemoveInactiveDownloads() =>
            Downloads.RemoveAll(d => !d.IsActive);

        private ICommand _removeSuccessfulDownloadsCommand;
        public ICommand RemoveSuccessfulDownloadsCommand
        {
            get
            {
                return _removeSuccessfulDownloadsCommand ??
                    (_removeSuccessfulDownloadsCommand = new CommandHandler((s) => RemoveSuccessfulDownloads(), () => CanRemoveSuccessfulDownloads));
            }
        }

        public bool CanRemoveSuccessfulDownloads => true;

        public void RemoveSuccessfulDownloads() =>
            Downloads.RemoveAll(d => d.IsSuccessful);

        private ICommand _restartFailedDownloadsCommand;
        public ICommand RestartFailedDownloadsCommand
        {
            get
            {
                return _restartFailedDownloadsCommand ??
                    (_restartFailedDownloadsCommand = new CommandHandler((s) => RestartFailedDownloads(), () => CanRestartFailedDownloads));
            }
        }

        public bool CanRestartFailedDownloads => true;

        public void RestartFailedDownloads()
        {
            var failedDownloads = Downloads.Where(d => d.IsFailed).ToArray();
            foreach (var failedDownload in failedDownloads)
                failedDownload.Restart();
        }
    }
}