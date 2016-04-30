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

        private IMycoController _container;
        private Context _context;

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

        public override bool OnSingleTapUp(MotionEvent e)
        {
            var gestureRecognizers = new List<Tuple<MycoView, MycoGestureRecognizer>>();

            _container.GetGestureRecognizers(new Xamarin.Forms.Point(_context.FromPixels(e.GetX()), _context.FromPixels(e.GetY())), gestureRecognizers);

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

            return base.OnSingleTapUp(e);
        }

        #endregion Methods
    }
}