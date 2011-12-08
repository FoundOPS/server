using System.ComponentModel;
using System.ComponentModel.Composition;
using FoundOps.Core.Context.Services;
using FoundOps.SLClient.Data.Services;
using FoundOps.Core.Models.CoreEntities;
using MEFedMVVM.ViewModelLocator;

namespace FoundOps.SLClient.UI.ViewModels
{
    /// <summary>
    /// Contains the logic for displaying Person
    /// </summary>
    [ExportViewModel("PersonVM")]
    public class PersonVM : PartyVM
    {
        #region Public Properties

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get
            {
                return _selectedPerson;
            }
            set
            {
                _selectedPerson = value;
                this.SelectedParty = value;
                RaisePropertyChanged("SelectedPerson");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="partyDataService">The party data service.</param>
        [ImportingConstructor]
        public PersonVM(DataManager dataManager, IPartyDataService partyDataService)
            : base(dataManager)
        {
            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != "SelectedPerson" || _selectedPerson == SelectedParty) return;
                if (SelectedParty is Person)
                    SelectedPerson = (Person)SelectedParty;
            };

            if (DesignerProperties.IsInDesignTool) //For Design Mode
                partyDataService.GetCurrentUserAccount(person => SelectedPerson = person);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonVM"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        /// <param name="selectedPerson">The selected person.</param>
        /// <param name="partyDataService">The party data service.</param>
        [ImportingConstructor]
        public PersonVM(DataManager dataManager, Person selectedPerson, IPartyDataService partyDataService)
            : this(dataManager, partyDataService)
        {
            SelectedPerson = selectedPerson;
        }
    }
}
