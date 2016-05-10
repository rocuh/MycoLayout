using SkiaSharp;
using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace Myco
{
    public class MycoCheckImage : MycoView
    {
        #region Fields

        public static readonly BindableProperty IsCheckableProperty = BindableProperty.Create(nameof(IsCheckable), typeof(bool), typeof(MycoCheckImage), false,
           propertyChanged: (bindableObject, oldValue, newValue) =>
           {
               ((MycoCheckImage)bindableObject).Invalidate();
           });

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(MycoCheckImage), false,
                   defaultBindingMode: BindingMode.TwoWay,
           propertyChanged: (bindableObject, oldValue, newValue) =>
           {
               ((MycoCheckImage)bindableObject).Invalidate();
           });

        public static readonly BindableProperty SelectionBackgroundColorProperty = BindableProperty.Create(nameof(SelectionBackgroundColor), typeof(Color), typeof(MycoCheckImage), Color.FromHex("#CCCCCC"),
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoCheckImage)bindableObject).Invalidate();
            });

        public static readonly BindableProperty SelectionColorProperty = BindableProperty.Create(nameof(SelectionColor), typeof(Color), typeof(MycoCheckImage), Color.FromHex("#5A93F1"),
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoCheckImage)bindableObject).Invalidate();
            });

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(MycoCheckImage), null,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                var ctrl = (MycoCheckImage)bindableObject;

                if (newValue != null)
                {
                    ctrl.Drawable = DependencyService.Get<IMycoImageSource>().SKBitmapFromImageSource((ImageSource)newValue);
                }

                ctrl.Invalidate();
            });

        protected static SKBitmap TickBitmap;
        protected IMycoDrawable Drawable;
        private const int BorderSize = 10;

        #endregion Fields

        #region Constructors

        public MycoCheckImage()
        {
            if (TickBitmap == null)
            {
                var assembly = typeof(MycoCheckImage).GetTypeInfo().Assembly;
                var tickImageName = assembly.GetName().Name + ".Resources.tick.png";

                using (var resource = assembly.GetManifestResourceStream(tickImageName))
                using (var stream = new SKManagedStream(resource))
                {
                    TickBitmap = SKBitmap.Decode(stream);
                }
            }

            var tapGestureRecoginizer = new MycoTapGestureRecognizer()
            {
                Command = IsSelectedCommand
            };

            GestureRecognizers.Add(tapGestureRecoginizer);
        }

        #endregion Constructors

        #region Properties

        public bool IsCheckable
        {
            get
            {
                return (bool)GetValue(IsCheckableProperty);
            }
            set
            {
                SetValue(IsCheckableProperty, value);
            }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        public Color SelectionBackgroundColor
        {
            get
            {
                return (Color)GetValue(SelectionBackgroundColorProperty);
            }
            set
            {
                SetValue(SelectionBackgroundColorProperty, value);
            }
        }

        public Color SelectionColor
        {
            get
            {
                return (Color)GetValue(SelectionColorProperty);
            }
            set
            {
                SetValue(SelectionColorProperty, value);
            }
        }

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

        private ICommand IsSelectedCommand
        {
            get
            {
                return new Command(() => { IsSelected = !IsSelected; });
            }
        }

        #endregion Properties

        #region Methods

        protected override void InternalDraw(SkiaSharp.SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            using (var paint = new SKPaint())
            {
                var renderBounds = RenderBounds;

                var smallestSize = Math.Min(renderBounds.Size.Width, renderBounds.Size.Height);
                var square = new Rectangle(renderBounds.Center.X - smallestSize / 2.0, renderBounds.Center.Y - smallestSize / 2.0, smallestSize, smallestSize);

                Rectangle imageBounds = renderBounds;

                paint.IsAntialias = true;

                if (IsCheckable)
                {
                    paint.Color = SelectionBackgroundColor.ToSKColor();
                    canvas.DrawOval(square.ToSKRect(), paint);

                    var darkerOutline = SelectionBackgroundColor.AddLuminosity(-0.1);
                    paint.IsStroke = true;
                    paint.Color = darkerOutline.ToSKColor();
                    paint.StrokeWidth = 1.0f;

                    canvas.DrawOval(square.ToSKRect(), paint);

                    imageBounds = renderBounds.Inflate(-BorderSize, -BorderSize);
                }

                if (Drawable != null && !IsSelected)
                {
                    var drawableSize = Drawable.GetSize(imageBounds.Width, imageBounds.Height);

                    double ratioX = (double)imageBounds.Width / (double)drawableSize.Width;
                    double ratioY = (double)imageBounds.Height / (double)drawableSize.Height;
                    double ratio = ratioX < ratioY ? ratioX : ratioY;

                    double newWidth = drawableSize.Width * ratio;
                    double newHeight = drawableSize.Height * ratio;

                    float cx = (float)(imageBounds.X + (imageBounds.Width / 2.0) - (newWidth / 2.0));
                    float cy = (float)(imageBounds.Y + (imageBounds.Height / 2.0) - (newHeight / 2.0));

                    paint.XferMode = SKXferMode.SrcOver;
                    paint.FilterQuality = SKFilterQuality.Low;
                    Drawable.Draw(canvas, new Rectangle(cx, cy, newWidth, newHeight), paint);
                }

                if (IsCheckable && IsSelected)
                {
                    paint.Color = SelectionColor.ToSKColor();
                    paint.IsStroke = false;
                    canvas.DrawOval(square.ToSKRect(), paint);

                    double ratioX = (double)imageBounds.Width / (double)TickBitmap.Width;
                    double ratioY = (double)imageBounds.Height / (double)TickBitmap.Height;
                    double ratio = ratioX < ratioY ? ratioX : ratioY;

                    double newWidth = TickBitmap.Width * ratio;
                    double newHeight = TickBitmap.Height * ratio;

                    float cx = (float)(imageBounds.X + (imageBounds.Width / 2.0) - (newWidth / 2.0));
                    float cy = (float)(imageBounds.Y + (imageBounds.Height / 2.0) - (newHeight / 2.0));

                    paint.XferMode = SKXferMode.SrcOver;
                    paint.FilterQuality = SKFilterQuality.Low;
                    canvas.DrawBitmap(TickBitmap, new SKRect(cx, cy, (float)(cx + newWidth), (float)(cy + newHeight)), paint);
                }
            }
        }

        protected override Size InternalSizeRequest(double widthConstraint, double heightConstaint)
        {
            var size = base.InternalSizeRequest(widthConstraint, heightConstaint);

            if (Drawable != null)
            {
                var drawableSize = Drawable.GetSize(widthConstraint, heightConstaint);
                return new Size(Double.IsPositiveInfinity(size.Width) ? drawableSize.Width + BorderSize : Math.Min(drawableSize.Width + (BorderSize * 2), size.Width), Double.IsPositiveInfinity(size.Height) ? drawableSize.Height + (BorderSize * 2) : Math.Min(drawableSize.Height + (BorderSize * 2), size.Height));
            }

            return size;
        }

        #endregion Methods
    }
}