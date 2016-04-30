using Foundation;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Myco.iOS.MycoImageSource))]

namespace Myco.iOS
{
    public class MycoImageSource : IMycoImageSource
    {
        #region Fields

        // FIXME: this needs to have some sort of cleanup!
        private Dictionary<string, SKBitmap> _cache = new Dictionary<string, SKBitmap>();

        #endregion Fields

        #region Methods

        public SKBitmap SKBitmapFromImageSource(ImageSource source)
        {
            if (source is FileImageSource)
            {
                string fullPath = ((FileImageSource)source).File;

                SKBitmap bitmap;

                if (!_cache.TryGetValue(fullPath, out bitmap))
                {
                    if (File.Exists(fullPath))
                    {
                        bitmap = SKBitmap.Decode(fullPath);
                    }
                    else
                    {
                        string path = Path.GetDirectoryName(fullPath);
                        string fileId = Path.GetFileNameWithoutExtension(fullPath).ToLower();
                        string extension = Path.GetExtension(fullPath);

                        var resourcePath = NSBundle.MainBundle.PathForResource(fileId, extension, path);

                        bitmap = SKBitmap.Decode(resourcePath);
                    }

                    if (bitmap != null)
                    {
                        _cache.Add(fullPath, bitmap);
                    }
                }

                return bitmap;
            }

            return null;
        }

        #endregion Methods
    }
}