using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace FoundOps.Common.Silverlight.Interactivity.Triggers
{
    //[DefaultTrigger(typeof(UIElement), typeof(EventTrigger), "MouseLeftButtonDown")]
    //[DefaultTrigger(typeof(ButtonBase), typeof(EventTrigger), "Click")] 
    public class ConfirmBeforeCommandTrigger : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (!CanExecute || Command == null) return;

            var result = MessageBox.Show(ConfirmationString, ConfirmationString, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Command.Execute(CommandParameter);
            }
        }

        #region Command Dependency Property

        /// <summary>
        /// Command
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Command Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(ConfirmBeforeCommandTrigger),
                new PropertyMetadata(null));

        #endregion

        #region CommandParameter Dependency Property

        /// <summary>
        /// CommandParameter
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// CommandParameter Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(ConfirmBeforeCommandTrigger),
                new PropertyMetadata(null));

        #endregion

        #region ConfirmationString Dependency Property

        /// <summary>
        /// ConfirmationString
        /// </summary>
        public string ConfirmationString
        {
            get { return (string)GetValue(ConfirmationStringProperty); }
            set { SetValue(ConfirmationStringProperty, value); }
        }

        /// <summary>
        /// ConfirmationString Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ConfirmationStringProperty =
            DependencyProperty.Register(
                "ConfirmationString",
                typeof(string),
                typeof(ConfirmBeforeCommandTrigger),
                new PropertyMetadata("Are you sure?"));

        #endregion

        #region CanExecute Dependency Property

        /// <summary>
        /// CanExecute
        /// </summary>
        public bool CanExecute
        {
            get { return (bool)GetValue(CanExecuteProperty); }
            set { SetValue(CanExecuteProperty, value); }
        }

        /// <summary>
        /// CanExecute Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CanExecuteProperty =
            DependencyProperty.Register(
                "CanExecute",
                typeof(bool),
                typeof(ConfirmBeforeCommandTrigger),
                new PropertyMetadata(true));

        #endregion
    }
}
