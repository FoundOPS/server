using System;
using System.Reactive.Linq;
using ReactiveUI.Xaml;
using FoundOps.Common.Silverlight.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.MVVM.VMs;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying OptionsField
    /// </summary>
    public class OptionsFieldVM : ThreadableVM, IAddDeleteCommands
    {
        /// <summary>
        /// A command for adding an entity
        /// </summary>
        public IReactiveCommand AddCommand { get; protected set; }
        /// <summary>
        /// A command for deleting an entity
        /// </summary>
        public IReactiveCommand DeleteCommand { get; protected set; }

        private OptionsField _optionsField;
        /// <summary>
        /// Gets or sets the options field.
        /// </summary>
        /// <value>
        /// The options field.
        /// </value>
        public OptionsField OptionsField
        {
            get { return _optionsField; }
            set
            {
                _optionsField = value;
                this.RaisePropertyChanged("OptionsField");
            }
        }

        private Option _selectedOption;
        /// <summary>
        /// Gets or sets the selected option.
        /// </summary>
        /// <value>
        /// The selected option.
        /// </value>
        public Option SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                _selectedOption = value;
                this.RaisePropertyChanged("SelectedOption");
            }
        }
        protected override void RegisterCommands()
        {
            AddCommand = new ReactiveCommand();
            AddCommand.Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher().Subscribe(param =>
            {
                var newOption = new Option();
                OptionsField.Options.Add(newOption);
                SelectedOption = newOption;
            });

            DeleteCommand = new ReactiveCommand();
            DeleteCommand.Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher().Subscribe(param =>
            {
                OptionsField.Options.Remove(SelectedOption);
                SelectedOption = null;
            });
        }

        protected override void RegisterMessages()
        {
        }
    }
}
