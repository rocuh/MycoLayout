using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Linq;

[assembly: ExportRenderer(typeof(Myco.MycoContainer), typeof(Myco.iOS.MycoContainerRenderer))]

namespace Myco.iOS
{
    public class MycoContainerRenderer : ViewRenderer<MycoContainer, NativeMycoContainer>
    {
        #region Fields

        private UILongPressGestureRecognizer _longPressGesture;
        private UISwipeGestureRecognizer _swipeGestureLeft;
        private UISwipeGestureRecognizer _swipeGestureRight;
        private UIPanGestureRecognizer _panGesture;

        private List<Tuple<MycoView, IMycoGestureRecognizerController>> _activeGestures = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

        #endregion Fields

        #region Constructors

        public MycoContainerRenderer()
        {
        }

        #endregion Constructors

        #region Methods

        public static void Initialize()
        {
            DependencyService.Register<IMycoImageSource, MycoImageSource>();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MycoContainer> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var nativeSkContainer = new NativeMycoContainer();
                SetNativeControl(nativeSkContainer);

                _longPressGesture = new UILongPressGestureRecognizer(HandlePress);
                _longPressGesture.MinimumPressDuration = 0.0f;
                _longPressGesture.ShouldReceiveTouch += ValidateTouch;
                
                _swipeGestureLeft = new UISwipeGestureRecognizer(HandleSwipe);
                _swipeGestureLeft.Direction = UISwipeGestureRecognizerDirection.Left;
                _swipeGestureLeft.ShouldReceiveTouch += ValidateTouch;

                _swipeGestureRight = new UISwipeGestureRecognizer(HandleSwipe);
                _swipeGestureRight.Direction = UISwipeGestureRecognizerDirection.Right;
                _swipeGestureRight.ShouldReceiveTouch += ValidateTouch;
                
                _panGesture = new UIPanGestureRecognizer(HandlePan);
                _panGesture.DelaysTouchesBegan = false;
                _panGesture.ShouldReceiveTouch += ValidateTouch;

                _longPressGesture.ShouldRecognizeSimultaneously = (gesture1, gesture2) => true;
                _swipeGestureLeft.ShouldRecognizeSimultaneously = (gesture1, gesture2) => true;
                _swipeGestureRight.ShouldRecognizeSimultaneously = (gesture1, gesture2) => true;
                _panGesture.ShouldRecognizeSimultaneously = (gesture1, gesture2) => true;

                AddGestureRecognizer(_panGesture);
                AddGestureRecognizer(_longPressGesture);
                AddGestureRecognizer(_swipeGestureLeft);
                AddGestureRecognizer(_swipeGestureRight);

            }

            if (e.NewElement == null)
            {
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
            var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

            var location = swipe.LocationInView(Control);

            (Element as IMycoController).GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var swipeGesture = gestureRecognizer.Item2 as IMycoSwipeGestureRecognizerController;

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

        private void HandlePan(UIPanGestureRecognizer pan)
        {
            var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

            var location = pan.LocationInView(Control);

            (Element as IMycoController).GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var panGesture = gestureRecognizer.Item2 as IMycoPanGestureRecognizerController;

                if (panGesture != null)
                {
                    switch (pan.State)
                    {
                        case UIGestureRecognizerState.Began:
                            panGesture.SendPanStarted(gestureRecognizer.Item1);
                            break;
                        case UIGestureRecognizerState.Changed:
                            var offset = pan.TranslationInView(Control);
                            panGesture.SendPanUpdatedWithOffset(gestureRecognizer.Item1, -offset.X, -offset.Y);
                            break;
                        case UIGestureRecognizerState.Ended:
                            panGesture.SendPanCompleted(gestureRecognizer.Item1);
                            break;
                        case UIGestureRecognizerState.Cancelled:
                            panGesture.SendPanCancelled(gestureRecognizer.Item1);
                            break;
                    }
                }
            }
        }

        private void HandlePress(UILongPressGestureRecognizer press)
        {
            var location = press.LocationInView(Control);

            switch (press.State)
            {
                case UIGestureRecognizerState.Began:
                    var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

                    (Element as IMycoController).GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

                    _activeGestures.Clear();

                    foreach (var gesture in gestureRecognizers)
                    {
                        gesture.Item2.SendTouchDown(gesture.Item1, location.X - gesture.Item1.RenderBounds.X, location.Y - gesture.Item1.RenderBounds.Y);
                        _activeGestures.Add(gesture);
                    }
                    break;
                case UIGestureRecognizerState.Failed:
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Ended:
                    foreach (var gesture in _activeGestures)
                    {
                        var isCanceled = !gesture.Item1.RenderBounds.Contains(new Xamarin.Forms.Point(location.X, location.Y));
                        gesture.Item2.SendTouchUp(gesture.Item1, location.X - gesture.Item1.RenderBounds.X, location.Y - gesture.Item1.RenderBounds.Y, isCanceled);
                    }

                    _activeGestures.Clear();
                    break;
            }
        }

        private bool ValidateTouch(UIGestureRecognizer gesture, UITouch touch)
        {
            var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

            var location = touch.LocationInView(Control);

            (Element as IMycoController).GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            bool gestureHandled = false;

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                if (gesture is UISwipeGestureRecognizer)
                {
                   gestureHandled |= gestureRecognizer.Item2 is MycoSwipeGestureRecognizer;
                }
                else if (gesture is UIPanGestureRecognizer)
                {
                   gestureHandled |= gestureRecognizer.Item2 is MycoPanGestureRecognizer;
                }
                else if (gesture is UILongPressGestureRecognizer)
                {
                    gestureHandled |= gestureRecognizer.Item2 is MycoTapGestureRecognizer;
                }
            }

            return gestureHandled;
        }

        #endregion Methods
    }
}