using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Foundation;
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
        private UIPanGestureRecognizer _panGesture;
        private MycoView _activeGestureView;
        private IMycoGestureRecognizerController _activeGesture;

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

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

            var touch = touches.AnyObject as UITouch;

            var location = touch.LocationInView(Control);

            (Element as IMycoController).GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);
            
            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var tapGesture = gestureRecognizer.Item2 as MycoTapGestureRecognizer;

                if (tapGesture != null)
                {
                    if (tapGesture.NumberOfTapsRequired == 1)
                    {
                        _activeGestureView = gestureRecognizer.Item1;
                        _activeGesture = gestureRecognizer.Item2;
                    }
                }
                else
                {
                    _activeGestureView = gestureRecognizer.Item1;
                    _activeGesture = gestureRecognizer.Item2;
                }
            }

            if (_activeGesture != null)
            {
                _activeGesture.SendTouchDown(_activeGestureView, location.X - _activeGestureView.RenderBounds.X, location.Y - _activeGestureView.RenderBounds.Y);
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if (_activeGesture != null)
            {
                var touch = touches.AnyObject as UITouch;
                var location = touch.LocationInView(Control);
                var isCanceled = !_activeGestureView.RenderBounds.Contains(new Xamarin.Forms.Point(location.X, location.Y));

                _activeGesture.SendTouchUp(_activeGestureView, location.X - _activeGestureView.RenderBounds.X, location.Y- _activeGestureView.RenderBounds.Y, isCanceled);
                
                _activeGesture = null;
                _activeGestureView = null;
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            if (_activeGesture != null)
            {
                var touch = touches.AnyObject as UITouch;
                var location = touch.LocationInView(Control);

                _activeGesture.SendTouchUp(_activeGestureView, location.X - _activeGestureView.RenderBounds.X, location.Y - _activeGestureView.RenderBounds.Y, true);

                _activeGesture = null;
                _activeGestureView = null;
            }
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

                _panGesture = new UIPanGestureRecognizer(HandlePan);
                _panGesture.ShouldReceiveTouch += ValidateTouch;
                AddGestureRecognizer(_panGesture);
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

        private void HandleTap(UITapGestureRecognizer tap)
        {
            var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

            var location = tap.LocationInView(Control);

            (Element as IMycoController).GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var tapGesture = gestureRecognizer.Item2 as IMycoTapGestureRecognizerController;

                if (tapGesture != null && tapGesture.NumberOfTapsRequired == 1)
                {
                    tapGesture.SendTapped(gestureRecognizer.Item1, location.X - gestureRecognizer.Item1.RenderBounds.X, location.Y - -gestureRecognizer.Item1.RenderBounds.Y);
                }
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
                if (gesture is UITapGestureRecognizer)
                {
                    var tapGesture = gestureRecognizer.Item2 as MycoTapGestureRecognizer;

                    if (tapGesture != null)
                    {
                        if (tapGesture.NumberOfTapsRequired == 1)
                        {
                            gestureHandled = true;
                        }
                    }
                }
                else if (gesture is UISwipeGestureRecognizer)
                {
                   gestureHandled |= gestureRecognizer.Item2 is MycoSwipeGestureRecognizer;
                }
                else if (gesture is UIPanGestureRecognizer)
                {
                   gestureHandled |= gestureRecognizer.Item2 is MycoPanGestureRecognizer;
                }
            }

            return gestureHandled;
        }

        #endregion Methods
    }
}