using SkiaSharp;
using Xamarin.Forms;

namespace Myco
{
    public static class MycoExtensions
    {
        #region Methods

        public static SKColor ToSKColor(this Color color)
        {
            return new SKColor((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255), (byte)(color.A * 255));
        }

        public static SKRect ToSKRect(this Rectangle rect)
        {
            return new SKRect((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
        }

        #endregion Methods
    }
}