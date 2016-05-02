using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Myco
{
    [ContentProperty("Content")]
    public class MycoContentView : MycoView
    {
        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(MycoView), typeof(MycoContentView), null,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoContentView)bindableObject).Invalidate();
            });

        public MycoView Content
        {
            get
            {
                return (MycoView)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public override void Layout(Rectangle rectangle)
        {
            base.Layout(rectangle);

            if (Content != null)
            {
                Content.Layout(new Rectangle(0,0, rectangle.Width, rectangle.Height));
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (Content != null)
            {
                Content.SetInheritedBindingContextReflect(BindingContext);
            }
        }
    }
}
