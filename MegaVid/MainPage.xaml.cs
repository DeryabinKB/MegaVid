using Xamarin.Forms;
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using MegaVid.Services;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;

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

        public MainPage()
        {
            InitializeComponent();
            _bookmarkService = new BookmarkService();
            _historyService = new HistoryService();
            _mediaLibraryService = new MediaLibraryService();
            LoadVideoFiles();
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
                _historyService.AddToHistory(result.FullPath, 0);
            }
        }

        private void OnPlayClicked(object sender, EventArgs e)
        {
            mediaElement.Play();
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            mediaElement.Pause();
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            mediaElement.Stop();
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

        private async void OnShowPlayControlPopupClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new PlayControlPopup(mediaElement));
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
    }
}