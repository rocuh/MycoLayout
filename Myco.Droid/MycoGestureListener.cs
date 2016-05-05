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

        private MycoView _activePanView;
        private MycoPanGestureRecognizer _activePanGesture;

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
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

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
                    }
                }
                else
                {
                    // pan and swipe always handle
                    gestureHandled = true;
                }
            }

            return gestureHandled;
        }


        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            bool gestureHandled = false;

            var point1 = new Xamarin.Forms.Point(e1.GetX(), e1.GetY());
            var point2 = new Xamarin.Forms.Point(e2.GetX(), e2.GetY());

            if (_activePanGesture == null && Math.Abs(point1.Distance(point2)) > ScrollThreshold)
            {
                var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

                _container.GetGestureRecognizers(new Xamarin.Forms.Point(_context.FromPixels(e1.GetX()), _context.FromPixels(e1.GetY())), gestureRecognizers);

                foreach (var gestureRecognizer in gestureRecognizers)
                {
                    var panGesture = gestureRecognizer.Item2 as MycoPanGestureRecognizer;

                    if (panGesture != null)
                    {
                        _activePanView = gestureRecognizer.Item1;
                        _activePanGesture = panGesture;
                        panGesture.SendPanStarted(gestureRecognizer.Item1);
                        gestureHandled = true;
                    }
                }
            }
            else if (_activePanGesture != null)
            {
                _activePanGesture.SendPanUpdatedWithUpdate(_activePanView, _context.FromPixels(distanceX), _context.FromPixels(distanceY));
                gestureHandled = true;
            }

            return gestureHandled;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

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
                            var swipeGesture = gestureRecognizer.Item2 as MycoSwipeGestureRecognizer;

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
                            var swipeGesture = gestureRecognizer.Item2 as MycoSwipeGestureRecognizer;

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
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

            _container.GetGestureRecognizers(new Xamarin.Forms.Point(_context.FromPixels(e.GetX()), _context.FromPixels(e.GetY())), gestureRecognizers);

            bool gestureHandled = false;

            foreach (var gestureRecognizer in gestureRecognizers)
            {
                var tapGesture = gestureRecognizer.Item2 as MycoTapGestureRecognizer;

                if (tapGesture != null && tapGesture.NumberOfTapsRequired == 1)
                {
                    tapGesture.SendTapped(gestureRecognizer.Item1, _context.FromPixels(e.GetX()) - gestureRecognizer.Item1.RenderBounds.X, _context.FromPixels(e.GetY()) -gestureRecognizer.Item1.RenderBounds.Y);
                    gestureHandled = true;
                }
            }

            return gestureHandled;
        }

        public bool OnUp(MotionEvent e)
        {
            if (_activePanGesture != null)
            {
                _activePanGesture.SendPanCompleted(_activePanView);

                _activePanGesture = null;
                _activePanView = null;

                return true;
            }

            return false;
        }

        #endregion Methods
    }
}