using Xamarin.Forms;
using Xamarin.Essentials;
using System;
using MegaVid.Helpers;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.UI.Views;
using MegaVid.Services;
using static MegaVid.Services.BookmarkService;
using System.Linq;

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
            try
            {
                _videoHelper.ClearHistory();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing history: " + ex.Message);
            }
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
            try
            {
                _videoHelper.ClearBookmarks();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing bookmarks: " + ex.Message);
            }
        }

        private async void OnBookmarkSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem is Bookmark selectedBookmark)
                {
                    Console.WriteLine($"Bookmark selected: {selectedBookmark.FilePath} at position {selectedBookmark.Position}");
                    await _videoHelper.OpenBookmarkAsync(selectedBookmark);
                    bookmarkFrame.IsVisible = false; // Скрыть панель закладок после выбора закладки
                }

                // Снять выделение после выбора элемента
                bookmarkListView.SelectedItem = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error selecting bookmark: " + ex.Message);
            }
        }

        private void OnAddToBookmarksClicked(object sender, EventArgs e)
        {
            try
            {
                _videoHelper.AddBookmark(mediaElement.Source.ToString(), mediaElement.Position.TotalSeconds);
                ResetControlPanelTimer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding bookmark: " + ex.Message);
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

        private void OnCloseBookmarksClicked(object sender, EventArgs e)
        {
            try
            {
                bookmarkFrame.IsVisible = false; // Скрыть панель закладок
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing bookmarks: " + ex.Message);
            }
        }

        private void OnShowBookmarksClicked(object sender, EventArgs e)
        {
            try
            {
                var bookmarks = _videoHelper.GetBookmarks();
                bookmarkListView.ItemsSource = bookmarks.Select(b => new { FilePath = System.IO.Path.GetFileName(b.FilePath), TimeAdded = b.TimeAdded, Position = b.Position, FullPath = b.FilePath }).ToList();
                bookmarkFrame.IsVisible = true; // Показать панель закладок
                ResetControlPanelTimer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error showing bookmarks: " + ex.Message);
            }
        }


        private void OnShowHistoryClicked(object sender, EventArgs e)
        {
            try
            {
                _videoHelper.ShowHistory();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error showing history: " + ex.Message);
            }
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