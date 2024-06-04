using Xamarin.Essentials;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MegaVid.Services
{
    public class BookmarkService
    {
        public List<Bookmark> GetBookmarks()
        {
            try
            {
                var bookmarkJson = Preferences.Get("VideoBookmarks", string.Empty);
                return string.IsNullOrEmpty(bookmarkJson) ? new List<Bookmark>() : JsonConvert.DeserializeObject<List<Bookmark>>(bookmarkJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting bookmarks: " + ex.Message);
                return new List<Bookmark>();
            }
        }



        public void SaveBookmarks(List<Bookmark> bookmarks)
        {
            try
            {
                var bookmarkJson = JsonConvert.SerializeObject(bookmarks);
                Preferences.Set("VideoBookmarks", bookmarkJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving bookmarks: " + ex.Message);
            }
        }

        public void AddBookmark(string filePath, double position)
        {
            var bookmarks = GetBookmarks();
            var existingBookmark = bookmarks.FirstOrDefault(b => b.FilePath == filePath);
            if (existingBookmark != null)
            {
                bookmarks.Remove(existingBookmark);
            }
            bookmarks.Add(new Bookmark { FilePath = filePath, Position = position });
            SaveBookmarks(bookmarks);
        }

        public void ClearBookmarks()
        {
            SaveBookmarks(new List<Bookmark>());
        }

        public class Bookmark
        {
            public string FilePath { get; set; }
            public double Position { get; set; }
        }
    }
}