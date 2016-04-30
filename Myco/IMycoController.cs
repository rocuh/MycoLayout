using SkiaSharp;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Myco
{
    public interface IMycoController : IViewController
    {
        #region Methods

        void GetGestureRecognizers(Point gestureStart, IList<Tuple<MycoView, MycoGestureRecognizer>> matches);

        void SendDraw(SKCanvas canvas);

        void SendLayout();

        #endregion Methods
    }
}