using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MegaVid.Services
{
    public class MediaLibraryService
    {
        public List<string> LoadVideoFiles(string directory)
        {
            try
            {
                return Directory.GetFiles(directory, "*.mp4").ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading video files: " + ex.Message);
                return new List<string>();
            }
        }
    }
}