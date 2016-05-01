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
                _tapGesture.ShouldReceiveTouch += ValidateTap;
                AddGestureRecognizer(_tapGesture);
            }

            if (e.NewElement == null)
            {
                if (_tapGesture != null)
                {
                    _tapGesture.ShouldReceiveTouch -= ValidateTap;
                    RemoveGestureRecognizer(_tapGesture);
                    _tapGesture = null;
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

        private void HandleTap(UITapGestureRecognizer tap)
        {
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

            var location = tap.LocationInView(Control);

            Element.GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            if (gestureRecognizers.Count > 0)
            {
                foreach (var gestureRecognizer in gestureRecognizers)
                {
                    var tapGesture = gestureRecognizer.Item2 as MycoTapGestureRecognizer;

                    if (tapGesture != null && tapGesture.NumberOfTapsRequired == 1)
                    {
                        tapGesture.SendTapped(gestureRecognizer.Item1);
                    }
                }
            }
        }

        private bool ValidateTap(UIGestureRecognizer tap, UITouch touch)
        {
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

            var location = touch.LocationInView(Control);

            Element.GetGestureRecognizers(new Xamarin.Forms.Point(location.X, location.Y), gestureRecognizers);

            bool gestureHandled = false;

            if (gestureRecognizers.Count > 0)
            {
                foreach (var gestureRecognizer in gestureRecognizers)
                {
                    var tapGesture = gestureRecognizer.Item2 as MycoTapGestureRecognizer;

                    if (tapGesture != null && tapGesture.NumberOfTapsRequired == 1)
                    {
                        gestureHandled = true;
                    }
                }
            }

            return gestureHandled;
        }

        #endregion Methods
    }
}