using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using Telerik.Data;
using Telerik.Windows.Controls;
using FoundOps.Common.Tools;
using FoundOps.SLClient.UI.Tools;
using ItemsControl = System.Windows.Controls.ItemsControl;

namespace FoundOps.SLClient.UI.Controls.ImportData
{
    //TODO Setup multiplicity
    /// <summary>
    /// A DataGrid for importing a CSV and classifying the type of data importing.
    /// </summary>
    public partial class ImportDataGrid : INotifyPropertyChanged
    {
        //For creating unique names on extra columns
        private int _customColumnIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDataGrid"/> class.
        /// </summary>
        public ImportDataGrid()
        {
            InitializeComponent();
            if (DesignerProperties.IsInDesignTool) return;

            Observable2.FromPropertyChangedPattern(VM.ImportData, x => x.DataTable).WhereNotNull().ObserveOnDispatcher().Subscribe(_ =>
            {
                //Update the columns whenever the DataTable changes
                this.ImportRadGridView.Columns.Clear();

                //Go through each column and setup the GridView
                foreach (var column in VM.ImportData.DataTable.Columns.Cast<ImportColumn>())
                    this.ImportRadGridView.Columns.Add(CreateColumn(column, column.ColumnName));
            });
        }

        /// <summary>
        /// Creates the ImportDataGrid's import column.
        /// </summary>
        /// <param name="importColumn">The import column to setup a DataGridColumn for.</param>
        /// <param name="columnDisplayName">Display name of the column.</param>
        /// <returns></returns>
        private GridViewDataColumn CreateColumn(ImportColumn importColumn, string columnDisplayName)
        {
            #region Setup the import column header

            var columnHeaderColumnStackPanel = new StackPanel();

            var importColumnTypeComboBox = new RadComboBox { Width = 100, DisplayMemberPath = "DisplayName" };

            //TODO: Setup ComboBox's ItemTemplate to work with Multiplicity

            //Bind the ImportColumnTypeComboBox's itemsource to the import destination's column types
            importColumnTypeComboBox.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("DestinationColumnTypes") { Source = VM.ImportData });

            //Bind the selected item of the ImportColumnTypeComboBox to the ImportColumn.ImportColumnType
            importColumnTypeComboBox.SetBinding(Selector.SelectedItemProperty, new Binding("ImportColumnType") { Mode = BindingMode.TwoWay, Source = importColumn });

            columnHeaderColumnStackPanel.Children.Add(importColumnTypeComboBox);
            columnHeaderColumnStackPanel.Children.Add(new TextBlock
            {
                Text = columnDisplayName,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var columnHeaderStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            columnHeaderStackPanel.Children.Add(columnHeaderColumnStackPanel);
            var removeColumnButton = new Button
            {
                Content = "x",
                VerticalAlignment = VerticalAlignment.Center
            };

            columnHeaderStackPanel.Children.Add(removeColumnButton);

            #endregion

            var gridViewColumn = new GridViewDataColumn
            {
                UniqueName = importColumn.ColumnName,
                DataMemberBinding = new Binding(importColumn.ColumnName),
                //CellTemplate = (DataTemplate)this.Resources["DefaultCellTemplate"],
                //CellEditTemplate = (DataTemplate)this.Resources["DefaultEditTemplate"],
                Header = columnHeaderStackPanel
            };

            //When the remove column button is pressed, remove this column from the DataGrid.
            removeColumnButton.Click += (sender, args) => this.ImportRadGridView.Columns.Remove(gridViewColumn);

            importColumnTypeComboBox.SelectionChanged += (sender, e) =>
            {
                //Check if every column is selected, if so create another column
                var allAreSelected = true;
                foreach (var column in ImportRadGridView.Columns)
                {
                    if (((StackPanel)column.Header).FindChildByType<RadComboBox>().SelectedItem == null)
                        allAreSelected = false;
                }

                //Create another column if all columns are selected
                if (allAreSelected)
                {
                    var newColumnUniqueName = String.Format("CustomColumn{0}", _customColumnIndex);
                    var newColumn = VM.ImportData.AddColumn(newColumnUniqueName);
                    ImportRadGridView.Columns.Add(CreateColumn(newColumn, ""));
                    _customColumnIndex++;
                }
            };

            return gridViewColumn;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                RaisePropertyChanged(name);
            }
        }
        #endregion

        private void ImportRadGridViewCopyingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            e.Value = e.Cell.Item;
        }

        private void ImportRadGridViewPastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            var dataRow = (DataRow)e.Cell.Item;
            dataRow[e.Cell.Column.UniqueName] = e.Value;
        }

        private void ImportRadGridViewPasted(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //Refresh Cells
            ImportRadGridView.Items.Refresh();
            ImportRadGridView.InvalidateArrange();
        }
    }
}