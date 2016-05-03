using CoreGraphics;
using SkiaSharp;
using System;
using UIKit;

namespace Myco.iOS
{
    public class NativeMycoContainer : UIView
    {
        #region Fields

        private const int BitmapInfo = ((int)CGBitmapFlags.ByteOrder32Big) | ((int)CGImageAlphaInfo.PremultipliedLast);

        private IntPtr _buff = IntPtr.Zero;
        private int _bufferHeight;
        private int _bufferWidth;
        private MycoContainer _mycoContainer;

        #endregion Fields

        #region Constructors

        public NativeMycoContainer()
        {
            Opaque = false;
            UserInteractionEnabled = true;
        }

        #endregion Constructors

        #region Destructors

        ~NativeMycoContainer()
        {
            Dispose(false);
        }

        #endregion Destructors

        #region Properties

        public MycoContainer Container
        {
            get
            {
                return _mycoContainer;
            }

            set
            {
                if (_mycoContainer != null)
                {
                    _mycoContainer.InvalidateNative -= _skContainer_InvalidateNative;
                }

                _mycoContainer = value;

                if (_mycoContainer != null)
                {
                    _mycoContainer.InvalidateNative += _skContainer_InvalidateNative;
                }
            }
        }

        #endregion Properties

        #region Methods

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            var screenScale = UIScreen.MainScreen.Scale;
            var width = (int)(Bounds.Width * screenScale);
            var height = (int)(Bounds.Height * screenScale);

            if (_buff == IntPtr.Zero || width != _bufferWidth || height != _bufferHeight)
            {
                if (_buff != IntPtr.Zero)
                {
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(_buff);
                }

                _buff = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(width * height * 4);

                _bufferWidth = width;
                _bufferHeight = height;

                (_mycoContainer as IMycoController).SendSurfaceSize(width, height);
            }

            using (var surface = SKSurface.Create(width, height, SKColorType.N_32, SKAlphaType.Premul, _buff, width * 4))
            {
                var skcanvas = surface.Canvas;
                skcanvas.Scale((float)screenScale, (float)screenScale);
                using (new SKAutoCanvasRestore(skcanvas, true))
                {
                    (_mycoContainer as IMycoController).SendDraw(skcanvas);
                }
            }

            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            using (var bContext = new CGBitmapContext(_buff, width, height, 8, width * 4, colorSpace, (CGImageAlphaInfo)BitmapInfo))
            using (var image = bContext.ToImage())
            using (var context = UIGraphics.GetCurrentContext())
            {
                // flip the image for CGContext.DrawImage
                context.TranslateCTM(0, Frame.Height);
                context.ScaleCTM(1, -1);
                context.ClearRect(Bounds);
                context.DrawImage(Bounds, image);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            (_mycoContainer as IMycoController).SendLayout();

            SetNeedsDisplay();
        }

        protected override void Dispose(bool disposing)
        {
            if (_buff != IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(_buff);
                _buff = IntPtr.Zero;
            }

            base.Dispose(disposing);
        }

        private void _skContainer_InvalidateNative(object sender, EventArgs e)
        {
            SetNeedsDisplay();
        }

        #endregion Methods
    }
}