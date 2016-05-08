using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Myco
{
    public class MycoSwipeGestureRecognizer : MycoGestureRecognizer, IMycoSwipeGestureRecognizerController
    {
        #region Fields

        public static readonly BindableProperty SwipeLeftCommandParameterProperty = BindableProperty.Create(nameof(SwipeLeftCommandParameter), typeof(object), typeof(MycoTapGestureRecognizer), null);
        public static readonly BindableProperty SwipeLeftCommandProperty = BindableProperty.Create(nameof(SwipeLeftCommand), typeof(ICommand), typeof(MycoTapGestureRecognizer), null);
        public static readonly BindableProperty SwipeRightCommandParameterProperty = BindableProperty.Create(nameof(SwipeRightCommandParameter), typeof(object), typeof(MycoTapGestureRecognizer), null);
        public static readonly BindableProperty SwipeRightCommandProperty = BindableProperty.Create(nameof(SwipeRightCommand), typeof(ICommand), typeof(MycoTapGestureRecognizer), null);

        #endregion Fields

        #region Events

        public event EventHandler SwipeLeft;

        public event EventHandler SwipeRight;

        #endregion Events

        #region Properties

        public ICommand SwipeLeftCommand
        {
            get
            {
                return (ICommand)GetValue(SwipeLeftCommandProperty);
            }
            set
            {
                SetValue(SwipeLeftCommandProperty, value);
            }
        }

        public object SwipeLeftCommandParameter
        {
            get
            {
                return GetValue(SwipeLeftCommandParameterProperty);
            }
            set
            {
                SetValue(SwipeLeftCommandParameterProperty, value);
            }
        }

        public ICommand SwipeRightCommand
        {
            get
            {
                return (ICommand)GetValue(SwipeRightCommandProperty);
            }
            set
            {
                SetValue(SwipeRightCommandProperty, value);
            }
        }

        public object SwipeRightCommandParameter
        {
            get
            {
                return GetValue(SwipeRightCommandParameterProperty);
            }
            set
            {
                SetValue(SwipeRightCommandParameterProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        void IMycoSwipeGestureRecognizerController.SendSwipeLeft(MycoView view)
        {
            if (SwipeLeftCommand != null)
            {
                SwipeLeftCommand.Execute(SwipeLeftCommandParameter);
            }

            if (SwipeLeft != null)
                SwipeLeft(view, new EventArgs());
        }

        void IMycoSwipeGestureRecognizerController.SendSwipeRight(MycoView view)
        {
            if (SwipeRightCommand != null)
            {
                SwipeRightCommand.Execute(SwipeRightCommandParameter);
            }

            if (SwipeRight != null)
                SwipeRight(view, new EventArgs());
        }

        #endregion Methods
    }

    public interface IMycoSwipeGestureRecognizerController
    {
        void SendSwipeLeft(MycoView view);
        void SendSwipeRight(MycoView view);
    }
}