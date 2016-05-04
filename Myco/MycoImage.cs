using SkiaSharp;
using System;
using Xamarin.Forms;

namespace Myco
{
    public class MycoImage : MycoView
    {
        #region Fields

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(MycoImage), null,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                var ctrl = (MycoImage)bindableObject;

                if (newValue != null)
                {
                    ctrl._bitmap = DependencyService.Get<IMycoImageSource>().SKBitmapFromImageSource((ImageSource)newValue);
                }

                ctrl.Invalidate();
            });

        private SKBitmap _bitmap;

        #endregion Fields

        #region Properties

        public ImageSource Source
        {
            get
            {
                return (ImageSource)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void InternalDraw(SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            if (_bitmap != null)
            {
                var renderBounds = RenderBounds;

                double ratioX = (double)renderBounds.Width / (double)_bitmap.Width;
                double ratioY = (double)renderBounds.Height / (double)_bitmap.Height;
                double ratio = ratioX < ratioY ? ratioX : ratioY;

                double newWidth = _bitmap.Width * ratio;
                double newHeight = _bitmap.Height * ratio;

                float cx = (float)(renderBounds.X + (renderBounds.Width / 2.0) - (newWidth / 2.0));
                float cy = (float)(renderBounds.Y + (renderBounds.Height / 2.0) - (newHeight / 2.0));
                
                using (var paint = new SKPaint())
                {
                    paint.XferMode = SKXferMode.SrcOver;
                    paint.FilterQuality = SKFilterQuality.Low;
                    canvas.DrawBitmap(_bitmap, new SKRect(cx, cy, (float)(cx + newWidth), (float)(cy + newHeight)), paint);
                }
            }
        }

        protected override Size InternalSizeRequest(double widthConstraint, double heightConstaint)
        {
            var size = base.InternalSizeRequest(widthConstraint, heightConstaint);

            if (_bitmap != null)
            {
                return new Size(Double.IsPositiveInfinity(size.Width) ? _bitmap.Width : Math.Min(_bitmap.Width, size.Width), Double.IsPositiveInfinity(size.Height) ? _bitmap.Height : Math.Min(_bitmap.Height, size.Height));
            }

            return size;
        }

        #endregion Methods
    }
}