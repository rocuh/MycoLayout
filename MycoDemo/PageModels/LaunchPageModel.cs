using FreshMvvm;
using Xamarin.Forms;

namespace MycoDemo
{
    public class LaunchPageModel : FreshBasePageModel
    {
        #region Properties

        public Command ShowMycoCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await CoreMethods.PushPageModel<MycoDemoPageModel>();
                });
            }
        }

        public Command ShowPagingLayoutCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await CoreMethods.PushPageModel<MycoPagingLayoutPageModel>();
                });
            }
        }

        public Command ShowPagingViewCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await CoreMethods.PushPageModel<MycoPagingViewPageModel>();
                });
            }
        }

        public Command ShowXamarinFormsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await CoreMethods.PushPageModel<FormsDemoPageModel>();
                });
            }
        }

        #endregion Properties
    }
}