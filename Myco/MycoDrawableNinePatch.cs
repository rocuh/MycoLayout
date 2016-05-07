using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Myco
{
    public class MycoDrawableNinePatch : IMycoDrawable
    {
        #region Fields

        private List<PatchSection> _horizontalSections;
        private List<PatchSection> _verticalSections;
        private double _stretchableHeight;
        private double _stretchableWidth;
        private SKColor[] _pixels;

        #endregion Fields

        #region Constructors

        public MycoDrawableNinePatch(SKBitmap bitmap)
        {
            Bitmap = bitmap;
            ProcessNinePatch();
        }

        #endregion Constructors

        #region Enums

        private enum RenderMode
        {
            Fixed,
            Stretch
        }

        #endregion Enums

        #region Properties

        public SKBitmap Bitmap { get; private set; }

        #endregion Properties

        #region Methods

        public void Draw(SKCanvas canvas, Rectangle rectangle, SKPaint paint = null)
        {
            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

            var fixedWidth = Bitmap.Width - 2 - _stretchableWidth;
            var fixedHeight = Bitmap.Height - 2 - _stretchableHeight;
            double totalVariableWidth = rectangle.Width - fixedWidth;
            double totalVariableHeight = rectangle.Height - fixedHeight;
            double remainingVariableHeight = totalVariableHeight;

            double y = rectangle.Y, yb = 1;
            int tileIndex = 0;

            foreach (var vs in _verticalSections)
            {
                double sh = CalcSectionSize(vs, totalVariableHeight, _stretchableHeight, ref remainingVariableHeight);

                double x = rectangle.X, xb = 1;
                double remainingVariableWidth = totalVariableWidth;

                foreach (var hs in _horizontalSections)
                {
                    var sourceRegion = new Rectangle(xb, yb, hs.Size, vs.Size);
                    double sw = CalcSectionSize(hs, totalVariableWidth, _stretchableWidth, ref remainingVariableWidth);
                    var targetRegion = new Rectangle(x, y, sw, sh);

                    canvas.DrawBitmap(Bitmap, sourceRegion.ToSKRect(), targetRegion.ToSKRect(), paint);

                    x += sw;
                    xb += hs.Size;
                    tileIndex++;
                }

                yb += vs.Size;
                y += sh;
            }
        }

        public Size GetSize(double targetWidth, double targetHeight)
        {
            var size = new Size();

            if (Double.IsPositiveInfinity(targetWidth))
            {
                size.Width = Bitmap.Width;
            }
            else
            {
                size.Width = targetWidth;
            }

            if (Double.IsPositiveInfinity(targetHeight))
            {
                size.Height = Bitmap.Height;
            }
            else
            {
                size.Height = targetHeight;
            }

            return size;
        }

        private double CalcSectionSize(PatchSection sec, double totalVariable, double stretchableSize, ref double remainingVariable)
        {
            if (sec.Mode != RenderMode.Fixed)
            {
                double sw = Math.Round(totalVariable * (sec.Size / stretchableSize));

                if (sw > remainingVariable)
                    sw = remainingVariable;

                remainingVariable -= sw;

                return sw;
            }
            else
            {
                return sec.Size;
            }
        }

        private List<PatchSection> CreateSections(IEnumerable<SKColor> pixels)
        {
            List<PatchSection> sections = new List<PatchSection>();

            PatchSection section = null;
            int n = 0;

            foreach (var p in pixels)
            {
                RenderMode mode;

                if (p.Red == 0 && p.Blue == 0 && p.Green == 0 && p.Alpha == 255)
                    mode = RenderMode.Stretch;
                else
                    mode = RenderMode.Fixed;

                if (section == null || mode != section.Mode)
                {
                    section = new PatchSection
                    {
                        Start = n,
                        Size = 1,
                        Mode = mode
                    };
                    sections.Add(section);
                }
                else
                {
                    section.Size++;
                }

                n++;
            }
            return sections;
        }

        private SKColor GetPixel(int x, int y)
        {
            // Bitmap.GetPixel crashes currently
            return _pixels[x + y * Bitmap.Width];
        }

        private void ProcessNinePatch()
        {
            _pixels = Bitmap.Pixels;

            _horizontalSections = CreateSections(Enumerable.Range(1, (int)Bitmap.Width - 2).Select(n => GetPixel(n, 0)));
            _verticalSections = CreateSections(Enumerable.Range(1, (int)Bitmap.Height - 2).Select(n => GetPixel(0, n)));
            _stretchableWidth = _horizontalSections.Where(s => s.Mode != RenderMode.Fixed).Sum(s => s.Size);
            _stretchableHeight = _verticalSections.Where(s => s.Mode != RenderMode.Fixed).Sum(s => s.Size);

            _pixels = null;
        }

        #endregion Methods

        #region Classes

        private class PatchSection
        {
            #region Fields

            public RenderMode Mode;
            public double Size;
            public int Start;

            #endregion Fields
        }

        #endregion Classes
    }
}