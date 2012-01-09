using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FoundOps.Common.Silverlight.UI.DynamicallySizedWrapPanel
{
    public class DynamicWrapPanelTools
    {
        /// <summary>
        /// Sets the starting points for columns.
        /// </summary>
        /// <param name="numCols">The number of columns.</param>
        /// <param name="averageElementsPerColumn">The average elements per column.</param>
        /// <param name="remainderNumberOfElements">The number of elements requiring an extra elemtnts.</param>
        /// <returns></returns>
        public static List<int[]> SetStartingPointsForColumns(int numCols, int averageElementsPerColumn, int remainderNumberOfElements)
        {
            //The current element index
            int elementIndex = 0;

            //The element index of the first element in the column
            //startOfColumnElementIndex[0] would be the first element, 0
            var startIndicies = new int[numCols];

            //The element index of the last element in the column
            //endOfColumnElementIndex[numCols-1] would be the last element, numberOfElements-1
            var endIndicies = new int[numCols];

            for (var currentColumn = 0; currentColumn < numCols; currentColumn++)
            {
                //Set the index of the first element in the current column
                startIndicies[currentColumn] = elementIndex;

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
                endIndicies[currentColumn] = elementIndex;

                //Add one to the element index to be ready for the next column
                elementIndex++;
            }

            var indexArray = new List<int[]> { startIndicies, endIndicies };

            return indexArray;
        }

        /// <summary>
        /// Calculates the height of column.
        /// </summary>
        /// <param name="i">The starting point.</param>
        /// <param name="startOfColumnElementIndex">Start index of the of column element.</param>
        /// <param name="endOfColumnElementIndex">End index of the of column element.</param>
        /// <param name="children">The children.</param>
        /// <returns></returns>
        public static int CalculateHeightOfColumn(int i, int[] startOfColumnElementIndex, int[] endOfColumnElementIndex, UIElementCollection children)
        {
            var thisColHeight = 0;
            for (var j = startOfColumnElementIndex[i]; j <= endOfColumnElementIndex[i]; j++)
            {
                var height = (int)children[j].DesiredSize.Height;
                thisColHeight += height;
            }

            return thisColHeight;
        }
    }
}
