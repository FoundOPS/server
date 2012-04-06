// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.Tools.DynamicallySizedWrapPanel
{
    /// <summary>
    /// Positions child elements sequentially from left to right or top to
    /// bottom.  When elements extend beyond the panel edge, elements are
    /// positioned in the next row or column.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class DynamicallySizedWrapPanel : Panel
    {
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignorePropertyChange;

        #region NumberOfColumns Dependency Property

        /// <summary>
        /// NumberOfColumns
        /// </summary>
        public int NumberOfColumns
        {
            get { return (int)GetValue(NumberOfColumnsProperty); }
            set { SetValue(NumberOfColumnsProperty, value); }
        }

        /// <summary>
        /// NumberOfColumns Dependency Property.
        /// </summary>
        public static readonly DependencyProperty NumberOfColumnsProperty =
            DependencyProperty.Register(
                "NumberOfColumns",
                typeof(int),
                typeof(DynamicallySizedWrapPanel),
                new PropertyMetadata(null));

        #endregion

        #region public double ItemHeight
        /// <summary>
        /// Gets or sets the height of the layout area for each item that is
        /// contained in a <see cref="DynamicallySizedWrapPanel" />.
        /// </summary>
        /// <value>
        /// The height applied to the layout area of each item that is contained
        /// within a <see cref="DynamicallySizedWrapPanel" />.  The
        /// default value is <see cref="F:System.Double.NaN" />.
        /// </value>
        [TypeConverter(typeof(LengthConverter))]
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.DynamicallySizedWrapPanel.ItemHeight" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.DynamicallySizedWrapPanel.ItemHeight" />
        /// dependency property
        /// </value>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(
                "ItemHeight",
                typeof(double),
                typeof(DynamicallySizedWrapPanel),
                new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));
        #endregion public double ItemHeight

        #region public double ItemWidth
        /// <summary>
        /// Gets or sets the width of the layout area for each item that is
        /// contained in a <see cref="DynamicallySizedWrapPanel" />.
        /// </summary>
        /// <value>
        /// The width that applies to the layout area of each item that is
        /// contained in a <see cref="DynamicallySizedWrapPanel" />.
        /// The default value is <see cref="F:System.Double.NaN" />.
        /// </value>
        [TypeConverter(typeof(LengthConverter))]
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.DynamicallySizedWrapPanel.ItemWidth" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.DynamicallySizedWrapPanel.ItemWidth" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(
                "ItemWidth",
                typeof(double),
                typeof(DynamicallySizedWrapPanel),
                new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));
        #endregion public double ItemWidth

        #region public Orientation Orientation
        /// <summary>
        /// Gets or sets the direction in which child elements are arranged.
        /// </summary>
        /// <value>
        /// One of the <see cref="T:System.Windows.Controls.Orientation" />
        /// values.  The default is
        /// <see cref="F:System.Windows.Controls.Orientation.Horizontal" />.
        /// </value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.DynamicallySizedWrapPanel.Orientation" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.DynamicallySizedWrapPanel.Orientation" />
        /// dependency property.
        /// </value>
        // TODO: In WPF, DynamicallySizedWrapPanel uses AddOwner to register the Orientation
        // property via StackPanel.  It then gets the default value of
        // StackPanel's Orientation property.  It looks like this should be no
        // different than using the same default value on a new Orientation
        // property.
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(DynamicallySizedWrapPanel),
                new PropertyMetadata(Orientation.Vertical, OnOrientationPropertyChanged));

        /// <summary>
        /// OrientationProperty property changed handler.
        /// </summary>
        /// <param name="d">DynamicallySizedWrapPanel that changed its Orientation.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Almost always set from the CLR property.")]
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (DynamicallySizedWrapPanel)d;
            var value = (Orientation)e.NewValue;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            // Validate the Orientation
            if ((value != Orientation.Horizontal) &&
                (value != Orientation.Vertical))
            {
                // Reset the property to its original state before throwing
                source._ignorePropertyChange = true;
                source.SetValue(OrientationProperty, (Orientation)e.OldValue);

                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid Orientation value '{0}'.",
                    value);
                throw new ArgumentException(message, "value");
            }

            // Orientation affects measuring.
            source.InvalidateMeasure();
        }
        #endregion public Orientation Orientation

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DynamicallySizedWrapPanel" /> class.
        /// </summary>
        public DynamicallySizedWrapPanel()
        {
        }

        /// <summary>
        /// Property changed handler for ItemHeight and ItemWidth.
        /// </summary>
        /// <param name="d">
        /// DynamicallySizedWrapPanel that changed its ItemHeight or ItemWidth.
        /// </param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Almost always set from the CLR property.")]
        private static void OnItemHeightOrWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (DynamicallySizedWrapPanel)d;
            var value = (double)e.NewValue;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            // Validate the length (which must either be NaN or a positive,
            // finite number)
            if (!value.IsNaN() && ((value <= 0.0) || double.IsPositiveInfinity(value)))
            {
                // Reset the property to its original state before throwing
                source._ignorePropertyChange = true;
                source.SetValue(e.Property, (double)e.OldValue);

                var message = string.Format(
                    CultureInfo.InvariantCulture,
                   "Invalid length value '{0}'.",
                    value);
                throw new ArgumentException(message, "value");
            }

            // The length properties affect measuring.
            source.InvalidateMeasure();
        }

        /// <summary>
        /// Measures the child elements of a
        /// <see cref="DynamicallySizedWrapPanel" /> in anticipation
        /// of arranging them during the
        /// <see cref="DynamicallySizedWrapPanel.ArrangeOverride(System.Windows.Size)" />
        /// pass.
        /// </summary>
        /// <param name="constraint">
        /// The size available to child elements of the wrap panel.
        /// </param>
        /// <returns>
        /// The size required by the
        /// <see cref="DynamicallySizedWrapPanel" /> and its 
        /// elements.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Compat with WPF.")]
        protected override Size MeasureOverride(Size constraint)
        {
            // Variables tracking the size of the current line, the total size
            // measured so far, and the maximum size available to fill.  Note
            // that the line might represent a row or a column depending on the
            // orientation.
            var o = Orientation;
            //var lineSize = new OrientedSize(o);
            var totalSize = new OrientedSize(o);
            //var maximumSize = new OrientedSize(o, constraint.Width, constraint.Height);

            // Determine the constraints for individual items
            var itemWidth = ItemWidth;
            var itemHeight = ItemHeight;
            var hasFixedWidth = !itemWidth.IsNaN();
            var hasFixedHeight = !itemHeight.IsNaN();
            var itemSize = new Size(
                hasFixedWidth ? itemWidth : constraint.Width,
                hasFixedHeight ? itemHeight : constraint.Height);

            var numCols = NumberOfColumns;
            var children = Children;
            var numberOfElements = children.Count;

            // Measure each of the Children
            foreach (var element in Children)
            {
                // Determine the size of the element
                element.Measure(itemSize);
            }


            #region Copied From ArrangeOverride

            var startOfColumnElementIndex = new int[numCols];

            var endOfColumnElementIndex = new int[numCols];

            var tallestHeight = 0;

            //if (children.Count(c => c.DesiredSize.Height > 0) == numCols)
            //{
            //    var i = 0;
            //    for (var counter = 0; counter < children.Count; counter++)
            //    {
            //        if (children[counter].DesiredSize.Height > 0)
            //        {
            //            startOfColumnElementIndex[i] = counter;
            //            endOfColumnElementIndex[i] = counter;
            //            i++;
            //        }
            //    }

            //    foreach (var child in children.Where(child => child.DesiredSize.Height > tallestHeight))
            //        tallestHeight = (int)child.DesiredSize.Height;
                
                
            //    // Return the total size required as an un-oriented quantity
            //    return new Size(totalSize.Width, tallestHeight); 
            //}

            var sizeArray = new int[(children.Where(c => c.DesiredSize.Height > 0)).Count()];
            var count = 0;

            //Iterates through the children to find the height for each and saves them to an arrays
            foreach (var child in Children)
            {
                if ((int)child.DesiredSize.Height != 0)
                {
                    sizeArray[count] = (int)child.DesiredSize.Height;
                    count++;
                }
            }

            //TODO: Figure out mean and standard deviation for the set
            //Had to specify double for the division that occurs below
            var totalChildHeight = sizeArray.Sum();
            double averageChildHeight = 0;
            if (sizeArray.Length > 0)
                averageChildHeight = sizeArray.Average();

            #region Standard Deviation Calculator

            double totalVariance = 0;

            var lengthOfArray = sizeArray.Length;

            var dataAverage = averageChildHeight;

            totalVariance += sizeArray.Sum(childHeight => Math.Pow(childHeight - dataAverage, 2));

            var stDev = lengthOfArray != 0 ? Math.Sqrt(totalVariance / lengthOfArray) : 0;

            #endregion

            //TODO: Put objects in each column until it surpasses any of the requirements

            //The average number of items each column should have
            var averageElementsPerColumn = numberOfElements / numCols;

            //Average height for a column
            var averageColumnHeight = totalChildHeight / numCols;

            startOfColumnElementIndex[0] = 0;
            //Set to negative 1 so that when the loop exits, the tracker will still be in the right position
            var positionTracker = -1;
            var maxHeight = 0;
            var tallestColumn = 0;

            for (var i = 0; i < numCols; i++)
            {
                var thisColHeight = 0;

                startOfColumnElementIndex[i] = (positionTracker + 1);

                var minHeightForColumn = (averageColumnHeight - (.5 * stDev));

                while (thisColHeight < minHeightForColumn && positionTracker < (children.Count - 1)) //(c => c.DesiredSize.Height > 0)
                {
                    positionTracker++;
                    thisColHeight += (int)children[positionTracker].DesiredSize.Height;
                }

                //if (thisColHeight > maxHeightForColumn || startOfColumnElementIndex[i] == positionTracker)
                //    positionTracker--;

                endOfColumnElementIndex[i] = positionTracker;

                if (i == (numCols - 1))
                {
                    endOfColumnElementIndex[i] = positionTracker - 1;
                    //thisColHeight -= (int)children[positionTracker].DesiredSize.Height;
                }
                if (thisColHeight > maxHeight)
                {
                    tallestColumn = i;
                    maxHeight = thisColHeight;
                }
            }
            #endregion

            for (var i = startOfColumnElementIndex[tallestColumn]; i <= endOfColumnElementIndex[tallestColumn]; i++)
            {
                tallestHeight += (int)children[i].DesiredSize.Height;
            }

            // Return the total size required as an un-oriented quantity
            return new Size(totalSize.Width, tallestHeight);
        }

        /// <summary>
        /// Arranges and sizes the
        /// <see cref="DynamicallySizedWrapPanel" /> control and its
        /// child elements.
        /// </summary>
        /// <param name="finalSize">
        /// The area within the parent that the
        /// <see cref="DynamicallySizedWrapPanel" /> should use 
        /// arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used by the
        /// <see cref="DynamicallySizedWrapPanel" />.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            #region Initialize Variables

            // Variables tracking the size of the current line, and the maximum
            // size available to fill.  Note that the line might represent a row
            // or a column depending on the orientation.
            var o = Orientation;
            var lineSize = new OrientedSize(o);
            var maximumSize = new OrientedSize(o, finalSize.Width, finalSize.Height);
            var numCols = NumberOfColumns;

            // Determine the constraints for individual items
            var itemWidth = ItemWidth;
            var itemHeight = ItemHeight;
            var hasFixedWidth = !itemWidth.IsNaN();
            var hasFixedHeight = !itemHeight.IsNaN();
            var indirectOffset = 0.0;
            var directDelta = (o == Orientation.Horizontal) ?
                (hasFixedWidth ? (double?)itemWidth : null) :
                (hasFixedHeight ? (double?)itemHeight : null);



            // Measure each of the Children.  We will process the elements one
            // line at a time, just like during measure, but we will wait until
            // we've completed an entire line of elements before arranging them.
            // The lineStart and lineEnd variables track the size of the
            // currently arranged line.
            var children = Children;
            var numberOfElements = children.Count;

            #endregion

            //TODO: Measure all of the children and save those numbers into an array

            var sizeArray = new int[(children.Where(c => c.DesiredSize.Height > 0)).Count()];
            var count = 0;

            //Iterates through the children to find the height for each and saves them to an arrays
            foreach (var child in Children)
            {
                if ((int)child.DesiredSize.Height != 0)
                {
                    sizeArray[count] = (int)child.DesiredSize.Height;
                    count++;
                }
            }

            //TODO: Figure out mean and standard deviation for the set
            //Had to specify double for the division that occurs below
            var totalChildHeight = sizeArray.Sum();
            double averageChildHeight = 0;
            if (sizeArray.Length > 0)
                averageChildHeight = sizeArray.Average();

            #region Standard Deviation Calculator

            double totalVariance = 0;

            var lengthOfArray = sizeArray.Length;

            var dataAverage = averageChildHeight;

            totalVariance += sizeArray.Sum(childHeight => Math.Pow(childHeight - dataAverage, 2));

            var stDev = lengthOfArray != 0 ? Math.Sqrt(totalVariance / lengthOfArray) : 0;

            #endregion

            //TODO: Put objects in each column until it surpasses any of the requirements

            //The average number of items each column should have
            var averageElementsPerColumn = numberOfElements / numCols;

            //Average height for a column
            var averageColumnHeight = totalChildHeight / numCols;

            var startOfColumnElementIndex = new int[numCols];

            var endOfColumnElementIndex = new int[numCols];

            startOfColumnElementIndex[0] = 0;
            //Set to negative 1 so that when the loop exits, the tracker will still be in the right position
            var positionTracker = -1;
            var maxHeight = 0;
            var tallestColumn = 0;

            for (var i = 0; i < numCols; i++)
            {
                var thisColHeight = 0;

                startOfColumnElementIndex[i] = (positionTracker + 1);

                var minHeightForColumn = (averageColumnHeight - (.5 * stDev));

                while (thisColHeight < minHeightForColumn && positionTracker < (children.Count - 1)) //(c => c.DesiredSize.Height > 0)
                {
                    positionTracker++;
                    thisColHeight += (int)children[positionTracker].DesiredSize.Height;
                }

                //if (thisColHeight > maxHeightForColumn || startOfColumnElementIndex[i] == positionTracker)
                //    positionTracker--;

                endOfColumnElementIndex[i] = positionTracker;

                if (i == (numCols - 1))
                {
                    endOfColumnElementIndex[i] = positionTracker - 1;
                    //thisColHeight -= (int)children[positionTracker].DesiredSize.Height;
                }
                if (thisColHeight > maxHeight)
                {
                    tallestColumn = i;
                    maxHeight = thisColHeight;
                }
            }
            //TODO: Print to the screen
            var colWidth = finalSize.Width / numCols;

            #region Print Elements to the screen
            var tallestChild = 0;

            if (children.Count(c => c.DesiredSize.Height > 0) == numCols)
            {
                var i = 0;
                for (var counter = 0; counter < children.Count; counter++)
                {
                    if (children[counter].DesiredSize.Height > 0)
                    {
                        startOfColumnElementIndex[i] = counter;
                        endOfColumnElementIndex[i] = counter;
                        i++;

                        if (children[counter].DesiredSize.Height > tallestChild)
                        {
                            tallestChild = (int) children[counter].DesiredSize.Height;
                            tallestColumn = i - 1;                        
                        }
                    }
                }
            }
            var lineStart = 0;
            var colCount = 0;

            var firstCol = true;
            //Iterate through each column
            for (var colIndex = 0; colIndex < numCols; colIndex++)
            {
                //Iterate through each element in the column
                for (var elementIndexToPrint = startOfColumnElementIndex[colIndex]; elementIndexToPrint <= endOfColumnElementIndex[colIndex]; elementIndexToPrint++)
                {
                    var element = children[elementIndexToPrint];

                    if (Math.Abs(element.DesiredSize.Height - 0.0) <= 0)
                        continue;

                    // Get the size of the element
                    var elementSize = new OrientedSize(
                        o,
                        hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                        hasFixedHeight ? itemHeight : element.DesiredSize.Height);


                    // If this element falls off the edge of the line
                    if (elementIndexToPrint == endOfColumnElementIndex[colIndex])
                    {
                        #region use this to print things

                        // Then we just completed a line and we should arrange it
                        ArrangeLine(lineStart, ++elementIndexToPrint, directDelta, colWidth, colCount);
                        colCount++;

                        // Advance the start index to a new line after arranging
                        lineStart = elementIndexToPrint;

                        if (firstCol)
                        {
                            lineSize.Indirect = elementSize.Indirect;
                        }

                        firstCol = false;

                        // Move the current element to a new line
                        indirectOffset += lineSize.Indirect;
                        lineSize = elementSize;

                        #endregion

                        lineSize.Indirect = 0;
                        continue;
                    }

                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }

            #endregion

            var tallestHeight = 0;

            for (var i = startOfColumnElementIndex[tallestColumn]; i <= endOfColumnElementIndex[tallestColumn]; i++)
            {
                tallestHeight += (int)children[i].DesiredSize.Height;
            }

            var test = maxHeight;
             
            return new Size(finalSize.Width, tallestHeight);
        }

        /// <summary>
        /// Arrange a sequence of elements in a single line.
        /// </summary>
        /// <param name="lineStart">
        /// Index of the first element in the sequence to arrange.
        /// </param>
        /// <param name="lineEnd">
        /// Index of the last element in the sequence to arrange.
        /// </param>
        /// <param name="directDelta">
        /// Optional fixed growth in the primary direction.
        /// </param>
        /// <param name="colWidth">
        /// Offset of the line in the indirect direction.
        /// </param>
        /// <param name="colCount">
        /// Shared indirect growth of the elements on this line.
        /// </param>
        private void ArrangeLine(int lineStart, int lineEnd, double? directDelta, double colWidth, int colCount)
        {
            var directOffset = 0.0;
            var o = Orientation;
            var isHorizontal = o == Orientation.Horizontal;
            var offset = colCount * colWidth;
            var children = Children;
            for (var index = lineStart; index < lineEnd; index++)
            {
                if (index >= children.Count) continue;
                // Get the size of the element
                var element = children[index];
                var elementSize = new OrientedSize(o, element.DesiredSize.Width, element.DesiredSize.Height);

                // Determine if we should use the element's desired size or the
                // fixed item width or height
                var directGrowth = directDelta != null
                                       ? directDelta.Value
                                       : elementSize.Direct;
                // Arrange the element
                var bounds = isHorizontal ? new Rect(directOffset, offset, directGrowth, colWidth) : new Rect(offset, directOffset, colWidth, directGrowth);
                element.Arrange(bounds);

                directOffset += directGrowth;
            }
        }
    }
}