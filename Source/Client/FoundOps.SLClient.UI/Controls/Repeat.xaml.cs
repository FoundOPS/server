using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using FoundOps.Common.Composite;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Framework.Views.Converters;
using Telerik.Windows.Controls;

namespace FoundOps.Framework.Views.Controls
{
    public partial class Repeat : UserControl
    {
        public Repeat()
        {
            InitializeComponent();
            VisualStateManager.GoToState(this, "Once", true);
        }

        private void NeverRadioButtonChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Value == null) return;

            Value.EndDate = null;
            Value.EndAfterTimes = null;
        }

        #region Value Dependency Property

        /// <summary>
        /// Value
        /// </summary>
        public Core.Models.CoreEntities.Repeat Value
        {
            get { return (Core.Models.CoreEntities.Repeat)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Value Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(Core.Models.CoreEntities.Repeat),
                typeof(Repeat),
                new PropertyMetadata(new PropertyChangedCallback(ValueChanged)));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as Repeat;
            if (c == null) return;

            var oldRepeat = e.OldValue as Core.Models.CoreEntities.Repeat;
            var newRepeat = e.NewValue as Core.Models.CoreEntities.Repeat;

            c.UpdateEndDateComboBoxes(newRepeat);

            if (oldRepeat != null)
                oldRepeat.PropertyChanged -= c.ValuePropertyChanged;
            if (newRepeat != null)
                newRepeat.PropertyChanged += c.ValuePropertyChanged;
        }

        private void ValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EndDate" || e.PropertyName == "EndAfterTimes")
                UpdateEndDateComboBoxes((Core.Models.CoreEntities.Repeat)sender);
        }

        void UpdateEndDateComboBoxes(Core.Models.CoreEntities.Repeat repeat)
        {
            if (repeat == null) return;
            //Manually update End Date comboboxes because of binding issues
            var endOfRepeatToIsCheckedConverter = new EndOfRepeatToIsCheckedConverter();
            var values = new object[] { repeat.EndAfterTimes, repeat.EndDate };

            NeverRadioButton.IsChecked = (bool?)endOfRepeatToIsCheckedConverter.Convert(values, typeof(bool?), "NeverEnd", null);
            AfterRadioButton.IsChecked = (bool?)endOfRepeatToIsCheckedConverter.Convert(values, typeof(bool?), "EndAfterTimes", null);
            OnRadioButton.IsChecked = (bool?)endOfRepeatToIsCheckedConverter.Convert(values, typeof(bool?), "EndDate", null);
        }

        #endregion

        private void FrequencyComboBoxSelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                VisualStateManager.GoToState(this, ((RadComboBox)sender).Text, true);
        }

        private void AfterRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (Value == null) return;

            Value.EndDate = null;
        }

        private void EndOnRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (Value == null) return;

            Value.EndAfterTimes = null;
        }
    }

    public class MonthlyFrequencyDetails : UserControl
    {
        private void SetupControl(FoundOps.Core.Models.CoreEntities.Repeat schedule)
        {
            var monthlyFrequencyDetailsStackPanel = new StackPanel();

            if (schedule == null || schedule.AvailableMonthlyFrequencyDetailTypes == null)
            {
                this.Content = null;
                this.InvalidateArrange();
                return;
            }

            foreach (var monthlyFrequencyDetail in schedule.AvailableMonthlyFrequencyDetailTypes)
            {
                var monthlyFrequencyDetailRadioButton = new RadioButton { GroupName = "RepeatOn" };

                monthlyFrequencyDetailRadioButton.SetBinding(ContentControl.ContentProperty,
                                                             new Binding("StartDate")
                                                                 {
                                                                     Source = schedule,
                                                                     Converter =
                                                                         new MonthlyFrequencyDetailToStringConverter(
                                                                         monthlyFrequencyDetail)
                                                                 });

                monthlyFrequencyDetailRadioButton.SetBinding(ToggleButton.IsCheckedProperty,
                                                             new Binding("FrequencyDetailAsMonthlyFrequencyDetail")
                                                                 {
                                                                     Source = schedule,
                                                                     Converter =
                                                                         new MonthlyFrequencyDetailIsCheckedConverter(
                                                                         monthlyFrequencyDetail)
                                                                 });

                var detail = monthlyFrequencyDetail;
                monthlyFrequencyDetailRadioButton.Checked += (o, args) =>
                                                                 {
                                                                     Value.FrequencyDetailAsMonthlyFrequencyDetail = detail;
                                                                 };

                monthlyFrequencyDetailsStackPanel.Children.Add(monthlyFrequencyDetailRadioButton);
            }

            this.Content = monthlyFrequencyDetailsStackPanel;
            this.InvalidateArrange();
        }

        #region Value Dependency Property

        /// <summary>
        /// Value
        /// </summary>
        public FoundOps.Core.Models.CoreEntities.Repeat Value
        {
            get { return (FoundOps.Core.Models.CoreEntities.Repeat)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Value Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(FoundOps.Core.Models.CoreEntities.Repeat),
                typeof(MonthlyFrequencyDetails),
                new PropertyMetadata(new PropertyChangedCallback(ValueChanged)));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthlyFrequencyDetails c = d as MonthlyFrequencyDetails;
            if (c != null)
            {
                var repeatValue = (Core.Models.CoreEntities.Repeat)e.NewValue;
                if (repeatValue == null) return;

                repeatValue.PropertyChanged +=
                    (o, args) =>
                    {
                        if (args.PropertyName == "AvailableMonthlyFrequencyDetailTypes")
                            c.SetupControl(repeatValue);
                    };

                c.SetupControl(repeatValue);
            }
        }

        #endregion
    }

    public class MonthlyFrequencyDetailToStringConverter : IValueConverter
    {
        private readonly MonthlyFrequencyDetail _monthlyFrequencyDetail;

        public MonthlyFrequencyDetailToStringConverter(MonthlyFrequencyDetail monthlyFrequencyDetail)
        {
            _monthlyFrequencyDetail = monthlyFrequencyDetail;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";

            var startDate = (DateTime)value;

            switch (_monthlyFrequencyDetail)
            {
                case MonthlyFrequencyDetail.OnDayInMonth:
                    return String.Format("The {0} of the month", DateTimeTools.OrdinalSuffix(startDate.Day)); //Ex. The 1st of the month

                case MonthlyFrequencyDetail.FirstOfDayOfWeekInMonth:
                    return String.Format("The first {0} of the month", startDate.ToString("dddd")); //Ex. The first Monday of the month

                case MonthlyFrequencyDetail.SecondOfDayOfWeekInMonth:
                    return String.Format("The second {0} of the month", startDate.ToString("dddd")); //Ex. The second Monday of the month

                case MonthlyFrequencyDetail.ThirdOfDayOfWeekInMonth:
                    return String.Format("The third {0} of the month", startDate.ToString("dddd")); //Ex. The third Monday of the month

                case MonthlyFrequencyDetail.LastOfDayOfWeekInMonth:
                    return String.Format("The last {0} of the month", startDate.ToString("dddd")); //Ex. The last Monday of the month

                case MonthlyFrequencyDetail.LastOfMonth:
                    return String.Format("The last day of the month");

                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MonthlyFrequencyDetailIsCheckedConverter : IValueConverter
    {
        private readonly MonthlyFrequencyDetail _monthlyFrequencyDetail;

        public MonthlyFrequencyDetailIsCheckedConverter(MonthlyFrequencyDetail monthlyFrequencyDetail)
        {
            _monthlyFrequencyDetail = monthlyFrequencyDetail;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";

            var currentMonthlyFrequencyDetail = (MonthlyFrequencyDetail)value;

            return _monthlyFrequencyDetail == currentMonthlyFrequencyDetail;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
