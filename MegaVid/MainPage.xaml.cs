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

        public MainPage()
        {
            InitializeComponent();
            _videoHelper = new VideoHelper(mediaElement);
            _videoControlHelper = new VideoControlHelper(mediaElement, playPauseButton, volumeSlider, progressSlider, currentTimeLabel, totalTimeLabel, controlPanel, progressPanel);
            LoadVideoFiles();
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
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
        }

        private void OnRewindClicked(object sender, EventArgs e)
        {
            _videoControlHelper.Rewind();
        }

        private void OnFastForwardClicked(object sender, EventArgs e)
        {
            _videoControlHelper.FastForward();
        }

        private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            _videoControlHelper.ChangeVolume(e.NewValue);
        }

        private async void OnRotateClicked(object sender, EventArgs e)
        {
            await _videoControlHelper.Rotate();
        }

        private void OnPreviousVideoClicked(object sender, EventArgs e)
        {
            _videoControlHelper.PreviousVideo();
        }

        private void OnNextVideoClicked(object sender, EventArgs e)
        {
            _videoControlHelper.NextVideo();
        }

        private void OnMediaElementTapped(object sender, EventArgs e)
        {
            _videoControlHelper.ToggleControlPanel();
        }

        private void OnProgressChanged(object sender, ValueChangedEventArgs e)
        {
            _videoControlHelper.OnProgressChanged(sender, e);
        }

        private void OnAddToBookmarksClicked(object sender, EventArgs e)
        {
            _videoHelper.AddBookmark(mediaElement.Source.ToString(), mediaElement.Position.TotalSeconds);
        }

        private void OnShowBookmarksClicked(object sender, EventArgs e)
        {
            _videoHelper.ShowBookmarks();
        }

        private void OnShowHistoryClicked(object sender, EventArgs e)
        {
            _videoHelper.ShowHistory();
        }

        private void OnShowMediaLibraryClicked(object sender, EventArgs e)
        {
            _videoHelper.ShowMediaLibrary();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _videoHelper.SaveHistory(mediaElement.Source.ToString(), mediaElement.Position.TotalSeconds);
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
        }
    }
}