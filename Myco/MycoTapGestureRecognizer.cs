using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Myco
{
    public class MycoTapGestureRecognizer : MycoGestureRecognizer
    {
        #region Fields

        private const int TapMaximum = 500;

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(MycoTapGestureRecognizer), null);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MycoTapGestureRecognizer), null);

        private DateTime _downTime;

        #endregion Fields

        #region Events

        public event EventHandler<TapEventArgs> Tapped;

        #endregion Events

        #region Properties

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnTouchDown(MycoView view, double x, double y)
        {
            base.OnTouchDown(view, x, y);
            _downTime = DateTime.Now;
        }

        protected override void OnTouchUp(MycoView view, double x, double y, bool canceled)
        {
            base.OnTouchUp(view, x, y, canceled);

            if ((DateTime.Now - _downTime).TotalMilliseconds < TapMaximum && !canceled)
            {
                if (Command != null)
                {
                    Command.Execute(CommandParameter);
                }

                if (Tapped != null)
                    Tapped(view, new TapEventArgs(x, y));
            }
        }

        #endregion Methods
    }

    public class TapEventArgs : EventArgs
    {
        #region Constructors

        public TapEventArgs(double x, double y)
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
}