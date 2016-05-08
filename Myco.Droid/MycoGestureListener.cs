using Android.Content;
using Android.Views;
using System;
using System.Linq;
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
        
        private List<Tuple<MycoView, IMycoGestureRecognizerController>> _activeGestures = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

        private MycoView _activePanGestureView;
        private IMycoPanGestureRecognizerController _activePanGesture;

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

            _activeGestures.Clear();

            if (gestureRecognizers.Count > 0)
            {
                foreach (var gesture in gestureRecognizers)
                {
                    gesture.Item2.SendTouchDown(gesture.Item1, _context.FromPixels(e.GetX()) - gesture.Item1.RenderBounds.X, _context.FromPixels(e.GetY()) - gesture.Item1.RenderBounds.Y);
                    _activeGestures.Add(gesture);
                }

                return true;
            }

            return false;
        }


        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            bool gestureHandled = false;

            var point1 = new Xamarin.Forms.Point(e1.GetX(), e1.GetY());
            var point2 = new Xamarin.Forms.Point(e2.GetX(), e2.GetY());

            if (_activePanGesture == null && Math.Abs(point1.Distance(point2)) > ScrollThreshold)
            {
                var gestureRecognizers = new List<Tuple<MycoView, IMycoGestureRecognizerController>>();

                _container.GetGestureRecognizers(new Xamarin.Forms.Point(_context.FromPixels(e1.GetX()), _context.FromPixels(e1.GetY())), gestureRecognizers);

                foreach (var gestureRecognizer in gestureRecognizers)
                {
                    var panGesture = gestureRecognizer.Item2 as IMycoPanGestureRecognizerController;

                    if (panGesture != null)
                    {
                        _activePanGestureView = gestureRecognizer.Item1;
                        _activePanGesture = panGesture;
                        panGesture.SendPanStarted(gestureRecognizer.Item1);
                        gestureHandled = true;
                        break;
                    }
                }
            }
            else if (_activePanGesture != null)
            {
                _activePanGesture.SendPanUpdatedWithUpdate(_activePanGestureView, _context.FromPixels(distanceX), _context.FromPixels(distanceY));
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
            return HandleUp(e);
        }

        private bool HandleUp(MotionEvent e)
        {
            foreach (var gesture in _activeGestures)
            {
                var point = new Xamarin.Forms.Point(_context.FromPixels(e.GetX()), _context.FromPixels(e.GetY()));
                var isCanceled = !gesture.Item1.RenderBounds.Contains(point);
                gesture.Item2.SendTouchUp(gesture.Item1, _context.FromPixels(e.GetX()) - gesture.Item1.RenderBounds.X, _context.FromPixels(e.GetY()) - gesture.Item1.RenderBounds.Y, isCanceled);
            }

            _activeGestures.Clear();

            if (_activePanGesture != null)
            {               
                if (_activePanGesture is IMycoPanGestureRecognizerController)
                {
                    (_activePanGesture as IMycoPanGestureRecognizerController).SendPanCompleted(_activePanGestureView);
                }

                _activePanGesture = null;
                _activePanGestureView = null;

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