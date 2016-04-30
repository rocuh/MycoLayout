using SkiaSharp;
using Xamarin.Forms;

namespace Myco
{
    public interface IMycoImageSource
    {
        #region Methods

        SKBitmap SKBitmapFromImageSource(ImageSource source);

        #endregion Methods
    }
}