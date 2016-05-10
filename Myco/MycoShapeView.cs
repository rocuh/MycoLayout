using SkiaSharp;
using Xamarin.Forms;

namespace Myco
{
    public enum MycoShapes
    {
        Rectangle,
        RoundedRectangle,
        Oval,
    }

    public class MycoShapeView : MycoView
    {
        #region Fields

        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(nameof(HasShadow), typeof(bool), typeof(MycoCheckImage), false,
           propertyChanged: (bindableObject, oldValue, newValue) =>
           {
               ((MycoShapeView)bindableObject).Invalidate();
           });

        public static readonly BindableProperty ShapeColorProperty = BindableProperty.Create(nameof(ShapeColor), typeof(Color), typeof(MycoCheckImage), Color.Accent,
           propertyChanged: (bindableObject, oldValue, newValue) =>
           {
               ((MycoShapeView)bindableObject).Invalidate();
           });

        public static readonly BindableProperty ShapeProperty = BindableProperty.Create(nameof(Shape), typeof(MycoShapes), typeof(MycoShapeView), MycoShapes.Rectangle,
           propertyChanged: (bindableObject, oldValue, newValue) =>
           {
               ((MycoShapeView)bindableObject).Invalidate();
           });

        private const float RoundedRectRadius = 6;

        #endregion Fields

        #region Properties

        public bool HasShadow
        {
            get
            {
                return (bool)GetValue(HasShadowProperty);
            }
            set
            {
                SetValue(HasShadowProperty, value);
            }
        }

        public MycoShapes Shape
        {
            get
            {
                return (MycoShapes)GetValue(ShapeProperty);
            }
            set
            {
                SetValue(ShapeProperty, value);
            }
        }

        public Color ShapeColor
        {
            get
            {
                return (Color)GetValue(ShapeColorProperty);
            }
            set
            {
                SetValue(ShapeColorProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void InternalDraw(SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;

                if (HasShadow)
                {
                    using (var filter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 2.0f))
                    {
                        paint.MaskFilter = filter;
                        paint.Color = Color.Gray.ToSKColor();

                        switch (Shape)
                        {
                            case MycoShapes.Rectangle:
                                canvas.DrawRect(RenderBounds.ToSKRect(), paint);
                                break;

                            case MycoShapes.RoundedRectangle:
                                canvas.DrawRoundRect(RenderBounds.ToSKRect(), RoundedRectRadius, RoundedRectRadius, paint);
                                break;

                            case MycoShapes.Oval:
                                canvas.DrawOval(RenderBounds.ToSKRect(), paint);
                                break;
                        }

                        paint.MaskFilter = null;
                    }
                }

                paint.Color = ShapeColor.ToSKColor();

                switch (Shape)
                {
                    case MycoShapes.Rectangle:
                        canvas.DrawRect(RenderBounds.ToSKRect(), paint);
                        break;

                    case MycoShapes.RoundedRectangle:
                        canvas.DrawRoundRect(RenderBounds.ToSKRect(), RoundedRectRadius, RoundedRectRadius, paint);
                        break;

                    case MycoShapes.Oval:
                        canvas.DrawOval(RenderBounds.ToSKRect(), paint);
                        break;
                }
            }
        }

        #endregion Methods
    }
}