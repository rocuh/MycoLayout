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
                    SKBitmap bitmap;

                    if (System.IO.File.Exists(fullPath))
                    {
                        bitmap = SKBitmap.Decode(fullPath);
                    }
                    else
                    {
                        string fileId = System.IO.Path.GetFileNameWithoutExtension(fullPath).ToLower();

                        bool isNinePatch = false;

                        if (System.IO.Path.GetExtension(fileId) == ".9p")
                        {
                            fileId = System.IO.Path.GetFileNameWithoutExtension(fileId).ToLower();
                            isNinePatch = true;
                        }

                        var id = Android.App.Application.Context.Resources.GetIdentifier(fileId, "drawable", Android.App.Application.Context.PackageName);

                        const int bytesPerPixel = 4;

                        BitmapFactory.Options opts = new BitmapFactory.Options { InPreferredConfig = Bitmap.Config.Argb8888, InScaled = !isNinePatch };

                        using (var nativeBitmap = BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, id, opts))
                        {
                            int size = nativeBitmap.Width * nativeBitmap.Height * bytesPerPixel;
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
                            drawable = isNinePatch ? new MycoDrawableNinePatch(bitmap) as IMycoDrawable : new MycoDrawableBitmap(bitmap) as IMycoDrawable;
                            _cache.Add(fullPath, drawable);
                        }
                    }
                }

                return drawable;
            }

            return null;
        }

        #endregion Methods
    }
}