using Android.Content;
using Android.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Platform.Android;

namespace Myco.Droid
{
    public class MycoGestureListener : GestureDetector.SimpleOnGestureListener
    {
        #region Fields

        private static int ScrollThreshold = 30;
        private static int SwipeThreshold = 50;
        private static int SwipeVelocityThreshold = 50;

        private IMycoController _container;
        private Context _context;

        private MycoView _activeGestureView;
        private IMycoGestureRecognizerController _activeGesture;

        #endregion Fields

        #region Constructors

        public MycoGestureListener(Context context)
        {
            _context = context;
        }

        #endregion Constructors

        #region Properties

        public IMycoController Container
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

        #endregion Properties

        #region Methods

        public override bool OnDown(MotionEvent e)
        {
            var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

            _container.GetGestureRecognizers(new Xamarin.Forms.Point(_context.FromPixels(e.GetX()), _context.FromPixels(e.GetY())), gestureRecognizers);
            
            bool gestureHandled = false;                      

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var tapGesture = gestureRecognizer.Item2 as MycoTapGestureRecognizer;

                if (tapGesture != null)
                {
                    if (tapGesture.NumberOfTapsRequired == 1)
                    {
                        gestureHandled = true;
                        _activeGestureView = gestureRecognizer.Item1;
                        _activeGesture = gestureRecognizer.Item2;
                    }
                }
                else
                {
                    // pan and swipe always handle
                    gestureHandled = true;
                    _activeGestureView = gestureRecognizer.Item1;
                    _activeGesture = gestureRecognizer.Item2;
                }
            }

            if (_activeGesture != null)
            {
                _activeGesture.SendTouchDown(_activeGestureView, _context.FromPixels(e.GetX()) - _activeGestureView.RenderBounds.X, _context.FromPixels(e.GetY()) - _activeGestureView.RenderBounds.Y);
            }

            return gestureHandled;
        }


        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            bool gestureHandled = false;

            var point1 = new Xamarin.Forms.Point(e1.GetX(), e1.GetY());
            var point2 = new Xamarin.Forms.Point(e2.GetX(), e2.GetY());

            if (!(_activeGesture is IMycoPanGestureRecognizerController) && Math.Abs(point1.Distance(point2)) > ScrollThreshold)
            {
                var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

                _container.GetGestureRecognizers(new Xamarin.Forms.Point(_context.FromPixels(e1.GetX()), _context.FromPixels(e1.GetY())), gestureRecognizers);

                foreach (var gestureRecognizer in gestureRecognizers)
                {
                    var panGesture = gestureRecognizer.Item2 as IMycoPanGestureRecognizerController;

                    if (panGesture != null)
                    {
                        if (_activeGesture != null && _activeGesture != panGesture)
                        {
                            _activeGesture.SendTouchUp(_activeGestureView, _context.FromPixels(e2.GetX()) - _activeGestureView.RenderBounds.X, _context.FromPixels(e2.GetY()) - _activeGestureView.RenderBounds.Y, true);
                        }

                        _activeGestureView = gestureRecognizer.Item1;
                        _activeGesture = panGesture as IMycoGestureRecognizerController;
                        panGesture.SendPanStarted(gestureRecognizer.Item1);
                        gestureHandled = true;
                    }
                }
            }
            else if (_activeGesture is IMycoPanGestureRecognizerController)
            {
                (_activeGesture as IMycoPanGestureRecognizerController).SendPanUpdatedWithUpdate(_activeGestureView, _context.FromPixels(distanceX), _context.FromPixels(distanceY));
                gestureHandled = true;
            }

            return gestureHandled;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

            _container.GetGestureRecognizers(new Xamarin.Forms.Point(_context.FromPixels(e1.GetX()), _context.FromPixels(e1.GetY())), gestureRecognizers);

            bool gestureHandled = false;

            float diffY = e2.GetY() - e1.GetY();
            float diffX = e2.GetX() - e1.GetX();

            if (Math.Abs(diffX) > Math.Abs(diffY))
            {
                if (Math.Abs(diffX) > SwipeThreshold && Math.Abs(velocityX) > SwipeVelocityThreshold)
                {
                    if (diffX < 0)
                    {
                        foreach (var gestureRecognizer in gestureRecognizers)
                        {
                            var swipeGesture = gestureRecognizer.Item2 as IMycoSwipeGestureRecognizerController;

                            if (swipeGesture != null)
                            {
                                swipeGesture.SendSwipeLeft(gestureRecognizer.Item1);
                                gestureHandled = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (var gestureRecognizer in gestureRecognizers)
                        {
                            var swipeGesture = gestureRecognizer.Item2 as IMycoSwipeGestureRecognizerController;

                            if (swipeGesture != null)
                            {
                                swipeGesture.SendSwipeRight(gestureRecognizer.Item1);
                                gestureHandled = true;
                            }
                        }
                    }
                }
            }

            return gestureHandled;
        }

        public override bool OnSingleTapUp(MotionEvent e)
        {
            HandleUp(e);

            var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

            _container.GetGestureRecognizers(new Xamarin.Forms.Point(_context.FromPixels(e.GetX()), _context.FromPixels(e.GetY())), gestureRecognizers);

            bool gestureHandled = false;

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var tapGesture = gestureRecognizer.Item2 as IMycoTapGestureRecognizerController;

                if (tapGesture != null && tapGesture.NumberOfTapsRequired == 1)
                {
                    tapGesture.SendTapped(gestureRecognizer.Item1, _context.FromPixels(e.GetX()) - gestureRecognizer.Item1.RenderBounds.X, _context.FromPixels(e.GetY()) -gestureRecognizer.Item1.RenderBounds.Y);
                    gestureHandled = true;
                }
            }

            return gestureHandled;
        }

        private bool HandleUp(MotionEvent e)
        {
            if (_activeGesture != null)
            {
                var point = new Xamarin.Forms.Point(_context.FromPixels(e.GetX()) - _activeGestureView.RenderBounds.X, _context.FromPixels(e.GetY()));

                var isCanceled = !_activeGestureView.RenderBounds.Contains(point);

                _activeGesture.SendTouchUp(_activeGestureView, _context.FromPixels(e.GetX()) - _activeGestureView.RenderBounds.X, _context.FromPixels(e.GetY()) - _activeGestureView.RenderBounds.Y, isCanceled);

                if (_activeGesture is IMycoPanGestureRecognizerController)
                {
                    (_activeGesture as IMycoPanGestureRecognizerController).SendPanCompleted(_activeGestureView);
                }

                _activeGesture = null;
                _activeGestureView = null;

                return true;
            }

            return false;
        }

        public bool OnUp(MotionEvent e)
        {
            return HandleUp(e);
        }

        #endregion Methods
    }
}