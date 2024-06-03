using Xamarin.Forms;
using Xamarin.Essentials;
using System;
using MegaVid.Helpers;

namespace MegaVid
{
    public partial class MainPage : ContentPage
    {
        private readonly VideoHelper _videoHelper;
        private readonly VideoControlHelper _videoControlHelper;
        private readonly System.Timers.Timer _hideControlPanelTimer;

        public MainPage()
        {
            InitializeComponent();
            _videoHelper = new VideoHelper(mediaElement);
            _videoControlHelper = new VideoControlHelper(mediaElement, playPauseButton, volumeSlider, progressSlider, currentTimeLabel, totalTimeLabel, controlPanel, progressPanel, rotateButton, sidePanel);
            _hideControlPanelTimer = new System.Timers.Timer(5000); // 5 секунд
            _hideControlPanelTimer.Elapsed += (sender, args) => HideControlPanel();
            _hideControlPanelTimer.AutoReset = false;

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
            ResetControlPanelTimer();
        }

        private void OnPreviousVideoClicked(object sender, EventArgs e)
        {
            _videoControlHelper.PreviousVideo();
            ResetControlPanelTimer();
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