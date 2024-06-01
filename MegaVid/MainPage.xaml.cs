using Xamarin.Forms;
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.CommunityToolkit.UI.Views;

namespace MegaVid
{
    public partial class MainPage : ContentPage
    {
        private double rotationAngle = 0;

        public MainPage()
        {
            InitializeComponent();
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
                AddToHistory(result.FullPath);
            }
        }

        private void AddToHistory(string filePath)
        {
            var history = GetHistory();
            history.Add(filePath);
            Preferences.Set("VideoHistory", string.Join(",", history));
        }

        private List<string> GetHistory()
        {
            var history = Preferences.Get("VideoHistory", string.Empty);
            return string.IsNullOrEmpty(history) ? new List<string>() : history.Split(',').ToList();
        }

        private void OnAddToBookmarksClicked(object sender, EventArgs e)
        {
            if (mediaElement.Source != null)
            {
                var bookmarks = GetBookmarks();
                bookmarks.Add(mediaElement.Source.ToString());
                Preferences.Set("VideoBookmarks", string.Join(",", bookmarks));
                DisplayAlert("Bookmark", "Video added to bookmarks.", "OK");
            }
        }

        private List<string> GetBookmarks()
        {
            var bookmarks = Preferences.Get("VideoBookmarks", string.Empty);
            return string.IsNullOrEmpty(bookmarks) ? new List<string>() : bookmarks.Split(',').ToList();
        }

        private void OnShowMediaLibraryClicked(object sender, EventArgs e)
        {
            // Placeholder for showing media library
            DisplayAlert("Media Library", "Showing media library...", "OK");
        }

        private void OnShowBookmarksClicked(object sender, EventArgs e)
        {
            var bookmarks = GetBookmarks();
            DisplayAlert("Bookmarks", string.Join("\n", bookmarks), "OK");
        }

        private void OnShowHistoryClicked(object sender, EventArgs e)
        {
            var history = GetHistory();
            DisplayAlert("History", string.Join("\n", history), "OK");
        }

        private void OnPlayClicked(object sender, EventArgs e)
        {
            mediaElement.Play();
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            mediaElement.Pause();
        }

        private void OnRewindClicked(object sender, EventArgs e)
        {
            mediaElement.Position -= TimeSpan.FromSeconds(10);
        }

        private void OnForwardClicked(object sender, EventArgs e)
        {
            mediaElement.Position += TimeSpan.FromSeconds(10);
        }

        private void OnVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            mediaElement.Volume = e.NewValue;
        }

        private void OnRotateClicked(object sender, EventArgs e)
        {
            rotationAngle = (rotationAngle + 90) % 360;
            mediaElement.Rotation = rotationAngle;
        }
    }
}