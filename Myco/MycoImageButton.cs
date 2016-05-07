using System.Threading.Tasks;
using System.Windows.Input;
using SkiaSharp;
using Xamarin.Forms;
using System;

namespace Myco
{
    public class MycoImageButton : MycoImage
    {
        #region Fields

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(MycoImageButton), null);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MycoImageButton), null);

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(MycoImageButton), FontAttributes.None,
           propertyChanged: (bindableObject, oldValue, newValue) =>
           {
               ((MycoImageButton)bindableObject).Invalidate();
           });

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MycoImageButton), null,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoImageButton)bindableObject).Invalidate();
            });

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int), typeof(MycoImageButton), 18,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoImageButton)bindableObject).Invalidate();
            });

        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(MycoImageButton), TextAlignment.Center,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoImageButton)bindableObject).Invalidate();
            });

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MycoImageButton), Color.Black,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoImageButton)bindableObject).Invalidate();
            });

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MycoImageButton), String.Empty,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoImageButton)bindableObject).Invalidate();
            });

        public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(nameof(VerticalTextAlignment), typeof(TextAlignment), typeof(MycoImageButton), TextAlignment.Center,
            propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                ((MycoImageButton)bindableObject).Invalidate();
            });

        private SKTypeface _typeFace;
        private FontAttributes _typeFaceFontAttributes;
        private string _typeFaceFontFamily;

        #endregion Fields

        #region Constructors

        public MycoImageButton()
        {
            var tapGestureRecoginizer = new MycoTapGestureRecognizer()
            {
                Command = TransitionCommand
            };

            GestureRecognizers.Add(tapGestureRecoginizer);
        }

        #endregion Constructors

        #region Properties

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

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

        private ICommand TransitionCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await this.ScaleTo(0.9, 50, Easing.CubicOut);
                    await Task.Delay(100);
                    await this.ScaleTo(1, 50, Easing.CubicOut);

                    if (Command != null)
                    {
                        Command.Execute(CommandParameter);
                    }
                });
            }
        }

        #endregion Properties

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
    }
}