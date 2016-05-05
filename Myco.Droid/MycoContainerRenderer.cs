using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Myco.MycoContainer), typeof(Myco.Droid.MycoContainerRenderer))]

namespace Myco.Droid
{
    public class MycoContainerRenderer : ViewRenderer<MycoContainer, NativeMycoContainer>
    {
        #region Fields

        private bool _disposed;

        #endregion Fields

        #region Constructors

        public MycoContainerRenderer()
        {
        }

        #endregion Constructors

        #region Methods

        public static void Initialize()
        {
            DependencyService.Register<IMycoImageSource, MycoImageSource>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed && Control != null)
            {
                Control.Container = null;
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MycoContainer> e)
        {
            base.OnElementChanged(e);

            var oldControl = Control;

            if (Control == null)
            {
                var nativeSkContainer = new NativeMycoContainer(Context);
                SetNativeControl(nativeSkContainer);

                var inViewCell = GetContainingViewCell(e.NewElement);

                if (inViewCell != null && inViewCell.Parent is Xamarin.Forms.ListView)
                {
                    var renderer = Platform.GetRenderer(inViewCell.Parent as Xamarin.Forms.ListView) as ListViewCleanupRenderer;
                    renderer.Cleanup += HandleCleanup;
                }
            }

            if (e.NewElement != null && e.OldElement != null)
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

        private ViewCell GetContainingViewCell(Xamarin.Forms.Element element)
        {
            Element parentElement = element.Parent;

            if (parentElement == null)
                return null;

            if (typeof(ViewCell).IsAssignableFrom(parentElement.GetType()))
                return (ViewCell)parentElement;
            else
                return GetContainingViewCell(parentElement);
        }

        private void HandleCleanup(object sender, EventArgs args)
        {
            Dispose(true);
        }

        #endregion Methods
    }
}