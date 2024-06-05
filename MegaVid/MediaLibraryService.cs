using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MegaVid.Services
{
    public class MediaLibraryService
    {
        private readonly string[] _supportedExtensions = { "*.mp4", "*.avi", "*.mkv", "*.mov", "*.wmv" };

        public List<string> LoadVideoFiles(string directory)
        {
            try
            {
                var videoFiles = new List<string>();
                foreach (var extension in _supportedExtensions)
                {
                    videoFiles.AddRange(Directory.GetFiles(directory, extension).ToList());
                }
                return videoFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading video files: " + ex.Message);
                return new List<string>();
            }
        }
    }
}