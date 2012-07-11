using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Services;
using MEFedMVVM.ViewModelLocator;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows.Browser;
using System.Windows.Controls;

namespace FoundOps.SLClient.Navigator
{
    [ExportViewModel("NavigationVM")]
    public class NavigationVM : INotifyPropertyChanged
    {
        #region Properties

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

        #endregion

        /// <summary>
        /// Navigate to a view
        /// </summary>
        /// <param name="name"></param>
        [ScriptableMember]
        public UserControl NavigateToView(string name)
        {
            var infiniteAccordionSection = _infiniteAccordionSections.FirstOrDefault(t => t.Item1 == name);

            var view = GetView(infiniteAccordionSection != null ? "Infinite Accordion" : name);
            if (infiniteAccordionSection != null)
            {
                ((IInfiniteAccordionPage)view).SelectedObjectType = infiniteAccordionSection.Item2.ToString();
            }

            SelectedView = view;

            TrackChosenSection(name);

            return view;
        }

        /// <summary>
        /// Tracks chosen section analytics
        /// </summary>
        private void TrackChosenSection(string name)
        {
            Section section;
            switch (name)
            {
                case "Feedback and Support":
                    section = Section.FeedbackAndSupport;
                    break;
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
