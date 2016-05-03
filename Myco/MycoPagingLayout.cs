using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Myco
{
    [ContentProperty("Children")]
    public class MycoPagingLayout : MycoView
    {
        #region Fields

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(MycoPagingLayout), 0,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: async (bindable, oldValue, newValue) =>
                {
                    await ((MycoPagingLayout)bindable).AnimateToPage((int)newValue);
                });

        private MycoView _currentView;
        private MycoView _nextView;
        private int _visiblePageIndex = -1;

        #endregion Fields

        #region Constructors

        public MycoPagingLayout() : base()
        {
            IsClippedToBounds = true;

            var swipeGestureRecognizer = new MycoSwipeGestureRecognizer();
            swipeGestureRecognizer.SwipeLeft += SwipeGestureRecognizer_SwipeLeft;
            swipeGestureRecognizer.SwipeRight += SwipeGestureRecognizer_SwipeRight;
            GestureRecognizers.Add(swipeGestureRecognizer);

            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (MycoView view in e.OldItems)
                {
                    view.Parent = null;
                }
            }

            if (e.NewItems != null)
            {
                foreach (MycoView view in e.NewItems)
                {
                    view.Parent = this;
                    view.IsVisible = false;
                }
            }

            SetupPage();
        }

        private async void SwipeGestureRecognizer_SwipeLeft(object sender, EventArgs e)
        {
            if (Children != null && SelectedIndex < Children.Count - 1)
            {
                await AnimateToPage(SelectedIndex + 1);
            }
        }

        private async void SwipeGestureRecognizer_SwipeRight(object sender, EventArgs e)
        {
            if (Children != null && SelectedIndex > 0)
            {
                await AnimateToPage(SelectedIndex - 1);
            }
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<MycoView> Children { get; } = new ObservableCollection<MycoView>();

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public async Task AnimateToPage(int index)
        {
            // really animate to new page ?
            if (index >= 0 && index < Children.Count && _visiblePageIndex != index && _nextView == null)
            {
                // get animation direction
                var translation = _visiblePageIndex < index ? Width : -Width;

                // indicate this is the one that is visible
                _visiblePageIndex = index;

                // save selected
                SelectedIndex = index;

                // get view to move to
                _nextView = Children[index];
                _nextView.IsVisible = true;
                _nextView.Layout(new Rectangle(0, 0, Bounds.Width, Bounds.Height));

                // setup the next view offscreen
                _nextView.TranslationX = translation;

                // view to set to once complete
                var targetView = _nextView;
                var oldView = _currentView;

                // animate new in, ol dout
                await Task.WhenAll(_nextView.TranslateTo(0, 0, 250, Easing.CubicInOut), _currentView.TranslateTo(-translation, 0, 250, Easing.CubicInOut));

                // setup the current view
                _currentView = targetView;

                // null out old view
                oldView.IsVisible = false;

                // allow next animation
                _nextView = null;
            }
        }

        public override void GetGestureRecognizers(Point gestureStart, IList<Tuple<MycoView, MycoGestureRecognizer>> matches)
        {
            base.GetGestureRecognizers(gestureStart, matches);

            if (RenderBounds.Contains(gestureStart))
            {
                foreach (var child in Children)
                {
                    child.GetGestureRecognizers(gestureStart, matches);
                }
            }
        }

        protected override void InternalLayout(Rectangle rectangle)
        {
            base.InternalLayout(rectangle);

            foreach (MycoView child in Children)
            {
                child.Layout(new Rectangle(0, 0, Bounds.Width, Bounds.Height));
            }
        }

        protected override void InternalDraw(SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            foreach (MycoView child in Children)
            {
                child.Draw(canvas);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            foreach (var child in Children)
            {
                child.SetInheritedBindingContextReflect(BindingContext);
            }
        }

        private void SetupPage()
        {
            if (_visiblePageIndex != SelectedIndex && SelectedIndex >= 0 && SelectedIndex < Children.Count)
            {
                // only show the current view
                _currentView = Children[SelectedIndex];
                _currentView.IsVisible = true;
                _currentView.Layout(new Rectangle(0, 0, Bounds.Width, Bounds.Height));
                _currentView.TranslationX = 0;

                if (_nextView != null)
                {
                    _nextView.IsVisible = false;
                    _nextView = null;
                }

                _visiblePageIndex = SelectedIndex;

                Invalidate();
            }
        }

        #endregion Methods
    }
}