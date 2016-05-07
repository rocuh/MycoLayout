using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Myco
{
    public interface IMycoDrawable
    {
        void Draw(SKCanvas canvas, Rectangle rect, SKPaint paint = null);

        Size GetSize(double targetWidth, double targetHeight);

        SKBitmap Bitmap { get; }
    }
}
