using System;

namespace Myco
{
    public class MycoPanGestureRecognizer : MycoGestureRecognizer
    {
        #region Events

        public event EventHandler PanCancelled;
        public event EventHandler PanCompleted;
        public event EventHandler PanStarted;
        public event EventHandler<PanEventArgs> PanUpdated;

        #endregion Events

        #region Methods

        public void SendPanCancelled(MycoView view)
        {
            if (PanCancelled != null)
            {
                PanCancelled(this, new EventArgs());
            }
        }

        public void SendPanCompleted(MycoView view)
        {
            if (PanCompleted != null)
            {
                PanCompleted(this, new EventArgs());
            }
        }

        public void SendPanStarted(MycoView view)
        {
            if (PanStarted != null)
            {
                PanStarted(this, new EventArgs());
            }
        }

        public void SendPanUpdated(MycoView view, double totalX, double totalY)
        {
            if (PanUpdated != null)
            {
                PanUpdated(this, new PanEventArgs(totalX, totalY));
            }
        }

        #endregion Methods
    }

    public class PanEventArgs : EventArgs
    {
        #region Constructors

        public PanEventArgs(double totalX, double totalY)
        {
            TotalX = totalX;
            TotalY = totalY;
        }

        #endregion Constructors

        #region Properties

        public double TotalX { get; private set; }
        public double TotalY { get; private set; }

        #endregion Properties
    }
}