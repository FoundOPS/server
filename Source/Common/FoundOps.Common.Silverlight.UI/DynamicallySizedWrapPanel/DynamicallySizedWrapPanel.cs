// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

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
            DynamicallySizedWrapPanel source = (DynamicallySizedWrapPanel)d;
            Orientation value = (Orientation)e.NewValue;

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

                string message = string.Format(
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
            DynamicallySizedWrapPanel source = (DynamicallySizedWrapPanel)d;
            double value = (double)e.NewValue;

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

                string message = string.Format(
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
            Orientation o = Orientation;
            //var lineSize = new OrientedSize(o);
            var totalSize = new OrientedSize(o);
            //var maximumSize = new OrientedSize(o, constraint.Width, constraint.Height);

            // Determine the constraints for individual items
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = !itemWidth.IsNaN();
            bool hasFixedHeight = !itemHeight.IsNaN();
            var itemSize = new Size(
                hasFixedWidth ? itemWidth : constraint.Width,
                hasFixedHeight ? itemHeight : constraint.Height);

            // Measure each of the Children
            foreach (UIElement element in Children)
            {
                //    // Determine the size of the element
                element.Measure(itemSize);
            }

            #region Old Method That I dont wanna lose just in case
            //    var elementSize = new OrientedSize(
            //        o,
            //        hasFixedWidth ? itemWidth : element.DesiredSize.Width,
            //        hasFixedHeight ? itemHeight : element.DesiredSize.Height);

            //    // If this element falls of the edge of the line
            //    if (NumericExtensions.IsGreaterThan(lineSize.Direct + elementSize.Direct, maximumSize.Direct))
            //    {
            //        // Update the total size with the direct and indirect growth
            //        // for the current line
            //        totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
            //        totalSize.Indirect += lineSize.Indirect;

            //        // Move the element to a new line
            //        lineSize = elementSize;

            //        // If the current element is larger than the maximum size,
            //        // place it on a line by itself
            //        if (NumericExtensions.IsGreaterThan(elementSize.Direct, maximumSize.Direct))
            //        {
            //            // Update the total size for the line occupied by this
            //            // single element
            //            totalSize.Direct = Math.Max(elementSize.Direct, totalSize.Direct);
            //            totalSize.Indirect += elementSize.Indirect;

            //            // Move to a new line
            //            lineSize = new OrientedSize(o);
            //        }
            //    }
            //    else
            //    {
            //        // Otherwise just add the element to the end of the line
            //        lineSize.Direct += elementSize.Direct;
            //        lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
            //    }


            //// Update the total size with the elements on the last line
            //totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
            //totalSize.Indirect += lineSize.Indirect;

            #endregion

            #region Copied From ArrangeOverride
            var children = Children;
            var numberOfElements = children.Count;
            var thisColHeight = 0;
            var numCols = NumberOfColumns;


            //The element index of the first element in the column
            //startOfColumnElementIndex[0] would be the first element, 0
            var startOfColumnElementIndex = new int[numCols];

            //The element index of the last element in the column
            //endOfColumnElementIndex[numCols-1] would be the last element, numberOfElements-1
            var endOfColumnElementIndex = new int[numCols];


            //Setup starting point
            //Sets up each column to have approx the same number of UI elements
            #region Setup Initial Starting Point

            //The average number of items each column should have
            var averageElementsPerColumn = numberOfElements / numCols;

            //Number of columns that require averageItemsPerColumn+1 items 
            var remainderNumberOfElements = numberOfElements % numCols;

            //The current element index
            var elementIndex = 0;

            for (int currentColumn = 0; currentColumn < numCols; currentColumn++)
            {
                //Set the index of the first element in the current column
                startOfColumnElementIndex[currentColumn] = elementIndex;

                //Add the average number of items a column should have
                //Subtract 1 to adjust for 0 based index
                elementIndex += (averageElementsPerColumn - 1);

                //If there are still columns that require an additional item than the average
                //add an item to the current columm
                if (remainderNumberOfElements > 0)
                {
                    elementIndex++;
                    remainderNumberOfElements--;
                }

                //Set the index of the last element in the current column
                endOfColumnElementIndex[currentColumn] = elementIndex;

                //Add one to the element index to be ready for the next column
                elementIndex++;
            }

            #endregion

            #region Sort items

            var columnWithMaxHeight = 0;
            var columnWithMinHeight = 0;
            var maxHeight = int.MinValue;

            while (true)
            {
                var firstcheck = 0;
                var secondCheck = 0;
                var minHeight = int.MaxValue;
                for (var i = 0; i < numCols; i++)
                {
                    //Calculate height of column
                    for (int j = startOfColumnElementIndex[i]; j <= endOfColumnElementIndex[i]; j++)
                    {
                        int height = (int)children[j].DesiredSize.Height;
                        thisColHeight += height;
                    }

                    if (thisColHeight > maxHeight)
                    {
                        maxHeight = thisColHeight;
                        columnWithMaxHeight = i;
                    }

                    if (thisColHeight < minHeight)
                    {
                        minHeight = thisColHeight;
                        columnWithMinHeight = i;
                    }

                    thisColHeight = 0;
                }

                if (columnWithMaxHeight < columnWithMinHeight)
                {
                    firstcheck = columnWithMaxHeight + 1;
                    endOfColumnElementIndex[columnWithMaxHeight]--;
                    startOfColumnElementIndex[firstcheck]--;
                }
                if (columnWithMaxHeight > columnWithMinHeight)
                {
                    firstcheck = columnWithMaxHeight - 1;
                    startOfColumnElementIndex[columnWithMaxHeight]++;
                    endOfColumnElementIndex[firstcheck]++;
                }

                var firstMinHeight = minHeight;
                var firstMaxCol = columnWithMaxHeight;
                var testCount = 0;

                for (var i = 0; i < numCols; i++)
                {
                    //Calculate height of column
                    for (var j = startOfColumnElementIndex[i]; j <= endOfColumnElementIndex[i]; j++)
                    {
                        var height = (int)children[j].DesiredSize.Height;
                        thisColHeight += height;
                    }

                    if (thisColHeight > maxHeight)
                    {
                        maxHeight = thisColHeight;
                        columnWithMaxHeight = i;
                    }

                    if (thisColHeight < minHeight)
                    {
                        minHeight = thisColHeight;
                        columnWithMinHeight = i;
                    }

                    thisColHeight = 0;
                }

                if (columnWithMaxHeight < columnWithMinHeight)
                {
                    secondCheck = columnWithMaxHeight + 1;
                }
                if (columnWithMaxHeight > columnWithMinHeight)
                {
                    secondCheck = columnWithMaxHeight - 1;
                }

                if (columnWithMinHeight == columnWithMaxHeight)
                    break;

                if (((firstcheck - secondCheck) == 0) && ((firstMaxCol != columnWithMaxHeight) || testCount < numberOfElements))
                    break;
                if (firstMaxCol == columnWithMaxHeight)
                    testCount++;
                if (((firstcheck - secondCheck) == 0) && ((firstMaxCol != columnWithMaxHeight) || testCount < numberOfElements) && (firstMinHeight == minHeight))
                {
                    if (columnWithMaxHeight < columnWithMinHeight)
                    {
                        firstcheck = columnWithMaxHeight + 1;
                        endOfColumnElementIndex[columnWithMaxHeight]--;
                        startOfColumnElementIndex[firstcheck]--;
                    }
                    if (columnWithMaxHeight > columnWithMinHeight)
                    {
                        firstcheck = columnWithMaxHeight - 1;
                        startOfColumnElementIndex[columnWithMaxHeight]++;
                        endOfColumnElementIndex[firstcheck]++;
                    }
                    break;
                }
            }

            #endregion

            #region Calculate New Max Height

            maxHeight = int.MinValue;

            for (int i = 0; i < numCols; i++)
            {
                //Calculate height of column
                for (int j = startOfColumnElementIndex[i]; j <= endOfColumnElementIndex[i]; j++)
                {
                    int height = (int)children[j].DesiredSize.Height;
                    thisColHeight += height;
                }

                if (thisColHeight > maxHeight)
                {
                    maxHeight = thisColHeight;
                }

                thisColHeight = 0;
            }

            #endregion

            #endregion

            // Return the total size required as an un-oriented quantity
            return new Size(totalSize.Width, maxHeight);
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
            // Variables tracking the size of the current line, and the maximum
            // size available to fill.  Note that the line might represent a row
            // or a column depending on the orientation.
            Orientation o = Orientation;
            OrientedSize lineSize = new OrientedSize(o);
            OrientedSize maximumSize = new OrientedSize(o, finalSize.Width, finalSize.Height);
            var numCols = NumberOfColumns;

            // Determine the constraints for individual items
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = !itemWidth.IsNaN();
            bool hasFixedHeight = !itemHeight.IsNaN();
            double indirectOffset = 0;
            double? directDelta = (o == Orientation.Horizontal) ?
                (hasFixedWidth ? (double?)itemWidth : null) :
                (hasFixedHeight ? (double?)itemHeight : null);



            // Measure each of the Children.  We will process the elements one
            // line at a time, just like during measure, but we will wait until
            // we've completed an entire line of elements before arranging them.
            // The lineStart and lineEnd variables track the size of the
            // currently arranged line.
            UIElementCollection children = Children;
            int numberOfElements = children.Count;
            int thisColHeight = 0;


            //The element index of the first element in the column
            //startOfColumnElementIndex[0] would be the first element, 0
            var startOfColumnElementIndex = new int[numCols];

            //The element index of the last element in the column
            //endOfColumnElementIndex[numCols-1] would be the last element, numberOfElements-1
            var endOfColumnElementIndex = new int[numCols];


            //Setup starting point
            //Sets up each column to have approx the same number of UI elements
            #region Setup Initial Starting Point

            //The average number of items each column should have
            int averageElementsPerColumn = numberOfElements / numCols;

            //Number of columns that require averageItemsPerColumn+1 items 
            int remainderNumberOfElements = numberOfElements % numCols;

            //The current element index
            int elementIndex = 0;

            for (int currentColumn = 0; currentColumn < numCols; currentColumn++)
            {
                //Set the index of the first element in the current column
                startOfColumnElementIndex[currentColumn] = elementIndex;

                //Add the average number of items a column should have
                //Subtract 1 to adjust for 0 based index
                elementIndex += (averageElementsPerColumn - 1);

                //If there are still columns that require an additional item than the average
                //add an item to the current columm
                if (remainderNumberOfElements > 0)
                {
                    elementIndex++;
                    remainderNumberOfElements--;
                }

                //Set the index of the last element in the current column
                endOfColumnElementIndex[currentColumn] = elementIndex;

                //Add one to the element index to be ready for the next column
                elementIndex++;
            }

            #endregion

            #region Sort items

            int columnWithMaxHeight = 0;
            int columnWithMinHeight = 0;
            int maxHeight = int.MinValue;

            while (true)
            {
                int firstcheck = 0;
                int secondCheck = 0;
                int minHeight = int.MaxValue;
                for (int i = 0; i < numCols; i++)
                {
                    //Calculate height of column
                    for (int j = startOfColumnElementIndex[i]; j <= endOfColumnElementIndex[i]; j++)
                    {
                        int height = (int)children[j].DesiredSize.Height;
                        thisColHeight += height;
                    }

                    if (thisColHeight > maxHeight)
                    {
                        maxHeight = thisColHeight;
                        columnWithMaxHeight = i;
                    }

                    if (thisColHeight < minHeight)
                    {
                        minHeight = thisColHeight;
                        columnWithMinHeight = i;
                    }

                    thisColHeight = 0;
                }

                if (columnWithMaxHeight < columnWithMinHeight)
                {
                    firstcheck = columnWithMaxHeight + 1;
                    endOfColumnElementIndex[columnWithMaxHeight]--;
                    startOfColumnElementIndex[firstcheck]--;
                }
                if (columnWithMaxHeight > columnWithMinHeight)
                {
                    firstcheck = columnWithMaxHeight - 1;
                    startOfColumnElementIndex[columnWithMaxHeight]++;
                    endOfColumnElementIndex[firstcheck]++;
                }

                var firstMinHeight = minHeight;
                var firstMaxCol = columnWithMaxHeight;
                var testCount = 0;

                for (int i = 0; i < numCols; i++)
                {
                    //Calculate height of column
                    for (int j = startOfColumnElementIndex[i]; j <= endOfColumnElementIndex[i]; j++)
                    {
                        int height = (int)children[j].DesiredSize.Height;
                        thisColHeight += height;
                    }

                    if (thisColHeight > maxHeight)
                    {
                        maxHeight = thisColHeight;
                        columnWithMaxHeight = i;
                    }

                    if (thisColHeight < minHeight)
                    {
                        minHeight = thisColHeight;
                        columnWithMinHeight = i;
                    }

                    thisColHeight = 0;
                }

                if (columnWithMaxHeight < columnWithMinHeight)
                {
                    secondCheck = columnWithMaxHeight + 1;
                }
                if (columnWithMaxHeight > columnWithMinHeight)
                {
                    secondCheck = columnWithMaxHeight - 1;
                }

                if (columnWithMinHeight == columnWithMaxHeight)
                    break;

                if (((firstcheck - secondCheck) == 0) && ((firstMaxCol != columnWithMaxHeight) || testCount < numberOfElements))
                    break;
                if (firstMaxCol == columnWithMaxHeight)
                    testCount++;
                if (((firstcheck - secondCheck) == 0) && ((firstMaxCol != columnWithMaxHeight) || testCount < numberOfElements) && (firstMinHeight == minHeight))
                {
                    if (columnWithMaxHeight < columnWithMinHeight)
                    {
                        firstcheck = columnWithMaxHeight + 1;
                        endOfColumnElementIndex[columnWithMaxHeight]--;
                        startOfColumnElementIndex[firstcheck]--;
                    }
                    if (columnWithMaxHeight > columnWithMinHeight)
                    {
                        firstcheck = columnWithMaxHeight - 1;
                        startOfColumnElementIndex[columnWithMaxHeight]++;
                        endOfColumnElementIndex[firstcheck]++;
                    }
                    break;
                }
            }

            #endregion

            #region Calculate New Max Height

            maxHeight = int.MinValue;

            for (int i = 0; i < numCols; i++)
            {
                //Calculate height of column
                for (int j = startOfColumnElementIndex[i]; j <= endOfColumnElementIndex[i]; j++)
                {
                    int height = (int)children[j].DesiredSize.Height;
                    thisColHeight += height;
                }

                if (thisColHeight > maxHeight)
                {
                    maxHeight = thisColHeight;
                }

                thisColHeight = 0;
            }

            #endregion


            //checks for new number of columns
            for (int i = (numCols - 1); i > 0; i--)
            {
                if (startOfColumnElementIndex[i] >= numberOfElements)
                    numCols--;
            }

            #region left Justify the solution
            //Start at left side of solution and move one from second column to first
            //check is column height is higher than the max, if yes then move it back
            if (children.Count > numCols)
            {
                for (int twice = 0; twice < 2; twice++)
                {
                    for (int i = 0; i < (numCols - 1); i++)
                    {
                        for (int j = startOfColumnElementIndex[i]; j <= endOfColumnElementIndex[i]; j++)
                        {
                            if (j < children.Count)
                            {
                                int height = (int)children[j].DesiredSize.Height;
                                thisColHeight += height;
                            }
                        }
                        while (thisColHeight <= maxHeight)
                        {

                            endOfColumnElementIndex[i]++;
                            startOfColumnElementIndex[i + 1]++;
                            if (endOfColumnElementIndex[i] >= numberOfElements)
                            {
                                endOfColumnElementIndex[i]--;
                                startOfColumnElementIndex[i + 1]--;
                                break;
                            }
                            thisColHeight += (int)children[endOfColumnElementIndex[i]].DesiredSize.Height;
                            if (thisColHeight > maxHeight)
                            {
                                endOfColumnElementIndex[i]--;
                                startOfColumnElementIndex[i + 1]--;
                                break;
                            }
                        }
                        if (startOfColumnElementIndex[i] > endOfColumnElementIndex[i])
                            endOfColumnElementIndex[i] = startOfColumnElementIndex[i];
                        thisColHeight = 0;
                    }
                }
            }
            #endregion

            //checks for new number of columns
            for (int i = (numCols - 1); i > 0; i--)
            {
                if (startOfColumnElementIndex[i] >= numberOfElements)
                    numCols--;
            }

            var colWidth = finalSize.Width / NumberOfColumns;

            #region Print Elements to the screen

            int lineStart = 0;
            double maxWidth = 0;
            int colCount = 0;
            int initialNumberOfColumns = NumberOfColumns;

            bool firstCol = true;
            //Iterate through each column
            for (int colIndex = 0; colIndex < numCols; colIndex++)
            {
                //Iterate through each element in the column
                for (int elementIndexToPrint = startOfColumnElementIndex[colIndex]; elementIndexToPrint <= endOfColumnElementIndex[colIndex]; elementIndexToPrint++)
                {
                    UIElement element = children[elementIndexToPrint];

                    // Get the size of the element
                    var elementSize = new OrientedSize(
                        o,
                        hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                        hasFixedHeight ? itemHeight : element.DesiredSize.Height);

                    if (elementSize.Indirect > maxWidth)
                        maxWidth = elementSize.Indirect;

                    // If this element falls of the edge of the line
                    if (elementIndexToPrint == endOfColumnElementIndex[colIndex])
                    {
                        #region use this to print things

                        // Then we just completed a line and we should arrange it
                        ArrangeLine(lineStart, ++elementIndexToPrint, directDelta, colWidth, colCount);
                        colCount++;
                        //// If the current element is larger than the maximum size
                        //if (startOfColumnElementIndex[colIndex] == endOfColumnElementIndex[colIndex])
                        //{
                        //    // Arrange the element as a single line
                        //    ArrangeLine(elementIndexToPrint, ++elementIndexToPrint, directDelta, indirectOffset, maxWidth);

                        //    // Move to a new line
                        //    indirectOffset += lineSize.Indirect;
                        //    lineSize = new OrientedSize(o);
                        //}
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
                        maxWidth = 0;
                        continue;
                    }

                    // Otherwise just add the element to the end of the line
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }
            for (int i = numCols + 1; i < initialNumberOfColumns; i++)
            {
                var offset = colCount * colWidth;
                var rectangle = new Rectangle();
                var rect = new Rect(offset, 0, colWidth, 0);
                rectangle.Arrange(rect);
            }
            //Not sure if this is used
            #region use this to print the last column

            //// Arrange any elements on the last line
            //if (lineStart < numberOfElements)
            //{
            //    startOfColumnElementIndex[startOfColumnElementIndex.Count() - 1] = lineStart;

            //    ArrangeLine(startOfColumnElementIndex[startOfColumnElementIndex.Count() - 1], numberOfElements, directDelta, indirectOffset, lineSize.Indirect);
            //}
            #endregion

            #endregion

            return new Size(finalSize.Width, maxHeight); ;
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
            double directOffset = 0.0;
            Orientation o = Orientation;
            bool isHorizontal = o == Orientation.Horizontal;
            var offset = colCount * colWidth;
            UIElementCollection children = Children;
            for (int index = lineStart; index < lineEnd; index++)
            {
                if (index < children.Count)
                {
                    // Get the size of the element
                    UIElement element = children[index];
                    OrientedSize elementSize = new OrientedSize(o, element.DesiredSize.Width, element.DesiredSize.Height);

                    // Determine if we should use the element's desired size or the
                    // fixed item width or height
                    double directGrowth = directDelta != null
                                              ? directDelta.Value
                                              : elementSize.Direct;
                    // Arrange the element
                    Rect bounds;
                    if (isHorizontal) bounds = new Rect(directOffset, offset, directGrowth, colWidth);
                    else bounds = new Rect(offset, directOffset, colWidth, directGrowth);
                    element.Arrange(bounds);

                    directOffset += directGrowth;
                }
            }
        }
    }
}