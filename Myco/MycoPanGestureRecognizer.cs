using System;

namespace Myco
{
    public class MycoPanGestureRecognizer : MycoGestureRecognizer
    {
        #region Fields

        private double _lastOffsetX;
        private double _lastOffsetY;

        #endregion Fields

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
            _lastOffsetX = 0;
            _lastOffsetY = 0;

            if (PanStarted != null)
            {
                PanStarted(this, new EventArgs());
            }
        }

        public void SendPanUpdatedWithOffset(MycoView view, double offsetX, double offsetY)
        {
            if (PanUpdated != null)
            {
                PanUpdated(this, new PanEventArgs(offsetX - _lastOffsetX, offsetY - _lastOffsetY));
            }

            _lastOffsetX = offsetX;
            _lastOffsetY = offsetY;
        }

        public void SendPanUpdatedWithUpdate(MycoView view, double updateX, double updateY)
        {
            if (PanUpdated != null)
            {
                PanUpdated(this, new PanEventArgs(updateX, updateY));
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