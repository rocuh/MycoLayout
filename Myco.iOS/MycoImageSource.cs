using Foundation;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Myco.iOS.MycoImageSource))]

namespace Myco.iOS
{
    public class MycoImageSource : IMycoImageSource
    {
        #region Fields

        // FIXME: this needs to have some sort of cleanup!
        private Dictionary<string, IMycoDrawable> _cache = new Dictionary<string, IMycoDrawable>();

        #endregion Fields

        #region Methods

        public IMycoDrawable SKBitmapFromImageSource(ImageSource source)
        {
            if (source is FileImageSource)
            {
                string fullPath = ((FileImageSource)source).File;

                IMycoDrawable drawable = null;

                if (!_cache.TryGetValue(fullPath, out drawable))
                {
                    SKBitmap bitmap = null;

                    string path = Path.GetDirectoryName(fullPath);
                    string fileId = Path.GetFileNameWithoutExtension(fullPath);
                    string extension = Path.GetExtension(fullPath);

                    bool isNinePatch = Path.GetExtension(fileId) == ".9p";
                    
                    int scaleInt = (int)Math.Round(UIScreen.MainScreen.Scale, MidpointRounding.AwayFromZero);

                    string lastFoundResource = "";

                    for (int i = 1; i <= scaleInt; i++)
                    {
                        if (File.Exists(Path.Combine(path, fileId + (i == 1 ? "" : $"@{i}x") + extension)))
                        {
                            lastFoundResource = Path.Combine(path, fileId + (i == 1 ? "" : $"@{i}x") + extension);
                        }
                        else
                        {
                            var resourcePath = NSBundle.MainBundle.PathForResource(fileId + (i == 1 ? "" : $"@{i}x"), extension, path);

                            if (!File.Exists(resourcePath))
                            {
                                break;
                            }

                            lastFoundResource = resourcePath;
                        }
                    }

                    if (!String.IsNullOrEmpty(lastFoundResource))
                    {
                        bitmap = SKBitmap.Decode(lastFoundResource);
                    }

                    if (bitmap != null)
                    {
                        drawable = isNinePatch ? new MycoDrawableNinePatch(bitmap) as IMycoDrawable : new MycoDrawableBitmap(bitmap) as IMycoDrawable;
                        _cache.Add(fullPath, drawable);
                    }
                }

                return drawable;
            }

            return null;
        }

        #endregion Methods
    }
}