using Xamarin.Forms;
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MegaVid.Services;

namespace MegaVid
{
    public partial class MainPage : ContentPage
    {
        private double rotationAngle = 0;
        private List<string> videoFiles = new List<string>();
        private int currentVideoIndex = -1;
        private readonly BookmarkService _bookmarkService;
        private readonly HistoryService _historyService;
        private readonly MediaLibraryService _mediaLibraryService;
        private bool isControlPanelVisible = true;
        private bool isPlaying = false;
        private DeviceTimer hideControlPanelTimer;

        public MainPage()
        {
            InitializeComponent();
            _bookmarkService = new BookmarkService();
            _historyService = new HistoryService();
            _mediaLibraryService = new MediaLibraryService();
            LoadVideoFiles();
            hideControlPanelTimer = Device.StartTimer(TimeSpan.FromSeconds(5), HideControlPanel);
        }

        private void LoadVideoFiles()
        {
            var directory = "/storage/emulated/0/Movies";
            videoFiles = _mediaLibraryService.LoadVideoFiles(directory);
        }

        private async void OnSelectVideoClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a video",
                FileTypes = FilePickerFileType.Videos
            });

            if (result != null)
            {
                mediaElement.Source = result.FullPath;
                mediaElement.Play();
                isPlaying = true;
                playPauseButton.Text = "⏸";
                _historyService.AddToHistory(result.FullPath, 0);
                Device.StartTimer(TimeSpan.FromSeconds(1), UpdateProgress);
            }
        }

        private void OnPlayPauseClicked(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                mediaElement.Pause();
                playPauseButton.Text = "▶";
            }
            else
            {
                mediaElement.Play();
                playPauseButton.Text = "⏸";
                Device.StartTimer(TimeSpan.FromSeconds(1), UpdateProgress);
            }
            isPlaying = !isPlaying;
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            mediaElement.Stop();
            playPauseButton.Text = "▶";
            isPlaying = false;
        }

        private void OnRewindClicked(object sender, EventArgs e)
        {
            mediaElement.Position = mediaElement.Position - TimeSpan.FromSeconds(10);
        }

        private void OnFastForwardClicked(object sender, EventArgs e)
        {
            mediaElement.Position = mediaElement.Position + TimeSpan.FromSeconds(10);
        }

        private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            mediaElement.Volume = e.NewValue;
        }

        private async void OnRotateClicked(object sender, EventArgs e)
        {
            var currentOrientation = DeviceDisplay.MainDisplayInfo.Orientation;
            if (currentOrientation == DisplayOrientation.Portrait)
            {
                DependencyService.Get<IOrientationHandler>().SetLandscape();
            }
            else
            {
                DependencyService.Get<IOrientationHandler>().SetPortrait();
            }
        }

        private void OnPreviousVideoClicked(object sender, EventArgs e)
        {
            // Реализация для предыдущего видео
        }

        private void OnNextVideoClicked(object sender, EventArgs e)
        {
            // Реализация для следующего видео
        }

        private void OnMediaElementTapped(object sender, EventArgs e)
        {
            if (!isControlPanelVisible)
            {
                ShowControlPanel();
            }
            else
            {
                HideControlPanel();
            }
        }

        private void ShowControlPanel()
        {
            controlPanel.IsVisible = true;
            progressPanel.IsVisible = true;
            isControlPanelVisible = true;
            hideControlPanelTimer = Device.StartTimer(TimeSpan.FromSeconds(5), HideControlPanel);
        }

        private bool HideControlPanel()
        {
            controlPanel.IsVisible = false;
            progressPanel.IsVisible = false;
            isControlPanelVisible = false;
            return false; // Не продолжать таймер
        }

        private void OnAddToBookmarksClicked(object sender, EventArgs e)
        {
            if (mediaElement.Source != null)
            {
                var currentVideo = mediaElement.Source.ToString();
                var currentTime = mediaElement.Position.TotalSeconds;
                _bookmarkService.AddBookmark(currentVideo, currentTime);
                DisplayAlert("Bookmark", "Bookmark updated.", "OK");
            }
        }

        private void OnShowBookmarksClicked(object sender, EventArgs e)
        {
            var bookmarks = _bookmarkService.GetBookmarks();
            var bookmarkList = string.Join("\n", bookmarks.Select(b => $"{Path.GetFileName(b.FilePath)}"));
            DisplayAlert("Bookmarks", bookmarkList, "OK");
        }

        private void OnShowHistoryClicked(object sender, EventArgs e)
        {
            var history = _historyService.GetHistory();
            var historyList = string.Join("\n", history.Select(h => $"{Path.GetFileName(h.FilePath)} at {h.Position}"));
            DisplayAlert("History", historyList, "OK");
        }

        private void OnShowMediaLibraryClicked(object sender, EventArgs e)
        {
            var videoList = string.Join("\n", videoFiles.Select(v => Path.GetFileName(v)));
            DisplayAlert("Media Library", videoList, "OK");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (mediaElement.Source != null)
            {
                var currentVideo = mediaElement.Source.ToString();
                var currentTime = mediaElement.Position.TotalSeconds;
                _historyService.AddToHistory(currentVideo, currentTime);
            }
        }

        private bool UpdateProgress()
        {
            if (mediaElement.Source != null && mediaElement.Duration.HasValue)
            {
                var position = mediaElement.Position;
                var duration = mediaElement.Duration.Value;

                progressSlider.Value = position.TotalSeconds / duration.TotalSeconds;
                currentTimeLabel.Text = position.ToString(@"mm\:ss");
                totalTimeLabel.Text = duration.ToString(@"mm\:ss");

                if (!isPlaying)
                    return false;
            }
            return true;
        }

        private void OnProgressChanged(object sender, ValueChangedEventArgs e)
        {
            if (mediaElement.Source != null && mediaElement.Duration.HasValue)
            {
                var duration = mediaElement.Duration.Value;
                mediaElement.Position = TimeSpan.FromSeconds(duration.TotalSeconds * e.NewValue);
            }
        }
    }
}