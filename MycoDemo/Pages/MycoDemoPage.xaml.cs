using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MycoDemo
{
    public partial class MycoDemoPage : ContentPage
    {
        public MycoDemoPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            BindingContext = null;
        }
    }
}
