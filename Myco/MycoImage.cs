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
                    ctrl.Drawable = DependencyService.Get<IMycoImageSource>().SKBitmapFromImageSource((ImageSource)newValue);
                }

                ctrl.Invalidate();
            });

        protected IMycoDrawable Drawable;

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

            if (Drawable != null)
            {
                var renderBounds = RenderBounds;
                var drawableSize = Drawable.GetSize(renderBounds.Width, renderBounds.Height);

                double ratioX = (double)renderBounds.Width / (double)drawableSize.Width;
                double ratioY = (double)renderBounds.Height / (double)drawableSize.Height;
                double ratio = ratioX < ratioY ? ratioX : ratioY;

                double newWidth = drawableSize.Width * ratio;
                double newHeight = drawableSize.Height * ratio;

                float cx = (float)(renderBounds.X + (renderBounds.Width / 2.0) - (newWidth / 2.0));
                float cy = (float)(renderBounds.Y + (renderBounds.Height / 2.0) - (newHeight / 2.0));
                
                using (var paint = new SKPaint())
                {
                    paint.XferMode = SKXferMode.SrcOver;
                    paint.FilterQuality = SKFilterQuality.Low;
                    Drawable.Draw(canvas, new Rectangle(cx, cy, newWidth, newHeight), paint);
                }
            }
        }

        protected override Size InternalSizeRequest(double widthConstraint, double heightConstaint)
        {
            var size = base.InternalSizeRequest(widthConstraint, heightConstaint);

            if (Drawable != null)
            {
                var drawableSize = Drawable.GetSize(widthConstraint, heightConstaint);
                return new Size(Double.IsPositiveInfinity(size.Width) ? drawableSize.Width : Math.Min(drawableSize.Width, size.Width), Double.IsPositiveInfinity(size.Height) ? drawableSize.Height : Math.Min(drawableSize.Height, size.Height));
            }

            return size;
        }

        #endregion Methods
    }
}