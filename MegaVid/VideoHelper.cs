using Xamarin.Forms;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MegaVid.Services;
using Xamarin.CommunityToolkit.UI.Views;

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
            _bookmarkService.AddBookmark(filePath, position);
            Application.Current.MainPage.DisplayAlert("Bookmark", "Bookmark updated.", "OK");
        }

        public void ShowBookmarks()
        {
            var bookmarks = _bookmarkService.GetBookmarks();
            var bookmarkList = string.Join("\n", bookmarks.Select(b => $"{Path.GetFileName(b.FilePath)}"));
            Application.Current.MainPage.DisplayAlert("Bookmarks", bookmarkList, "OK");
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