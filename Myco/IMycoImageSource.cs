using SkiaSharp;
using Xamarin.Forms;

namespace Myco
{
    public interface IMycoImageSource
    {
        #region Methods

        IMycoDrawable SKBitmapFromImageSource(ImageSource source);

        #endregion Methods
    }
}