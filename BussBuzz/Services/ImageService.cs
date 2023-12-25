using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Services
{
    public class ImageService
    {
        private readonly string _folderPath;
        private static ImageService instance;
        public static ImageService Instance
        {
            get
            {
                if (instance == null) instance = new ImageService();
                return instance;
            }
        }
        private ImageService() 
        {
            _folderPath = Path.Combine(".." ,"..", "..", "Resources", "Images");
        }

        public List<string> GetImagesNameForAvatarAsync()
        {
            try
            {
                return new List<string>()
                {
                    "emoji1.png",
                    "emoji2.png",
                    "emoji3.png",
                    "emoji4.png",
                    "emoji5.png",
                    "emoji6.png",
                    "emoji7.png",
                    "emoji8.png",
                    "emoji9.png",
                    "emoji10.png" 
                };
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}
