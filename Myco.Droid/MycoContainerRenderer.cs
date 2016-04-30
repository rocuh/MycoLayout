using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Myco.MycoContainer), typeof(Myco.Droid.MycoContainerRenderer))]

namespace Myco.Droid
{
    public class MycoContainerRenderer : ViewRenderer<MycoContainer, NativeMycoContainer>
    {
        #region Constructors

        public MycoContainerRenderer()
        {
        }

        #endregion Constructors

        #region Methods

        protected override void OnElementChanged(ElementChangedEventArgs<MycoContainer> e)
        {
            base.OnElementChanged(e);

            var oldControl = Control;

            if (Control == null)
            {
                var nativeSkContainer = new NativeMycoContainer(Context);
                SetNativeControl(nativeSkContainer);
            }

            if (e.OldElement != null)
            {
                e.NewElement.Layout(e.OldElement.Bounds);
                (e.NewElement as IMycoController).SendLayout();
            }

            Control.Container = e.NewElement;
            Control.Invalidate();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == MycoContainer.ContentProperty.PropertyName)
            {
                Control.Invalidate();
            }
        }

        #endregion Methods
    }
}