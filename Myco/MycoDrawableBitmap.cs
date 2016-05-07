using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Myco
{
    public class MycoDrawableBitmap : IMycoDrawable
    {
        public MycoDrawableBitmap(SKBitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public void Draw(SKCanvas canvas, Rectangle rect, SKPaint paint = null)
        {
            if (Bitmap != null)
            {
                canvas.DrawBitmap(Bitmap, rect.ToSKRect(), paint);
            }
        }

        public Size GetSize(double targetWidth, double targetHeight)
        {
            return new Size(Bitmap.Width, Bitmap.Height);
        }

        public SKBitmap Bitmap { get; private set; }
    }
}
