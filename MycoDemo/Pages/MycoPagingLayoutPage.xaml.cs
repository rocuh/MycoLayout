using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MycoDemo
{
    public partial class MycoPagingLayoutPage : ContentPage
    {
        public MycoPagingLayoutPage()
        {
            InitializeComponent();

            this.PrevButton.Clicked += PrevButton_Clicked;
            this.NextButton.Clicked += NextButton_Clicked;
        }

        private async void PrevButton_Clicked(object sender, EventArgs e)
        {
            if (Carousel.SelectedIndex > 0)
            {
                await Carousel.AnimateToPage(Carousel.SelectedIndex - 1);
            }
        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            if (Carousel.SelectedIndex < Carousel.Children.Count - 1)
            {
                await Carousel.AnimateToPage(Carousel.SelectedIndex + 1);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            BindingContext = null;
        }
    }
}
