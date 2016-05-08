using SkiaSharp;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Myco
{
    [ContentProperty("Content")]
    public class MycoContainer : View, IMycoController
    {
        #region Fields

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(MycoView), typeof(MycoContainer), null,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (oldValue != null)
                {
                    ((MycoView)oldValue).Container = null;
                }

                if (newValue != null)
                {
                    ((MycoView)newValue).Container = (MycoContainer)bindableObject;
                }
            });

        private int _surfaceWidth;
        private int _surfaceHeight;

        #endregion Fields

        #region Constructors

        public MycoContainer()
        {
        }

        #endregion Constructors

        #region Events

        public event EventHandler<EventArgs> InvalidateNative;

        #endregion Events

        #region Properties

        public MycoView Content
        {
            get
            {
                return (MycoView)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        void IMycoController.GetGestureRecognizers(Point gestureStart, IList<Tuple<MycoView, IMycoGestureRecognizerController>> matches)
        {
            if (Content != null)
            {
                Content.GetGestureRecognizers(gestureStart, matches);
            }
        }

        public override SizeRequest GetSizeRequest(double widthConstraint, double heightConstraint)
        {
            if (Content != null)
            {
                return new SizeRequest(Content.SizeRequest(widthConstraint, heightConstraint));
            }

            return base.GetSizeRequest(widthConstraint, heightConstraint);
        }

        void IMycoController.SendSurfaceSize(int width, int height)
        {
            _surfaceWidth = width;
            _surfaceHeight = height;
        }

        void IMycoController.SendDraw(SKCanvas canvas)
        {
            Draw(canvas);
        }

        void IMycoController.SendLayout()
        {
            InternalLayout();
        }

        public void Invalidate()
        {
            if (InvalidateNative != null)
            {
                InvalidateNative(this, new EventArgs());
            }
        }

        protected virtual void Draw(SKCanvas canvas)
        {
            canvas.Clear();

            if (Content != null)
            {
                Content.Draw(canvas);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (Content != null)
            {
                Content.SetInheritedBindingContextReflect(BindingContext);
            }
        }

        protected virtual void InternalLayout()
        {
            if (Content != null)
            {
                Content.Layout(new Rectangle(0,0, Bounds.Width, Bounds.Height));
            }
        }

        public SKSurface CreateOpacitySurface()
        {
            // transparency will be slow!
            return SKSurface.Create(new SKImageInfo(_surfaceWidth, _surfaceHeight, SKColorType.Rgba_8888, SKAlphaType.Opaque));
        }

        #endregion Methods
    }
}