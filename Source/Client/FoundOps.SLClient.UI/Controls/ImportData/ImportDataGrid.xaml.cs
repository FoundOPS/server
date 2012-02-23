using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Reactive.Linq;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using FoundOps.Common.Tools;
using FoundOps.SLClient.UI.Tools;
using Telerik.Windows.Controls;
using FoundOps.Common.Silverlight.Models.Import;
using FoundOps.Common.Silverlight.Models.DataTable;
using ItemsControl = System.Windows.Controls.ItemsControl;

namespace FoundOps.SLClient.UI.Controls.ImportData
{
    //TODO Setup multiplicity
    /// <summary>
    /// 
    /// </summary>
    public partial class ImportDataGrid : INotifyPropertyChanged
    {
        //For creating unique names on extra columns
        private int _customColumnIndex;
        private readonly DataRowColumnToValueConverter<ValueWithOptionalAssociation> _dataRowColumnToValueConverter = new DataRowColumnToValueConverter<ValueWithOptionalAssociation>();

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
                foreach (var column in VM.ImportData.DataTable.Columns.OfType<ImportColumn>()) //All are ImportColumns
                    this.ImportRadGridView.Columns.Add(CreateColumn(column, column.ColumnName));
            });
        }

        private GridViewDataColumn CreateColumn(ImportColumn importColumn, string columnDisplayName)
        {
            string uniqueColumnName = importColumn.ColumnName;

            var columnHeaderColumnStackPanel = new StackPanel();

            var importColumnTypeComboBox = new RadComboBox { Width = 100, DisplayMemberPath = "DisplayName" };

            ////Setup ComboBox's ItemTemplate to work with Multiplicity
            //importColumnTypeComboBox.ItemTemplate = (DataTemplate)this.Resources["ImportDestinationComboBoxTemplate"];

            importColumnTypeComboBox.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("DestinationColumnTypes") { Source = VM.ImportData });
            //Bind the ImportColumnType to the DataTable's ImportColumn
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

            var gridViewColumn = new GridViewDataColumn
            {
                UniqueName = uniqueColumnName,
                DataMemberBinding = new Binding { Converter = _dataRowColumnToValueConverter, ConverterParameter = uniqueColumnName },
                CellTemplate = (DataTemplate)this.Resources["DefaultCellTemplate"],
                CellEditTemplate = (DataTemplate)((ImportDataGridCellTemplateConverter)this.Resources["ImportDataGridCellTemplateConverter"]).Convert(importColumnTypeComboBox.SelectedValue, null, null, null),
                Header = columnHeaderStackPanel
            };

            removeColumnButton.Click += (sender, args) =>
                                            {
                                                importColumn.ImportColumnType = null;
                                                this.ImportRadGridView.Columns.Remove(gridViewColumn);
                                            };

            importColumnTypeComboBox.SelectionChanged += (sender, e) =>
            {
                //Setup the cell template
                gridViewColumn.CellEditTemplate =
                      (DataTemplate)((ImportDataGridCellTemplateConverter)this.Resources["ImportDataGridCellTemplateConverter"]).Convert(importColumnTypeComboBox.SelectedValue, null, null, null);
  
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

                //TODO: update multiplicity

                ////Refresh the UI
                //ImportRadGridView.Items.Refresh();
                //ImportRadGridView.InvalidateArrange();
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
            var dataRow = (DataRow<ValueWithOptionalAssociation>)e.Cell.Item;
            e.Value = ((ValueWithOptionalAssociation)_dataRowColumnToValueConverter.Convert(dataRow, null, e.Cell.Column.UniqueName, null)).Value;
        }

        private void ImportRadGridViewPastingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            var dataRow = (DataRow<ValueWithOptionalAssociation>)e.Cell.Item;
            dataRow[e.Cell.Column.UniqueName] = new ValueWithOptionalAssociation { Value = e.Value };
        }

        private void ImportRadGridViewPasted(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //Refresh Cells
            ImportRadGridView.Items.Refresh();
            ImportRadGridView.InvalidateArrange();
        }
    }

    public class ImportDataGridCellTemplateConverter : IValueConverter
    {
        #region Public Properties

        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ClientNameTemplate { get; set; }
        public DataTemplate ClientNameAssociationTemplate { get; set; }

        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var importColumnType = (ImportColumnType)value;
            if (importColumnType == null)
                return DefaultTemplate;

            var importDestination = VM.ImportData.ImportDestination;

            if (importColumnType.Type == ImportColumnEnum.ClientName)
            {
                return importDestination == ImportDestination.Clients
                           ? DefaultTemplate
                           : ClientNameAssociationTemplate;
            }

            return DefaultTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}