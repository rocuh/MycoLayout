using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Myco
{
    public class MycoTapGestureRecognizer : MycoGestureRecognizer
    {
        #region Fields

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(MycoTapGestureRecognizer), null);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MycoTapGestureRecognizer), null);
        public static readonly BindableProperty NumberOfTapsRequiredProperty = BindableProperty.Create(nameof(NumberOfTapsRequired), typeof(int), typeof(MycoTapGestureRecognizer), 1);

        #endregion Fields

        #region Events

        public event EventHandler Tapped;

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

        public void SendTapped(MycoView view)
        {
            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }

            if (Tapped != null)
                Tapped(view, new EventArgs());
        }

        #endregion Methods
    }
}