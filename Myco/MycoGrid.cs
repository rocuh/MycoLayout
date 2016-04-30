using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Myco
{
    [ContentProperty("Children")]
    public class MycoGrid : MycoView
    {
        #region Fields

        public static readonly BindableProperty ColumnDefintionsProperty = BindableProperty.Create(nameof(ColumnDefinitions), typeof(IList<ColumnDefinition>), typeof(MycoGrid), null);
        public static readonly BindableProperty ColumnProperty = BindableProperty.CreateAttached("Column", typeof(int), typeof(MycoGrid), 0, BindingMode.OneWay);
        public static readonly BindableProperty ColumnSpacingProperty = BindableProperty.Create(nameof(ColumnSpacing), typeof(double), typeof(MycoGrid), 6.0);
        public static readonly BindableProperty ColumnSpanProperty = BindableProperty.CreateAttached("ColumnSpan", typeof(int), typeof(MycoGrid), 1, BindingMode.OneWay);
        public static readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(MycoGrid), new Thickness(0, 0, 0, 0));
        public static readonly BindableProperty RowDefinitionsProperty = BindableProperty.Create(nameof(RowDefinitions), typeof(IList<RowDefinition>), typeof(MycoGrid), null);
        public static readonly BindableProperty RowProperty = BindableProperty.CreateAttached("Row", typeof(int), typeof(MycoGrid), 0, BindingMode.OneWay);
        public static readonly BindableProperty RowSpacingProperty = BindableProperty.Create(nameof(RowSpacing), typeof(double), typeof(MycoGrid), 6.0);
        public static readonly BindableProperty RowSpanProperty = BindableProperty.CreateAttached("RowSpan", typeof(int), typeof(MycoGrid), 1, BindingMode.OneWay);

        #endregion Fields

        #region Constructors

        public MycoGrid()
        {
            RowDefinitions = new List<RowDefinition>();
            ColumnDefinitions = new List<ColumnDefinition>();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<MycoView> Children { get; } = new ObservableCollection<MycoView>();

        public IList<ColumnDefinition> ColumnDefinitions
        {
            get
            {
                return (IList<ColumnDefinition>)GetValue(ColumnDefintionsProperty);
            }
            set
            {
                SetValue(ColumnDefintionsProperty, value);
            }
        }

        public double ColumnSpacing
        {
            get
            {
                return (double)GetValue(ColumnSpacingProperty);
            }
            set
            {
                SetValue(ColumnSpacingProperty, value);
            }
        }

        public Thickness Padding
        {
            get
            {
                return (Thickness)GetValue(PaddingProperty);
            }
            set
            {
                SetValue(PaddingProperty, value);
            }
        }

        public IList<RowDefinition> RowDefinitions
        {
            get
            {
                return (IList<RowDefinition>)GetValue(RowDefinitionsProperty);
            }
            set
            {
                SetValue(RowDefinitionsProperty, value);
            }
        }

        public double RowSpacing
        {
            get
            {
                return (double)GetValue(RowSpacingProperty);
            }
            set
            {
                SetValue(RowSpacingProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public static int GetColumn(BindableObject bindable)
        {
            return (int)bindable.GetValue(MycoGrid.ColumnProperty);
        }

        public static int GetColumnSpan(BindableObject bindable)
        {
            return (int)bindable.GetValue(MycoGrid.ColumnSpanProperty);
        }

        public static int GetRow(BindableObject bindable)
        {
            return (int)bindable.GetValue(MycoGrid.RowProperty);
        }

        public static int GetRowSpan(BindableObject bindable)
        {
            return (int)bindable.GetValue(MycoGrid.RowSpanProperty);
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!IsVisible)
                return;

            foreach (var child in Children)
            {
                child.Draw(canvas);
            }
        }

        public override void GetGestureRecognizers(Point gestureStart, IList<Tuple<MycoView, MycoGestureRecognizer>> matches)
        {
            if (Bounds.Contains(gestureStart))
            {
                foreach (var child in Children)
                {
                    child.GetGestureRecognizers(gestureStart, matches);
                }
            }
        }

        public override void Layout(Rectangle rectangle)
        {
            base.Layout(rectangle);

            if (!IsVisible)
                return;

            double embeddedWidth, embeddedHeight;
            double[] columnWidths;
            double[] rowHeights;
            double[] columnSpacing;
            double[] rowSpacing;

            // adjust available size
            rectangle.Left += Padding.Left;
            rectangle.Top += Padding.Top;
            rectangle.Width -= (Padding.Right + Padding.Left);
            rectangle.Height -= (Padding.Bottom + Padding.Top);

            // get minimum size the grid can fit into
            CalculateRowColumns(out columnWidths, out rowHeights, out columnSpacing, out rowSpacing, out embeddedWidth, out embeddedHeight);

            // work how much of the percentage room is used by column spacing
            double totalPercentColumnSpacing = 0;

            for (int c = 0; c < ColumnDefinitions.Count; c++)
            {
                if (ColumnDefinitions[c].Width.IsStar)
                {
                    totalPercentColumnSpacing += (c > 0 ? ColumnSpacing : 0);
                }
            }
            double totalPercentRowSpacing = 0;

            // work how much of the percentage room is used by row spacing
            for (int r = 0; r < RowDefinitions.Count; r++)
            {
                if (RowDefinitions[r].Height.IsStar)
                {
                    totalPercentRowSpacing += (r > 0 ? RowSpacing : 0);
                }
            }

            // determine how much is left for the percentage based columns and rows
            var widthAutoSize = (rectangle.Width - totalPercentColumnSpacing) - embeddedWidth;
            var heightAutoSize = (rectangle.Height - totalPercentRowSpacing) - embeddedHeight;

            var totalStarWidths = GetTotalStarWidths();

            // setup the remaining column widths
            for (int c = 0; c < ColumnDefinitions.Count; c++)
            {
                if (ColumnDefinitions[c].Width.IsStar)
                {
                    columnWidths[c] = ((ColumnDefinitions[c].Width.Value / totalStarWidths) * widthAutoSize) + (c > 0 ? ColumnSpacing : 0);
                }
            }

            var totalStarHeights = GetTotalStarHeights();

            // setup the remaining  row heights
            for (int r = 0; r < RowDefinitions.Count; r++)
            {
                if (RowDefinitions[r].Height.IsStar)
                {
                    rowHeights[r] = ((RowDefinitions[r].Height.Value / totalStarHeights) * heightAutoSize) + (r > 0 ? RowSpacing : 0);
                }
            }

            // position the cells
            foreach (var child in Children)
            {
                var row = GetRow(child);
                var column = GetColumn(child);
                var rowSpan = GetRowSpan(child);
                var columnSpan = GetColumnSpan(child);

                double columnLeft = rectangle.Left, rowTop = rectangle.Top, columnRight = 0, rowBottom = 0;

                for (int c = 0; c < column; c++)
                {
                    columnLeft += columnWidths[c];
                }

                for (int r = 0; r < row; r++)
                {
                    rowTop += rowHeights[r];
                }

                columnRight = columnLeft;
                rowBottom = rowTop;

                for (int c = column; c < column + columnSpan; c++)
                {
                    columnRight += columnWidths[c];
                }

                for (int r = row; r < row + rowSpan; r++)
                {
                    rowBottom += rowHeights[r];
                }

                columnLeft += columnSpacing[column];
                rowTop += rowSpacing[row];

                var size = child.SizeRequest(columnRight - columnLeft, rowBottom - rowTop);

                double left = columnLeft, top = rowTop, right = columnRight, bottom = rowBottom;

                // pinned to left and right
                if (child.HorizontalOptions.Alignment == LayoutAlignment.Fill)
                {
                    left = columnLeft + child.Margin.Left;
                    right = columnRight - child.Margin.Right;
                }

                // pinned to left
                if (child.HorizontalOptions.Alignment == LayoutAlignment.Start)
                {
                    left = columnLeft + child.Margin.Left;
                    right = left + size.Width;
                }

                // pinned to right
                if (child.HorizontalOptions.Alignment == LayoutAlignment.End)
                {
                    right = columnRight - child.Margin.Right;
                    left = right - size.Width;
                }

                // unpinnded both left and right
                if (child.HorizontalOptions.Alignment == LayoutAlignment.Center)
                {
                    left = columnLeft + (columnRight - columnLeft) / 2.0 - size.Width / 2.0;
                    right = left + size.Width;
                }

                // pinned top and bottom
                if (child.VerticalOptions.Alignment == LayoutAlignment.Fill)
                {
                    top = rowTop + child.Margin.Top;
                    bottom = rowBottom - child.Margin.Bottom;
                }

                // pinned top
                if (child.VerticalOptions.Alignment == LayoutAlignment.Start)
                {
                    top = rowTop + child.Margin.Top;
                    bottom = top + size.Height;
                }

                // pinned bottom
                if (child.VerticalOptions.Alignment == LayoutAlignment.End)
                {
                    bottom = rowBottom - child.Margin.Bottom;
                    top = bottom - size.Height;
                }

                // unpinned both top and bottom
                if (child.VerticalOptions.Alignment == LayoutAlignment.Center)
                {
                    top = rowTop + (rowBottom - rowTop) / 2.0 - size.Height / 2.0;
                    bottom = top + size.Height;
                }

                child.Layout(new Rectangle(left, top, (right - left) + 1, (bottom - top) + 1));
            }
        }

        public override Size SizeRequest(double widthConstraint, double heightConstaint)
        {
            if (!IsVisible)
                return new Size(0, 0);

            var size = base.SizeRequest(widthConstraint, heightConstaint);

            double embeddedWidth, embeddedHeight;
            double[] columnWidths;
            double[] rowHeights;
            double[] columnSpacing;
            double[] rowSpacing;

            // gets the minimum height of the grid
            CalculateRowColumns(out columnWidths, out rowHeights, out columnSpacing, out rowSpacing, out embeddedWidth, out embeddedHeight);

            embeddedWidth += Padding.Left + Padding.Right;
            embeddedHeight += Padding.Top + Padding.Bottom;

            return new Size(Double.IsPositiveInfinity(size.Width) ? embeddedWidth : size.Width, Double.IsPositiveInfinity(size.Height) ? embeddedHeight : size.Height);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            foreach (var child in Children)
            {
                child.BindingContext = BindingContext;
            }
        }

        private void CalculateRowColumns(out double[] columnWidths, out double[] rowHeights, out double[] columnSpacing, out double[] rowSpacing, out double embeddedWidth, out double embeddedHeight)
        {
            embeddedWidth = 0;
            embeddedHeight = 0;

            columnWidths = new double[ColumnDefinitions.Count];
            rowHeights = new double[RowDefinitions.Count];
            columnSpacing = new double[ColumnDefinitions.Count];
            rowSpacing = new double[RowDefinitions.Count];

            var contentWidths = new double[ColumnDefinitions.Count];
            var contentHeights = new double[RowDefinitions.Count];

            foreach (var child in Children)
            {
                bool allAutoSizeColunn = true;

                var row = GetRow(child);
                var column = GetColumn(child);
                var rowSpan = GetRowSpan(child);
                var columnSpan = GetColumnSpan(child);
                var size = child.SizeRequest(double.PositiveInfinity, double.PositiveInfinity);

                for (int c = column; c < column + columnSpan; c++)
                {
                    if (ColumnDefinitions[c].Width.IsStar)
                    {
                        allAutoSizeColunn = false;
                    }
                }

                // determine autosize widths and heights
                for (int c = column; c < column + columnSpan; c++)
                {
                    if (ColumnDefinitions[c].Width.IsAuto && allAutoSizeColunn)
                    {
                        contentWidths[c] = Math.Max(size.Width, contentWidths[c]);
                        columnWidths[c] = Math.Max(columnWidths[c], Double.IsPositiveInfinity(size.Width) ? 0 : (size.Width / columnSpan));
                    }
                    else if (ColumnDefinitions[c].Width.IsAbsolute)
                    {
                        contentWidths[c] = Math.Max(ColumnDefinitions[c].Width.Value, contentWidths[c]);
                        columnWidths[c] = ColumnDefinitions[c].Width.Value;
                    }
                }

                bool allAutoSizeRow = true;

                for (int r = row; r < row + rowSpan; r++)
                {
                    if (RowDefinitions[r].Height.IsStar)
                    {
                        allAutoSizeRow = false;
                    }
                }

                for (int r = row; r < row + rowSpan; r++)
                {
                    if (RowDefinitions[r].Height.IsAuto && allAutoSizeRow)
                    {
                        contentHeights[r] = Math.Max(size.Height, contentHeights[r]);
                        rowHeights[r] = Math.Max(rowHeights[r], Double.IsPositiveInfinity(size.Height) ? 0 : (size.Height / rowSpan));
                    }
                    else if (RowDefinitions[r].Height.IsAbsolute)
                    {
                        contentHeights[r] = Math.Max(RowDefinitions[r].Height.Value, contentHeights[r]);
                        rowHeights[r] = RowDefinitions[r].Height.Value;
                    }
                }
            }

            // only put spacing into a column if it has any content
            // it can still have a column width as a view may span into it
            for (int c = 0; c < ColumnDefinitions.Count; c++)
            {
                if (c > 0 && (contentWidths[c] > 0 || ColumnDefinitions[c].Width.IsStar))
                {
                    columnWidths[c] += ColumnSpacing;
                    columnSpacing[c] = ColumnSpacing;
                }
            }

            // only put spacing into a row if it has any content
            // it can still have a row height as a view may span into it
            for (int r = 0; r < RowDefinitions.Count; r++)
            {
                if (r > 0 && (contentHeights[r] > 0 || RowDefinitions[r].Height.IsStar))
                {
                    rowHeights[r] += RowSpacing;
                    rowSpacing[r] = RowSpacing;
                }
            }

            // sum up minimum width required for this layout
            for (int c = 0; c < ColumnDefinitions.Count; c++)
            {
                if (!ColumnDefinitions[c].Width.IsStar)
                {
                    embeddedWidth += columnWidths[c];
                }
            }

            // sum up minimum height required for this layout
            for (int r = 0; r < RowDefinitions.Count; r++)
            {
                if (!RowDefinitions[r].Height.IsStar)
                {
                    embeddedHeight += rowHeights[r];
                }
            }
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
                }
            }
        }

        private double GetTotalStarHeights()
        {
            double total = 0;

            for (int r = 0; r < RowDefinitions.Count; r++)
            {
                if (RowDefinitions[r].Height.IsStar)
                {
                    total += RowDefinitions[r].Height.Value;
                }
            }

            return total;
        }

        private double GetTotalStarWidths()
        {
            double total = 0;

            for (int c = 0; c < ColumnDefinitions.Count; c++)
            {
                if (ColumnDefinitions[c].Width.IsStar)
                {
                    total += ColumnDefinitions[c].Width.Value;
                }
            }

            return total;
        }

        #endregion Methods
    }
}