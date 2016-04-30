using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Myco
{
    public static class MycoViewExtensions
    {
        #region Methods

        public static Task<bool> ScaleTo(this MycoView view, double scale, uint length = 250, Easing easing = null)
        {
            if (view == null)
                throw new ArgumentNullException("mycoview");
            if (easing == null)
                easing = Easing.Linear;

            var tcs = new TaskCompletionSource<bool>();
            var weakView = new WeakReference<MycoView>(view);
            Action<double> _scale = f =>
            {
                MycoView v;
                if (weakView.TryGetTarget(out v))
                    v.Scale = f;
            };

            new Animation(_scale, view.Scale, scale, easing).Commit(view, "ScaleTo", 16, length, finished: (f, a) => tcs.SetResult(a));

            return tcs.Task;
        }

        public static Task<bool> TranslateTo(this MycoView view, double x, double y, uint length = 250, Easing easing = null)
        {
            if (view == null)
                throw new ArgumentNullException("mycoview");
            easing = easing ?? Easing.Linear;

            var tcs = new TaskCompletionSource<bool>();
            var weakView = new WeakReference<MycoView>(view);
            Action<double> translateX = f =>
            {
                MycoView v;
                if (weakView.TryGetTarget(out v))
                    v.TranslationX = f;
            };
            Action<double> translateY = f =>
            {
                MycoView v;
                if (weakView.TryGetTarget(out v))
                    v.TranslationY = f;
            };
            new Animation { { 0, 1, new Animation(translateX, view.TranslationX, x) }, { 0, 1, new Animation(translateY, view.TranslationY, y) } }.Commit(view, "TranslateTo", 16, length, easing,
                (f, a) => tcs.SetResult(a));

            return tcs.Task;
        }

        #endregion Methods
    }
}