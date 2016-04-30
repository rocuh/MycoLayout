using Android.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Myco.Droid.MycoImageSource))]

namespace Myco.Droid
{
    public class MycoImageSource : IMycoImageSource
    {
        #region Fields

        /// FIXME: this needs to have some sort of cleanup!
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
                    if (System.IO.File.Exists(fullPath))
                    {
                        bitmap = SKBitmap.Decode(fullPath);
                    }
                    else
                    {
                        string path = System.IO.Path.GetDirectoryName(fullPath);
                        string fileId = System.IO.Path.GetFileNameWithoutExtension(fullPath).ToLower();
                        string extension = System.IO.Path.GetExtension(fullPath);

                        var id = Android.App.Application.Context.Resources.GetIdentifier(fileId, "drawable", Android.App.Application.Context.PackageName);

                        BitmapFactory.Options opts = new BitmapFactory.Options { InPreferredConfig = Bitmap.Config.Argb8888 };

                        using (var nativeBitmap = BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, id, opts))
                        {
                            var pixelLength = nativeBitmap.Height * nativeBitmap.Width;
                            var pixelInts = new int[pixelLength];

                            nativeBitmap.GetPixels(pixelInts, 0, nativeBitmap.Width, 0, 0, nativeBitmap.Width, nativeBitmap.Height);

                            // seems that r & b are inverted from skia sharp and android SKColorType, which is odd since skia is the backend for android...
                            // really would be nice if you could load from resource directly from skiasharp, this is not fast...
                            for (int i = 0; i < pixelLength; i++)
                            {
                                byte a = (byte)((pixelInts[i] >> 24) & 0xFF);
                                byte r = (byte)((pixelInts[i] >> 16) & 0xFF);
                                byte g = (byte)((pixelInts[i] >> 8) & 0xFF);
                                byte b = (byte)((pixelInts[i] >> 0) & 0xFF);

                                pixelInts[i] = (a << 24) | (b << 16) | (g << 8) | r;
                            }

                            bitmap = new SKBitmap(nativeBitmap.Width, nativeBitmap.Height, SKColorType.Rgba_8888, SKAlphaType.Opaque);

                            IntPtr length;

                            bitmap.LockPixels();
                            try
                            {
                                var pixels = bitmap.GetPixels(out length);

                                Marshal.Copy(pixelInts, 0, pixels, nativeBitmap.Width * nativeBitmap.Height);
                            }
                            finally
                            {
                                bitmap.UnlockPixels();
                            }
                        }

                        if (bitmap != null)
                        {
                            _cache.Add(fullPath, bitmap);
                        }
                    }
                }

                return bitmap;
            }

            return null;
        }

        #endregion Methods
    }
}