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

        public void GetGestureRecognizers(Point gestureStart, IList<Tuple<MycoView, MycoGestureRecognizer>> matches)
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

        void IMycoController.SendDraw(SKCanvas canvas)
        {
            Draw(canvas);
        }

        void IMycoController.SendLayout()
        {
            SKLayout();
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
                Content.Render(canvas);
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

        protected virtual void SKLayout()
        {
            if (Content != null)
            {
                Content.Layout(new Rectangle(0,0, Bounds.Width, Bounds.Height));
            }
        }

        #endregion Methods
    }
}