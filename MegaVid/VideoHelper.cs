using Xamarin.Forms;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MegaVid.Services;
using Xamarin.CommunityToolkit.UI.Views;
using static MegaVid.Services.BookmarkService;
using System;
using System.Threading.Tasks;

namespace MegaVid.Helpers
{
    public class VideoHelper
    {
        private readonly MediaElement _mediaElement;
        private readonly BookmarkService _bookmarkService;
        private readonly HistoryService _historyService;
        private readonly MediaLibraryService _mediaLibraryService;
        private List<string> _videoFiles;

        public VideoHelper(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;
            _bookmarkService = new BookmarkService();
            _historyService = new HistoryService();
            _mediaLibraryService = new MediaLibraryService();
        }

        public void AddBookmark(string filePath, double position)
        {
            var timeAdded = DateTime.Now.ToString("HH:mm:ss");
            _bookmarkService.AddBookmark(filePath, position, timeAdded);
            Application.Current.MainPage.DisplayAlert("Bookmark", "Bookmark updated.", "OK");
        }

        public void ShowBookmarks()
        {
            var bookmarks = _bookmarkService.GetBookmarks();
            var bookmarkList = string.Join("\n", bookmarks.Select(b => $"{Path.GetFileName(b.FilePath)} at {b.TimeAdded}"));
            Application.Current.MainPage.DisplayAlert("Bookmarks", bookmarkList, "OK");
        }

        public async Task OpenBookmarkAsync(Bookmark bookmark)
        {
            try
            {
                Console.WriteLine($"Attempting to open bookmark: {bookmark.FilePath} at position {bookmark.Position}");

                if (File.Exists(bookmark.FilePath))
                {
                    _mediaElement.Source = bookmark.FilePath;
                    await Task.Delay(500); // Небольшая задержка для загрузки источника
                    _mediaElement.Position = TimeSpan.FromSeconds(bookmark.Position);
                    _mediaElement.Play();
                    Console.WriteLine("Video started playing from bookmark.");
                }
                else
                {
                    Console.WriteLine($"File does not exist: {bookmark.FilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening bookmark: " + ex.Message);
            }
        }

        public void ShowHistory()
        {
            var history = _historyService.GetHistory();
            var historyList = string.Join("\n", history.Select(h => $"{Path.GetFileName(h.FilePath)} at {h.Position}"));
            Application.Current.MainPage.DisplayAlert("History", historyList, "OK");
        }

        public void SaveHistory(string filePath, double position)
        {
            _historyService.AddToHistory(filePath, position);
        }

        public void ClearBookmarks()
        {
            _bookmarkService.ClearBookmarks();
        }

        public void OpenBookmark(Bookmark bookmark)
        {
            _mediaElement.Source = bookmark.FilePath;
            _mediaElement.Position = TimeSpan.FromSeconds(bookmark.Position);
            _mediaElement.Play();
        }

        public List<BookmarkService.Bookmark> GetBookmarks()
        {
            return _bookmarkService.GetBookmarks();
        }

        public void ClearHistory()
        {
            _historyService.ClearHistory();
            Application.Current.MainPage.DisplayAlert("History", "History cleared.", "OK");
        }

        public void LoadVideoFiles(string directory)
        {
            _videoFiles = _mediaLibraryService.LoadVideoFiles(directory);
        }

        public void ShowMediaLibrary()
        {
            var videoList = string.Join("\n", _videoFiles.Select(v => Path.GetFileName(v)));
            Application.Current.MainPage.DisplayAlert("Media Library", videoList, "OK");
        }
    }
}