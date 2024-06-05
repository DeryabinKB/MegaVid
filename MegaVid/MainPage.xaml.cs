using Xamarin.Forms;
using Xamarin.Essentials;
using System;
using MegaVid.Helpers;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.UI.Views;
using MegaVid.Services;

namespace MegaVid
{
    public partial class MainPage : ContentPage
    {
        private readonly VideoHelper _videoHelper;
        private readonly VideoControlHelper _videoControlHelper;
        private readonly System.Timers.Timer _hideControlPanelTimer;
        private Slider volumeSlider;
        private readonly System.Timers.Timer _checkVideoPositionTimer;

        public MainPage()
        {
            InitializeComponent();
            _videoHelper = new VideoHelper(mediaElement);
            _videoControlHelper = new VideoControlHelper(mediaElement, playPauseButton, volumeSlider, progressSlider, currentTimeLabel, totalTimeLabel, controlPanel, progressPanel, rotateButton, sidePanel);

            _hideControlPanelTimer = new System.Timers.Timer(5000);
            _hideControlPanelTimer.Elapsed += (sender, args) => HideControlPanel();
            _hideControlPanelTimer.AutoReset = false;

            _checkVideoPositionTimer = new System.Timers.Timer(5000);
            _checkVideoPositionTimer.Elapsed += (sender, args) => _videoControlHelper.CheckVideoPositionAndSize();
            _checkVideoPositionTimer.Start();

            LoadVideoFiles();
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnMediaElementTapped;
            mainGrid.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void LoadVideoFiles()
        {
            var directory = "/storage/emulated/0/Movies";
            _videoHelper.LoadVideoFiles(directory);
        }

        private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            _videoControlHelper.AdjustControlPanelPosition(e.DisplayInfo.Orientation);
            _videoControlHelper.AdjustVideoSize(e.DisplayInfo.Orientation); // Добавляем этот вызов
        }

        private void OnClearHistoryClicked(object sender, EventArgs e)
        {
            _videoHelper.ClearHistory();
        }

        private async void OnSelectVideoClicked(object sender, EventArgs e)
        {
            await _videoControlHelper.SelectVideo();
        }

        private void OnPlayPauseClicked(object sender, EventArgs e)
        {
            _videoControlHelper.TogglePlayPause();
            ResetControlPanelTimer();
        }

        private void OnRewindClicked(object sender, EventArgs e)
        {
            _videoControlHelper.Rewind();
            ResetControlPanelTimer();
        }

        private void OnFastForwardClicked(object sender, EventArgs e)
        {
            _videoControlHelper.FastForward();
            ResetControlPanelTimer();
        }

        private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            _videoControlHelper.ChangeVolume(e.NewValue);
            ResetControlPanelTimer();
        }

        private async void OnRotateClicked(object sender, EventArgs e)
        {
            await _videoControlHelper.Rotate();
            //controlPanel.Margin = 25;
            //controlPanel.Padding = 25;
            //controlPanel = controlPanel;
            ResetControlPanelTimer();
        }

        private void OnPreviousVideoClicked(object sender, EventArgs e)
        {
            _videoControlHelper.PreviousVideo();
            controlPanel = controlPanel;
            //controlPanel.Padding = 0;
            ResetControlPanelTimer();
        }

        private void OnClearBookmarksClicked(object sender, EventArgs e)
        {
            _videoHelper.ClearBookmarks();
        }

        private void OnBookmarkSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is BookmarkService.Bookmark selectedBookmark)
            {
                _videoHelper.OpenBookmark(selectedBookmark);
            }
        }

        private void OnNextVideoClicked(object sender, EventArgs e)
        {
            _videoControlHelper.NextVideo();
            ResetControlPanelTimer();
        }

        private void OnMediaElementTapped(object sender, EventArgs e)
        {
            _videoControlHelper.ToggleControlPanel();
            ResetControlPanelTimer();
        }

        private void OnProgressChanged(object sender, ValueChangedEventArgs e)
        {
            _videoControlHelper.OnProgressChanged(sender, e);
            ResetControlPanelTimer();
        }

        private void OnAddToBookmarksClicked(object sender, EventArgs e)
        {
            _videoHelper.AddBookmark(mediaElement.Source.ToString(), mediaElement.Position.TotalSeconds);
            ResetControlPanelTimer();
        }

        private void OnShowBookmarksClicked(object sender, EventArgs e)
        {
            _videoHelper.ShowBookmarks();
            ResetControlPanelTimer();
        }

        private void OnShowHistoryClicked(object sender, EventArgs e)
        {
            _videoHelper.ShowHistory();
            ResetControlPanelTimer();
        }

        private void OnShowMediaLibraryClicked(object sender, EventArgs e)
        {
            _videoHelper.ShowMediaLibrary();
            ResetControlPanelTimer();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _videoHelper.SaveHistory(mediaElement.Source.ToString(), mediaElement.Position.TotalSeconds);
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
        }

        private void ResetControlPanelTimer()
        {
            _hideControlPanelTimer.Stop();
            _hideControlPanelTimer.Start();
        }

        private void HideControlPanel()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                controlPanel.IsVisible = false;
                progressPanel.IsVisible = false;
                rotateButton.IsVisible = false;
                sidePanel.IsVisible = false;
            });
        }

        private void ShowControlPanel()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                controlPanel.IsVisible = true;
                progressPanel.IsVisible = true;
                rotateButton.IsVisible = true;
                sidePanel.IsVisible = true;
            });
        }
    }
}