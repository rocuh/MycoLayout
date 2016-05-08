using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Myco
{
    public class MycoTapGestureRecognizer : MycoGestureRecognizer, IMycoTapGestureRecognizerController
    {
        #region Fields

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(MycoTapGestureRecognizer), null);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MycoTapGestureRecognizer), null);
        public static readonly BindableProperty NumberOfTapsRequiredProperty = BindableProperty.Create(nameof(NumberOfTapsRequired), typeof(int), typeof(MycoTapGestureRecognizer), 1);

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

        public int NumberOfTapsRequired
        {
            get
            {
                return (int)GetValue(NumberOfTapsRequiredProperty);
            }
            set
            {
                SetValue(NumberOfTapsRequiredProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        void IMycoTapGestureRecognizerController.SendTapped(MycoView view, double x, double y)
        {
            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }

            if (Tapped != null)
                Tapped(view, new TapEventArgs(x, y));
        }

        #endregion Methods
    }

    public interface IMycoTapGestureRecognizerController
    {
        int NumberOfTapsRequired { get; }
        void SendTapped(MycoView view, double x, double y);
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