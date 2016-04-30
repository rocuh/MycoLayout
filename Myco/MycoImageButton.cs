using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Myco
{
    public class MycoImageButton : MycoImage
    {
        #region Fields

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(MycoImageButton), null);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MycoImageButton), null);

        #endregion Fields

        #region Constructors

        public MycoImageButton()
        {
            var tapGestureRecoginizer = new MycoTapGestureRecognizer()
            {
                Command = TransitionCommand
            };

            GestureRecognizers.Add(tapGestureRecoginizer);
        }

        #endregion Constructors

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

        private ICommand TransitionCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await this.ScaleTo(0.8, 50, Easing.Linear);
                    await Task.Delay(100);
                    await this.ScaleTo(1, 50, Easing.Linear);

                    if (Command != null)
                    {
                        Command.Execute(CommandParameter);
                    }
                });
            }
        }

        #endregion Properties
    }
}