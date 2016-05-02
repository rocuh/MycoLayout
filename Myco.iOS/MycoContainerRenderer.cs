using System;
using System.Collections.Generic;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Myco.MycoContainer), typeof(Myco.iOS.MycoContainerRenderer))]

namespace Myco.iOS
{
    public class MycoContainerRenderer : ViewRenderer<MycoContainer, NativeMycoContainer>
    {
        #region Fields

        private UISwipeGestureRecognizer _swipeGestureLeft;
        private UISwipeGestureRecognizer _swipeGestureRight;
        private UITapGestureRecognizer _tapGesture;

        #endregion Fields

        #region Constructors

        public MycoContainerRenderer()
        {
        }

        #endregion Constructors

        #region Methods

        public static void Initialize()
        {
            DateTime.Now.ToString();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MycoContainer> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var nativeSkContainer = new NativeMycoContainer();
                SetNativeControl(nativeSkContainer);

                _tapGesture = new UITapGestureRecognizer(HandleTap);
                _tapGesture.ShouldReceiveTouch += ValidateTouch;
                AddGestureRecognizer(_tapGesture);

                _swipeGestureLeft = new UISwipeGestureRecognizer(HandleSwipe);
                _swipeGestureLeft.Direction = UISwipeGestureRecognizerDirection.Left;
                _swipeGestureLeft.ShouldReceiveTouch += ValidateTouch;
                AddGestureRecognizer(_swipeGestureLeft);

                _swipeGestureRight = new UISwipeGestureRecognizer(HandleSwipe);
                _swipeGestureRight.Direction = UISwipeGestureRecognizerDirection.Right;
                _swipeGestureRight.ShouldReceiveTouch += ValidateTouch;
                AddGestureRecognizer(_swipeGestureRight);
            }

            if (e.NewElement == null)
            {
                if (_tapGesture != null)
                {
                    _tapGesture.ShouldReceiveTouch -= ValidateTouch;
                    RemoveGestureRecognizer(_tapGesture);
                    _tapGesture = null;
                }

                if (_swipeGestureLeft != null)
                {
                    _swipeGestureLeft.ShouldReceiveTouch -= ValidateTouch;
                    RemoveGestureRecognizer(_swipeGestureLeft);
                    _swipeGestureLeft = null;
                }

                if (_swipeGestureRight != null)
                {
                    _swipeGestureRight.ShouldReceiveTouch -= ValidateTouch;
                    RemoveGestureRecognizer(_swipeGestureRight);
                    _swipeGestureRight = null;
                }
            }
            else if (e.OldElement != null)
            {
                e.NewElement.Layout(e.OldElement.Bounds);
                (e.NewElement as IMycoController).SendLayout();
            }

            Control.Container = e.NewElement;
            Control.SetNeedsDisplay();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == MycoContainer.ContentProperty.PropertyName && Control != null)
            {
                Control.SetNeedsDisplay();
            }
        }

        private void HandleSwipe(UISwipeGestureRecognizer swipe)
        {
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

            var location = swipe.LocationInView(Control);

            Element.GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var swipeGesture = gestureRecognizer.Item2 as MycoSwipeGestureRecognizer;

                if (swipeGesture != null)
                {
                    switch (swipe.Direction)
                    {
                        case UISwipeGestureRecognizerDirection.Left:
                            swipeGesture.SendSwipeLeft(gestureRecognizer.Item1);
                            break;

                        case UISwipeGestureRecognizerDirection.Right:
                            swipeGesture.SendSwipeRight(gestureRecognizer.Item1);
                            break;
                    }
                }
            }
        }

        private void HandleTap(UITapGestureRecognizer tap)
        {
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

            var location = tap.LocationInView(Control);

            Element.GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var tapGesture = gestureRecognizer.Item2 as MycoTapGestureRecognizer;

                if (tapGesture != null && tapGesture.NumberOfTapsRequired == 1)
                {
                    tapGesture.SendTapped(gestureRecognizer.Item1);
                }
            }
        }

        private bool ValidateTouch(UIGestureRecognizer tap, UITouch touch)
        {
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

            var location = touch.LocationInView(Control);

            Element.GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            bool gestureHandled = false;

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var tapGesture = gestureRecognizer.Item2 as MycoTapGestureRecognizer;

                if (tapGesture != null && tapGesture.NumberOfTapsRequired == 1)
                {
                    gestureHandled = true;
                }

                var swipeGesture = gestureRecognizer.Item2 as MycoSwipeGestureRecognizer;

                if (swipeGesture != null)
                {
                    gestureHandled = true;
                }
            }

            return gestureHandled;
        }

        #endregion Methods
    }
}