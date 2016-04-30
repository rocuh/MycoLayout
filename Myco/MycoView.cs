using SkiaSharp;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Myco
{
    public class MycoView : BindableObject, IAnimatable
    {
        #region Fields

        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(MycoView), Color.Transparent);                
        public static readonly BindableProperty HorizontalOptionsProperty = BindableProperty.Create(nameof(HorizontalOptions), typeof(LayoutOptions), typeof(MycoView), LayoutOptions.Fill);
        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(MycoView), true, BindingMode.OneWay);
        public static readonly BindableProperty MarginProperty = BindableProperty.Create(nameof(Margin), typeof(Thickness), typeof(MycoView), new Thickness(0, 0, 0, 0));
        public static readonly BindableProperty ScaleProperty = BindableProperty.Create(nameof(Scale), typeof(double), typeof(MycoView), 1.0);
        public static readonly BindableProperty TranslationXProperty = BindableProperty.Create(nameof(TranslationX), typeof(double), typeof(MycoView), 0.0);
        public static readonly BindableProperty TranslationYProperty = BindableProperty.Create(nameof(TranslationY), typeof(double), typeof(MycoView), 0.0);
        public static readonly BindableProperty VerticalOptionsProperty = BindableProperty.Create(nameof(VerticalOptions), typeof(LayoutOptions), typeof(MycoView), LayoutOptions.Fill);
        public static readonly BindableProperty WidthRequestProperty = BindableProperty.Create(nameof(WidthRequest), typeof(double), typeof(MycoView), -1.0, BindingMode.OneWay);
        public static readonly BindableProperty HeightRequestProperty = BindableProperty.Create(nameof(HeightRequest), typeof(double), typeof(MycoView), -1.0, BindingMode.OneWay);

        public static readonly BindablePropertyKey XPropertyKey = BindableProperty.CreateReadOnly(nameof(X), typeof(double), typeof(MycoView), 0.0, BindingMode.OneWayToSource);
        public static readonly BindableProperty XProperty = XPropertyKey.BindableProperty;

        public static readonly BindablePropertyKey YPropertyKey = BindableProperty.CreateReadOnly(nameof(Y), typeof(double), typeof(MycoView), 0.0, BindingMode.OneWayToSource);
        public static readonly BindableProperty YProperty = YPropertyKey.BindableProperty;

        public static readonly BindablePropertyKey WidthPropertyKey = BindableProperty.CreateReadOnly(nameof(Width), typeof(double), typeof(MycoView), 0.0, BindingMode.OneWayToSource);
        public static readonly BindableProperty WidthProperty = WidthPropertyKey.BindableProperty;

        public static readonly BindablePropertyKey HeightPropertyKey = BindableProperty.CreateReadOnly(nameof(Height), typeof(double), typeof(MycoView), 0.0, BindingMode.OneWayToSource);
        public static readonly BindableProperty HeightProperty = HeightPropertyKey.BindableProperty;

        private readonly List<MycoTapGestureRecognizer> _gestureRecognizers = new List<MycoTapGestureRecognizer>();
        private MycoContainer _container;
        private MycoView _parent;

        #endregion Fields

        #region Properties

        public Color BackgroundColor
        {
            get
            {
                return (Color)GetValue(BackgroundColorProperty);
            }
            set
            {
                SetValue(BackgroundColorProperty, value);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        public MycoContainer Container
        {
            get
            {
                return _container;
            }

            set
            {
                _container = value;
            }
        }

        public IList<MycoTapGestureRecognizer> GestureRecognizers
        {
            get { return _gestureRecognizers; }
        }

        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            private set
            {
                SetValue(HeightPropertyKey, value);
            }
        }

        public double HeightRequest
        {
            get
            {
                return (double)GetValue(MycoView.HeightRequestProperty);
            }
            set
            {
                SetValue(MycoView.HeightRequestProperty, value);
            }
        }

        public LayoutOptions HorizontalOptions
        {
            get
            {
                return (LayoutOptions)GetValue(HorizontalOptionsProperty);
            }
            set
            {
                SetValue(HorizontalOptionsProperty, value);
            }
        }

        public bool IsVisible
        {
            get
            {
                return (bool)GetValue(MycoView.IsVisibleProperty);
            }
            set
            {
                SetValue(MycoView.IsVisibleProperty, value);
            }
        }

        public Thickness Margin
        {
            get
            {
                return (Thickness)GetValue(MarginProperty);
            }
            set
            {
                SetValue(MarginProperty, value);
            }
        }

        public MycoView Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                _parent = value;
            }
        }

        public Rectangle RenderBounds
        {
            get
            {
                var bounds = Bounds;

                var scaleW = bounds.Width * Scale;
                var scaleH = bounds.Height * Scale;

                var scaled = Bounds.Inflate((scaleW - bounds.Width) / 2.0, (scaleH - bounds.Height) / 2.0);

                scaled.X += TranslationX;
                scaled.Y += TranslationY;

                return scaled;
            }
        }

        public double Scale
        {
            get
            {
                return (double)GetValue(ScaleProperty);
            }
            set
            {
                SetValue(ScaleProperty, value);
            }
        }

        public double TranslationX
        {
            get
            {
                return (double)GetValue(TranslationXProperty);
            }
            set
            {
                SetValue(TranslationXProperty, value);
            }
        }

        public double TranslationY
        {
            get
            {
                return (double)GetValue(TranslationYProperty);
            }
            set
            {
                SetValue(TranslationYProperty, value);
            }
        }

        public LayoutOptions VerticalOptions
        {
            get
            {
                return (LayoutOptions)GetValue(VerticalOptionsProperty);
            }
            set
            {
                SetValue(VerticalOptionsProperty, value);
            }
        }

        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }
            private set
            {
                SetValue(WidthPropertyKey, value);
            }
        }

        public double WidthRequest
        {
            get
            {
                return (double)GetValue(MycoView.WidthRequestProperty);
            }
            set
            {
                SetValue(MycoView.WidthRequestProperty, value);
            }
        }

        public double X
        {
            get
            {
                return (double)GetValue(XProperty);
            }
            private set
            {
                SetValue(XPropertyKey, value);
            }
        }

        public double Y
        {
            get
            {
                return (double)GetValue(YProperty);
            }
            private set
            {
                SetValue(YPropertyKey, value);
            }
        }

        #endregion Properties

        #region Methods

        public void BatchBegin()
        {
        }

        public void BatchCommit()
        {
            Invalidate();
        }

        public virtual void Draw(SKCanvas canvas)
        {
            if (!IsVisible)
                return;

            if (BackgroundColor.A > 0)
            {
                using (var paint = new SKPaint())
                {
                    paint.Color = BackgroundColor.ToSKColor();
                    canvas.DrawRect(Bounds.ToSKRect(), paint);
                }
            }
        }

        public virtual void GetGestureRecognizers(Point gestureStart, IList<Tuple<MycoView, MycoGestureRecognizer>> matches)
        {
            if (Bounds.Contains(gestureStart))
            {
                foreach (var gesture in _gestureRecognizers)
                {
                    matches.Add(new Tuple<MycoView, MycoGestureRecognizer>(this, gesture));
                }
            }
        }

        public MycoContainer GetSKContainer()
        {
            if (_parent != null)
            {
                return _parent.GetSKContainer();
            }

            return _container;
        }

        public void Invalidate()
        {
            var container = GetSKContainer();

            if (container != null)
            {
                container.Invalidate();
            }
        }

        public virtual void Layout(Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public virtual Size SizeRequest(double widthConstraint, double heightConstaint)
        {
            if (!IsVisible)
                return new Size(0, 0);

            return new Size(
                WidthRequest >= 0 ? (Double.IsPositiveInfinity(widthConstraint) ? WidthRequest : Math.Min(WidthRequest, widthConstraint)) : widthConstraint,
                HeightRequest >= 0 ? (Double.IsPositiveInfinity(heightConstaint) ? HeightRequest : Math.Min(HeightRequest, heightConstaint)) : heightConstaint);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Invalidate();
        }

        #endregion Methods
    }
}