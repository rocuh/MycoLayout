using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ListView), typeof(Myco.Droid.ListViewCleanupRenderer))]

namespace Myco.Droid
{
    /// <summary>
    /// It appears that the listview cell renderers do not get explictly disposed on android, as these can be relatively large with MycoLayout
    /// this wrapper renderer will trigger a dispose on the MycoContainerRenderer
    /// </summary>
    public class ListViewCleanupRenderer : ListViewRenderer
    {
        #region Constructors

        public ListViewCleanupRenderer()
        {
        }

        #endregion Constructors

        #region Events

        public event EventHandler<EventArgs> Cleanup;

        #endregion Events

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing && Cleanup != null)
            {
                Cleanup(this, new EventArgs());
            }

            base.Dispose(disposing);
        }

        #endregion Methods
    }
}