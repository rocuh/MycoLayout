using SkiaSharp;
using System;
using System.Collections;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Myco
{
    public class MycoPagingView : MycoView
    {
        #region Fields

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(object), typeof(MycoPagingView), null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((MycoPagingView)bindable).SetupPage();
            });

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(MycoPagingView), null);

        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(MycoPagingView), 0,
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((MycoPagingView)bindable).SetupPage();
                });

        private MycoView _currentView;
        private MycoView _nextView;
        private int _visiblePageIndex = -1;

        #endregion Fields

        #region Constructors

        public MycoPagingView() : base()
        {
            IsClippedToBounds = true;

            var swipeGestureRecognizer = new MycoSwipeGestureRecognizer();
            swipeGestureRecognizer.SwipeLeft += SwipeGestureRecognizer_SwipeLeft;
            swipeGestureRecognizer.SwipeRight += SwipeGestureRecognizer_SwipeRight;
            GestureRecognizers.Add(swipeGestureRecognizer);
        }

        private async void SwipeGestureRecognizer_SwipeLeft(object sender, EventArgs e)
        {
            if (ItemsSource != null && SelectedIndex < ItemsSource.Count - 1)
            {
                await AnimateToPage(SelectedIndex + 1);
            }
        }

        private async void SwipeGestureRecognizer_SwipeRight(object sender, EventArgs e)
        {
            if (ItemsSource != null && SelectedIndex > 0)
            {
                await AnimateToPage(SelectedIndex - 1);
            }
        }

        #endregion Constructors

        #region Properties

        public IList ItemsSource
        {
            get
            {
                return (IList)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ItemTemplateProperty);
            }
            set
            {
                base.SetValue(ItemTemplateProperty, value);
            }
        }

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
            if (SelectedIndex != index && _visiblePageIndex != index)
            {
                // indicate this is the one that is visible
                _visiblePageIndex = index;

                // get view to move to
                _nextView = (MycoView)ItemTemplate.CreateContent();
                _nextView.BindingContext = ItemsSource[index];
                _nextView.Parent = this;
                _nextView.Layout(new Rectangle(0, 0, Bounds.Width, Bounds.Height));

                // get animation direction
                var translation = SelectedIndex < index ? Width : -Width;

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
                oldView.Parent = null;
                oldView = null;

                // save selected
                SelectedIndex = index;
            }
        }

        protected override void InternalLayout(Rectangle rectangle)
        {
            base.InternalLayout(rectangle);

            if (_currentView != null)
            {
                _currentView.Layout(new Rectangle(0, 0, rectangle.Width, rectangle.Height));
            }

            if (_nextView != null)
            {
                _nextView.Layout(new Rectangle(0, 0, rectangle.Width, rectangle.Height));
            }
        }

        protected override void InternalDraw(SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            if (_currentView != null)
            {
                _currentView.Draw(canvas);
            }

            if (_nextView != null)
            {
                _nextView.Draw(canvas);
            }
        }

        private void SetupPage()
        {
            if (_visiblePageIndex != SelectedIndex)
            {
                _currentView = (MycoView)ItemTemplate.CreateContent();
                _currentView.BindingContext = ItemsSource[SelectedIndex];
                _currentView.Parent = this;
                _currentView.Layout(new Rectangle(0, 0, Bounds.Width, Bounds.Height));
                _currentView.TranslationX = 0;

                if (_nextView != null)
                {
                    _nextView.Parent = null;
                    _nextView = null;
                }

                _visiblePageIndex = SelectedIndex;

                Invalidate();
            }
        }

        #endregion Methods
    }
}