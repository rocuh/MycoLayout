using SkiaSharp;
using System;
using Xamarin.Forms;

namespace Myco
{
    public class MycoLabel : MycoView
    {
        #region Fields

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(MycoLabel), FontAttributes.None,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
               ((MycoLabel)bindableObject).Invalidate();
            });

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MycoLabel), null,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoLabel)bindableObject).Invalidate();
            });

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int), typeof(MycoLabel), 10,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoLabel)bindableObject).Invalidate();
            });

        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(MycoLabel), TextAlignment.Start,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoLabel)bindableObject).Invalidate();
            });

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MycoLabel), Color.Black,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoLabel)bindableObject).Invalidate();
            });

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MycoLabel), String.Empty,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoLabel)bindableObject).Invalidate();
            });

        public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(nameof(VerticalTextAlignment), typeof(TextAlignment), typeof(MycoLabel), TextAlignment.Start,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoLabel)bindableObject).Invalidate();
            });

        private SKTypeface _typeFace;
        private FontAttributes _typeFaceFontAttributes;
        private string _typeFaceFontFamily;

        #endregion Fields

        #region Properties

        public FontAttributes FontAttributes
        {
            get
            {
                return (FontAttributes)GetValue(FontAttributesProperty);
            }
            set
            {
                SetValue(FontAttributesProperty, value);
            }
        }

        public string FontFamily
        {
            get
            {
                return (string)GetValue(FontFamilyProperty);
            }
            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }

        public int FontSize
        {
            get
            {
                return (int)GetValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }

        public TextAlignment HorizontalTextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
            }
            set
            {
                SetValue(HorizontalTextAlignmentProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public Color TextColor
        {
            get
            {
                return (Color)GetValue(TextColorProperty);
            }
            set
            {
                SetValue(TextColorProperty, value);
            }
        }

        public TextAlignment VerticalTextAlignment
        {
            get
            {
                return (TextAlignment)GetValue(VerticalTextAlignmentProperty);
            }
            set
            {
                SetValue(VerticalTextAlignmentProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        protected override void InternalDraw(SKCanvas canvas)
        {
            base.InternalDraw(canvas);

            if (!String.IsNullOrEmpty(Text))
            {
                var renderBounds = RenderBounds;

                using (var paint = new SKPaint())
                {
                    paint.Typeface = GetTypeface();
                    paint.TextSize = FontSize;
                    paint.IsAntialias = true;
                    paint.Color = TextColor.ToSKColor();

                    var labelWidth = paint.MeasureText(Text);
                    var labelHeight = Math.Abs(paint.FontMetrics.Descent) + Math.Abs(paint.FontMetrics.Ascent);

                    double x = 0;

                    switch (HorizontalTextAlignment)
                    {
                        case TextAlignment.Start:
                            x = renderBounds.X;
                            break;

                        case TextAlignment.End:
                            x = renderBounds.Right - labelWidth;
                            break;

                        case TextAlignment.Center:
                            x = renderBounds.X + (renderBounds.Width / 2.0) - (labelWidth / 2.0);
                            break;
                    }

                    double y = 0;

                    switch (VerticalTextAlignment)
                    {
                        case TextAlignment.Start:
                            y = renderBounds.Top + labelHeight;
                            break;

                        case TextAlignment.End:
                            y = renderBounds.Bottom;
                            break;

                        case TextAlignment.Center:
                            y = renderBounds.Y + (renderBounds.Height / 2.0) + (labelHeight / 2.0);
                            break;
                    }

                    canvas.DrawText(Text, (float)x, (float)y - paint.FontMetrics.Descent, paint);
                }
            }
        }

        protected override Size InternalSizeRequest(double widthConstraint, double heightConstaint)
        {
            var size = base.InternalSizeRequest(widthConstraint, heightConstaint);

            if (!String.IsNullOrEmpty(Text))
            {
                float labelWidth, labelHeight;

                using (var paint = new SKPaint())
                {
                    paint.Typeface = GetTypeface();
                    paint.TextSize = FontSize;
                    paint.IsAntialias = true;
                    labelWidth = paint.MeasureText(Text);
                    labelHeight = Math.Abs(paint.FontMetrics.Descent) + Math.Abs(paint.FontMetrics.Ascent);
                }

                return new Size(Double.IsPositiveInfinity(size.Width) ? labelWidth : Math.Min(labelWidth, size.Width), Double.IsPositiveInfinity(size.Height) ? labelHeight : Math.Min(labelHeight, size.Height));
            }

            return size;
        }

        private SKTypeface GetTypeface()
        {
            if (_typeFace == null || _typeFaceFontFamily != FontFamily || _typeFaceFontAttributes != FontAttributes)
            {
                SKTypefaceStyle style = SKTypefaceStyle.Normal;

                switch (FontAttributes)
                {
                    case FontAttributes.None:
                        style = SKTypefaceStyle.Normal;
                        break;

                    case FontAttributes.Bold:
                        style = SKTypefaceStyle.Bold;
                        break;

                    case FontAttributes.Italic:
                        style = SKTypefaceStyle.Italic;
                        break;
                }

                _typeFace = SKTypeface.FromFamilyName(FontFamily == null ? Device.OnPlatform("Helvetica", "sans-serif", "Arial") : FontFamily, style);

                _typeFaceFontFamily = FontFamily;
                _typeFaceFontAttributes = FontAttributes;
            }

            return _typeFace;
        }

        #endregion Methods
    }
}