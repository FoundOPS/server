using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FoundOps.Common.Composite;
using FoundOps.Common.Silverlight.Converters;

namespace FoundOps.Common.Silverlight.Controls
{
    public partial class GlobalDaysOfWeek : UserControl,INotifyPropertyChanged
    {
        private readonly DateTimeFormatInfo _currentDateFormat;
        private readonly List<DayOfWeekSelection> _dayOfWeekSelections;

        public GlobalDaysOfWeek()
        {
            _currentDateFormat = DateTimeTools.GetCurrentDateFormat();
            _dayOfWeekSelections = new List<DayOfWeekSelection>();

            var shortestDayNamesInOrder = _currentDateFormat.ShortestDayNamesInOrder();
            int i = 0;
            foreach (var shortestDayName in shortestDayNamesInOrder)
            {
                var selection = new DayOfWeekSelection(_currentDateFormat.DayOfWeek(i++), shortestDayName)
                                                   {
                                                       IsSelected = false
                                                   };
                _dayOfWeekSelections.Add(selection);
            }

            InitializeComponent();

            foreach (var dayOfWeekSelection in _dayOfWeekSelections)
            {
                var dayOfWeekButton = new Button() { DataContext = dayOfWeekSelection, FontSize = 14};
                dayOfWeekButton.SetBinding(ContentControl.ContentProperty, new Binding("ShortestDayName"));
                dayOfWeekButton.SetBinding(Control.ForegroundProperty,
                                           new Binding("IsSelected")
                                               {
                                                   Converter = new SelectedStatusColorConverter(),
                                                   ConverterParameter = Foreground
                                               });
               
                dayOfWeekButton.Click += ((s, e) =>
                                              {
                                                  var dows = (DayOfWeekSelection) ((Button) s).DataContext;
                                                  if (AllowEdit)
                                                  {
                                                      dows.IsSelected = !dows.IsSelected;

                                                      var selectedDaysOfWeekList = SelectedDaysOfWeek.ToList();

                                                      if (dows.IsSelected)
                                                          selectedDaysOfWeekList.Add(dows.DayOfWeek);
                                                      else
                                                          selectedDaysOfWeekList.Remove(dows.DayOfWeek);

                                                      SelectedDaysOfWeek = selectedDaysOfWeekList.ToArray();

                                                      //Update binding
                                                      BindingExpression be =
                                                          this.GetBindingExpression(SelectedDaysOfWeekProperty);
                                                      be.UpdateSource();
                                                  }
                                              });

                this.GlobalDaysOfWeekLayoutRoot.Children.Add(dayOfWeekButton);
            }
        }

        #region AllowEdit Dependency Property

        /// <summary>
        /// 
        /// </summary>
        public bool AllowEdit
        {
            get { return (bool)GetValue(AllowEditProperty); }
            set { SetValue(AllowEditProperty, value); }
        }

        /// <summary>
        /// AllowEdit Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AllowEditProperty =
            DependencyProperty.Register(
                "AllowEdit",
                typeof(bool),
                typeof(GlobalDaysOfWeek),
                new PropertyMetadata(new PropertyChangedCallback(AllowEditChanged)));

        private static void AllowEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlobalDaysOfWeek c = d as GlobalDaysOfWeek;
            if (c != null)
            {

            }
        }

        #endregion

        private void UpdateDayOfWeekSelections(IEnumerable<DayOfWeek> selectedDaysOfWeek)
        {
            foreach (var dayOfWeekSelection in _dayOfWeekSelections)
            {
                dayOfWeekSelection.IsSelected = false;
            }
            if (selectedDaysOfWeek != null)
            {
                foreach (var dayOfWeek in selectedDaysOfWeek)
                {
                    var dayOfWeekSelection = _dayOfWeekSelections.FirstOrDefault(dow => dow.DayOfWeek == dayOfWeek);
                    dayOfWeekSelection.IsSelected = true;
                }
            }

        }

        #region SelectedDaysOfWeek Dependency Property

        /// <summary>
        /// 
        /// </summary>
        public DayOfWeek[] SelectedDaysOfWeek
        {
            get { return (DayOfWeek[])GetValue(SelectedDaysOfWeekProperty); }
            set { SetValue(SelectedDaysOfWeekProperty, value); }
        }

        /// <summary>
        /// SelectedDaysOfWeek Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedDaysOfWeekProperty =
            DependencyProperty.Register(
                "SelectedDaysOfWeek",
                typeof (DayOfWeek[]),
                typeof (GlobalDaysOfWeek),
                new PropertyMetadata(new PropertyChangedCallback(SelectedDaysOfWeekChanged)));

        private static void SelectedDaysOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as GlobalDaysOfWeek;
            if (c != null)
            {
                c.UpdateDayOfWeekSelections((DayOfWeek[])e.NewValue);
            }
        }

        #endregion

        #region Foreground Dependency Property

        /// <summary>
        /// 
        /// </summary>
        public new Brush Foreground
        {
            get { return (Brush) GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Foreground Dependency Property.
        /// </summary>
        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground",
                typeof (Brush),
                typeof (GlobalDaysOfWeek),
                new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(ForegroundChanged)));

        private static void ForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlobalDaysOfWeek c = d as GlobalDaysOfWeek;
            if (c != null)
            {
                foreach (Button button in c.GlobalDaysOfWeekLayoutRoot.Children)
                {
                    button.SetBinding(Control.ForegroundProperty,
                                           new Binding("IsSelected")
                                               {
                                                   Converter = new SelectedStatusColorConverter(),
                                                   ConverterParameter = e.NewValue
                                               });
                }
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }

    public class DayOfWeekSelection:INotifyPropertyChanged
    {
        public DayOfWeekSelection(DayOfWeek dayOfWeek, string shortestDayName)
        {
            DayOfWeek = dayOfWeek;
            ShortestDayName = shortestDayName;
        }
        public DayOfWeek DayOfWeek { get; private set; }

        public string ShortestDayName { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}