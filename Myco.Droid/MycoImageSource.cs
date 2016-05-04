using Android.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xamarin.Forms;
using Android.Runtime;

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
                        string fileId = System.IO.Path.GetFileNameWithoutExtension(fullPath).ToLower();

                        var id = Android.App.Application.Context.Resources.GetIdentifier(fileId, "drawable", Android.App.Application.Context.PackageName);

                        const int bytesPerPixel = 4;

                        BitmapFactory.Options opts = new BitmapFactory.Options { InPreferredConfig = Bitmap.Config.Argb8888 };

                        using (var nativeBitmap = BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, id, opts))
                        {
                            int size = nativeBitmap.Width * nativeBitmap .Height * bytesPerPixel;
                            var pixelData = new byte[size];
                            using (var byteBuffer = Java.Nio.ByteBuffer.AllocateDirect(size))
                            {
                                nativeBitmap.CopyPixelsToBuffer(byteBuffer);
                                Marshal.Copy(byteBuffer.GetDirectBufferAddress(), pixelData, 0, size);
                            }

                            bitmap = new SKBitmap(nativeBitmap.Width, nativeBitmap.Height, SKColorType.Rgba_8888, SKAlphaType.Premul);

                            IntPtr length;

                            bitmap.LockPixels();
                            try
                            {
                                // wish there was a way to IntPtr to IntPtr copy
                                var pixels = bitmap.GetPixels(out length);
                                Marshal.Copy(pixelData, 0, pixels, size);
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