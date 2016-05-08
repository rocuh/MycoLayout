using System;
using Xamarin.Forms;

namespace Myco
{
    public class MycoGestureRecognizer : BindableObject, IMycoGestureRecognizerController
    {
        public event EventHandler<TouchDownEventArgs> TouchDown;
        public event EventHandler<TouchUpEventArgs> TouchUp;

        void IMycoGestureRecognizerController.SendTouchDown(MycoView view, double x, double y)
        {
            if (TouchDown != null)
                TouchDown(view, new TouchDownEventArgs(x, y));
        }

        void IMycoGestureRecognizerController.SendTouchUp(MycoView view, double x, double y, bool canceled)
        {
            if (TouchUp != null)
                TouchUp(view, new TouchUpEventArgs(x, y, canceled));
        }
    }

    public interface IMycoGestureRecognizerController
    {
        void SendTouchDown(MycoView view, double x, double y);
        void SendTouchUp(MycoView view, double x, double y, bool canceled);
    }

    public class TouchDownEventArgs : EventArgs
    {
        #region Constructors

        public TouchDownEventArgs(double x, double y)
        {
            X = x;
            Y = y;
        }

        #endregion Constructors

        #region Properties

        public double X { get; private set; }
        public double Y { get; private set; }

        #endregion Properties
    }

    public class TouchUpEventArgs : EventArgs
    {
        #region Constructors

        public TouchUpEventArgs(double x, double y, bool canceled)
        {
            X = x;
            Y = y;
            Canceled = canceled;
        }

        #endregion Constructors

        #region Properties

        public double X { get; private set; }
        public double Y { get; private set; }
        public bool Canceled { get; private set; }

        #endregion Properties
    }
}