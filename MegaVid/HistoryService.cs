using Xamarin.Essentials;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MegaVid.Services
{
    public class HistoryService
    {
        public List<VideoHistory> GetHistory()
        {
            try
            {
                var historyJson = Preferences.Get("VideoHistory", string.Empty);
                return string.IsNullOrEmpty(historyJson) ? new List<VideoHistory>() : JsonConvert.DeserializeObject<List<VideoHistory>>(historyJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting history: " + ex.Message);
                return new List<VideoHistory>();
            }
        }

        public void SaveHistory(List<VideoHistory> history)
        {
            try
            {
                var historyJson = JsonConvert.SerializeObject(history);
                Preferences.Set("VideoHistory", historyJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving history: " + ex.Message);
            }
        }

        public void ClearHistory()
        {
            try
            {
                Preferences.Set("VideoHistory", string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing history: " + ex.Message);
            }
        }

        public void AddToHistory(string filePath, double position)
        {
            var history = GetHistory();
            history.Add(new VideoHistory { FilePath = filePath, Position = position });
            SaveHistory(history);
        }

        public class VideoHistory
        {
            public string FilePath { get; set; }
            public double Position { get; set; }
        }
    }
}