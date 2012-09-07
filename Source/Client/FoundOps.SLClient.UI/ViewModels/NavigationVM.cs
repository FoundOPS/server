using FoundOps.Common.Silverlight.Tools.ExtensionMethods;
using Analytics = FoundOps.SLClient.Data.Services.Analytics;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using MEFedMVVM.ViewModelLocator;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Browser;
using System.Windows.Controls;

namespace FoundOps.SLClient.UI.ViewModels
{
    [ExportViewModel("NavigationVM")]
    public class NavigationVM : INotifyPropertyChanged
    {
        #region Properties

        private readonly BehaviorSubject<string> _currentSectionObservable = new BehaviorSubject<string>("Infinite Accordion");
        /// <summary>
        /// The current section Observable (backed by a BehaviorSubject)
        /// </summary>
        public IObservable<string> CurrentSectionObservable { get { return _currentSectionObservable.AsObservable(); } }

        /// <summary>
        /// The current section
        /// </summary>
        public string CurrentSection
        {
            get { return _currentSection; }
            set
            {
                _currentSection = value;
                _currentSectionObservable.OnNext(_currentSection);
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private readonly Tuple<string, Type>[] _infiniteAccordionSections = {
            new Tuple<string, Type>("Business Accounts", typeof (BusinessAccount)),
            new Tuple<string, Type>("Clients", typeof (Client)),
            new Tuple<string, Type>("Employees", typeof (Employee)),
            new Tuple<string, Type>("Locations", typeof (Location)),
            new Tuple<string, Type>("Regions", typeof (Region)),
            new Tuple<string, Type>("Services", typeof (Service)),
            new Tuple<string, Type>("Vehicles", typeof (Vehicle))
        };

        /// <summary>
        /// The current selected view.
        /// </summary>
        public UserControl SelectedView
        {
            get { return _selectedView; }
            private set
            {
                _selectedView = value;
                this.RaisePropertyChanged("SelectedView");
            }
        }

        private bool _firstBlockChosen;
        private UserControl _selectedView;
        private string _currentSection;

        #endregion

        public NavigationVM()
        {
            //Start with the InfiniteAccordion all the VMs will be attached and any messages can be properly listened to
            Rxx3.RunDelayed(TimeSpan.FromMilliseconds(100), () => NavigateToView("Infinite Accordion"));
        }

        /// <summary>
        /// Navigate to a view
        /// </summary>
        /// <param name="name"></param>
        [ScriptableMember]
        public void NavigateToView(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            //if this is an infiniteAccordionSection
            //a) load the Infinite Accordion View (instead of the view with this name)
            //b) set the IInfiniteAccordionPage.SelectedObjectType to the section's type
            var infiniteAccordionSection = _infiniteAccordionSections.FirstOrDefault(t => t.Item1 == name);

            var view = GetView(infiniteAccordionSection != null ? "Infinite Accordion" : name);
            if (infiniteAccordionSection != null)
            {
                MessageBus.Current.SendMessage(new MoveToDetailsViewMessage(infiniteAccordionSection.Item2, MoveStrategy.StartFresh));
            }

            SelectedView = view;

            CurrentSection = name;

            TrackChosenSection(name);
        }

        /// <summary>
        /// Change the current role.
        /// </summary>
        [ScriptableMember]
        public void ChangeRole(string roleId)
        {
            var roleGuid = new Guid(roleId);
            Manager.Context.RoleIdObserver.OnNext(roleGuid);
        }

        /// <summary>
        /// Tracks chosen section analytics
        /// </summary>
        private void TrackChosenSection(string name)
        {
            Section section;
            switch (name)
            {
                case "Infinite Accordion": //The initial load
                    return;
                case "Business Accounts":
                    section = Section.BusinessAccounts;
                    break;
                default:
                    section = (Section)Enum.Parse(typeof(Section), name, true);
                    break;
            }

            if (!_firstBlockChosen)
            {
                Analytics.Track(Event.FirstSectionChosen, section);
                _firstBlockChosen = true;
            }

            Analytics.Track(Event.SectionChosen, section);
        }

        private UserControl GetView(string contractName)
        {
            var definition = new ContractBasedImportDefinition(contractName, null, null, ImportCardinality.ExactlyOne, false,
                                                               false, CreationPolicy.Shared);

            return (UserControl)ViewModelRepository.Instance.Resolver.Container.GetExports(definition).First().Value;
        }
    }
}
