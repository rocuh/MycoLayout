using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MycoDemo
{
    public class LaunchPageModel : FreshBasePageModel
    {
        public Command ShowXamarinFormsCommand
        {
            get
            {
                return new Command(async () => {
                    await CoreMethods.PushPageModel<FormsDemoPageModel>();
                });
            }
        }

        public Command ShowMycoCommand
        {
            get
            {
                return new Command(async () => {
                    await CoreMethods.PushPageModel<MycoDemoPageModel>();
                });
            }
        }
    }
}
