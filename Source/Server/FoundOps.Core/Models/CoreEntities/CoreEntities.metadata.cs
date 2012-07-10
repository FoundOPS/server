using FoundOps.Core.Models.CoreEntities.Validation;
using System;

// ReSharper disable CheckNamespace
//Metadata must be in the same namespace as the Entities
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;
    using System.ServiceModel.DomainServices.Server;

    [MetadataTypeAttribute(typeof(Block.BlockMetadata))]
    public partial class Block
    {
        internal sealed class BlockMetadata
        {
            private BlockMetadata()
            {
            }

            [Exclude]
            public Byte[] Icon { get; set; }

            public Uri Link { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(BusinessAccount.BusinessAccountMetadata))]
    public partial class BusinessAccount
    {
        internal sealed class BusinessAccountMetadata : Party.PartyMetadata
        {
            private BusinessAccountMetadata()
            {
            }

            [Include]
            public EntityCollection<Location> Depots { get; set; }

            [Include]
            public EntityCollection<ServiceTemplate> ServiceTemplates { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(Client.ClientMetadata))]
    public partial class Client
    {
        internal sealed class ClientMetadata
        {
            private ClientMetadata()
            {
            }

            [Include]
            public EntityCollection<ContactInfo> ContactInfoSet { get; set; }

            [Include]
            public EntityCollection<Location> Locations { get; set; }

            [Include]
            public EntityCollection<RecurringService> RecurringServices { get; set; }

            [Include]
            public EntityCollection<ServiceTemplate> ServiceTemplates { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(ClientTitle.ClientTitleMetadata))]
    public partial class ClientTitle
    {
        internal sealed class ClientTitleMetadata
        {
            private ClientTitleMetadata()
            {
            }

            [Required(ErrorMessage = "The Client is required")]
            public Guid ClientId { get; set; }

            [Include]
            public Client Client { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(Contact.ContactMetadata))]
    public partial class Contact
    {
        internal sealed class ContactMetadata
        {
            private ContactMetadata()
            {
            }

            [Include]
            public Person OwnedPerson { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(ContactInfo.ContactInfoMetadata))]
    public partial class ContactInfo
    {
        internal sealed class ContactInfoMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private ContactInfoMetadata()
            {
            }

            //Does not work for import
            //[StringLength(13, MinimumLength = 0, ErrorMessage = "The Type value cannot exceed 13 characters.")]
            public String Type { get; set; }

            //Does not work for import
            //[StringLength(13, MinimumLength = 0, ErrorMessage = "The Data value cannot exceed 13 characters.")]
            public String Data { get; set; }

            //Does not work for import
            //[StringLength(13, MinimumLength = 0, ErrorMessage = "The Label value cannot exceed 13 characters.")]
            public String Label { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(DateTimeField.DateTimeFieldMetadata))]
    public partial class DateTimeField
    {
        internal sealed class DateTimeFieldMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private DateTimeFieldMetadata()
            {
            }

            [CustomValidation(typeof(FieldValidators), "IsTimeValueValid")]
            public DateTime? Value { get; set; }

            [CustomValidation(typeof(FieldValidators), "IsTimeValueWithinEarliest")]
            public DateTime Earliest { get; set; }

            [CustomValidation(typeof(FieldValidators), "IsTimeValueWithinLatest")]
            public DateTime Latest { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(EmployeeMetadata))]
    public partial class Employee
    {
        internal sealed class EmployeeMetadata
        {
            private EmployeeMetadata()
            {
            }

            [Include]
            public Person OwnedPerson { get; set; }

            [Include]
            public UserAccount LinkedUserAccount { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(InvoiceMetadata))]
    public partial class Invoice
    {
        internal sealed class InvoiceMetadata
        {
            private InvoiceMetadata()
            {
            }

            [Include]
            public SalesTerm SalesTerm { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(Location.LocationMetadata))]
    public partial class Location
    {
        internal sealed class LocationMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private LocationMetadata()
            {
            }

            [Include]
            public EntityCollection<ContactInfo> ContactInfoSet { get; set; }

            [Include]
            public Client Client { get; set; }

            [Include]
            public Region Region { get; set; }

            [Include]
            public EntityCollection<SubLocation> SubLocations { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(LocationField.LocationFieldMetadata))]
    public partial class LocationField
    {
        internal sealed class LocationFieldMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private LocationFieldMetadata()
            {
            }

            [Include]
            public Location Value { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(OptionsField.OptionsFieldMetadata))]
    public partial class OptionsField
    {
        internal sealed class OptionsFieldMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private OptionsFieldMetadata()
            {
            }

            [Include]
            public EntityCollection<Option> Options { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(NumericField.NumericFieldMetadata))]
    public partial class NumericField
    {
        internal sealed class NumericFieldMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private NumericFieldMetadata()
            {
            }

            [CustomValidation(typeof(FieldValidators), "IsValueValid")]
            public decimal? Value { get; set; }

            [CustomValidation(typeof(FieldValidators), "IsValueWithinMinimum")]
            public decimal Minimum { get; set; }

            [CustomValidation(typeof(FieldValidators), "IsValueWithinMaximum")]
            public decimal Maximum { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies PartyMetadata as the class
    // that carries additional metadata for the Party class.
    [MetadataTypeAttribute(typeof(Party.PartyMetadata))]
    public partial class Party
    {

        // This class allows you to attach custom attributes to properties
        // of the Party class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal class PartyMetadata
        {
            protected PartyMetadata()
            {
            }

            public EntityCollection<Role> RoleMembership { get; set; }

            [Include]
            public EntityCollection<Role> OwnedRoles { get; set; }

            [Include]
            public PartyImage PartyImage { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(RecurringServiceMetadata))]
    public partial class RecurringService
    {

        internal sealed class RecurringServiceMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private RecurringServiceMetadata()
            {
            }

            [Include]
            public Client Client { get; set; }

            [Include]
            public Repeat Repeat { get; set; }

            [Include]
            public ServiceTemplate ServiceTemplate { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(Region.RegionMetadata))]
    public partial class Region
    {
        internal sealed class RegionMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private RegionMetadata()
            {
            }

            [Include]
            public EntityCollection<Location> Locations { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(RepeatMetadata))]
    public partial class Repeat
    {

        internal sealed class RepeatMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private RepeatMetadata()
            {
            }

            [CustomValidation(typeof(RepeatValidators), "IsCorrectDayCheckedForStartDate")]
            public DateTime StartDate { get; set; }

            [CustomValidation(typeof(RepeatValidators), "IsCorrectDayCheckedForFrequencyInt")]
            public int FrequencyInt { get; set; }

            [CustomValidation(typeof(RepeatValidators), "IsCorrectDayCheckedForFrequencyDetailInt")]
            public int? FrequencyDetailInt { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(RoleMetadata))]
    public partial class Role
    {
        internal sealed class RoleMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private RoleMetadata()
            {
            }

            public string Description { get; set; }

            public EntityCollection<Block> Blocks { get; set; }

            public EntityCollection<Party> MemberParties { get; set; }

            [Include]
            public Party OwnerParty { get; set; }
        }
    }


    [MetadataTypeAttribute(typeof(Route.RouteMetadata))]
    public partial class Route
    {
        internal sealed class RouteMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RouteMetadata()
            {
            }

            [Include]
            public EntityCollection<RouteDestination> RouteDestinations { get; set; }

            public BusinessAccount OwnerBusinessAccount { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(RouteDestination.RouteDestinationMetadata))]
    public partial class RouteDestination
    {
        internal sealed class RouteDestinationMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RouteDestinationMetadata()
            {
            }

            [Include]
            public Location Location { get; set; }

            public Route Route { get; set; }

            [Include]
            public EntityCollection<RouteTask> RouteTasks { get; set; }

            [Include]
            public Client Client { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(RouteTask.RouteTaskMetadata))]
    public partial class RouteTask
    {
        internal sealed class RouteTaskMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private RouteTaskMetadata()
            {
            }

            [Include]
            public Client Client { get; set; }

            [Include]
            public Location Location { get; set; }

            [Include]
            public Service Service { get; set; }
        }
    }


    [MetadataTypeAttribute(typeof(ServiceMetadata))]
    public partial class Service
    {
        internal sealed class ServiceMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private ServiceMetadata()
            {
            }

            [Required(ErrorMessage = "The Client is required")]
            public Guid ClientId { get; set; }

            [Include]
            public RecurringService RecurringServiceParent { get; set; }

            [Include]
            public ServiceTemplate ServiceTemplate { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(ServiceHolderMetadata))]
    public partial class ServiceHolder
    {
        internal sealed class ServiceHolderMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private ServiceHolderMetadata()
            {
            }

            //No need to do change tracking
            [RoundtripOriginalAttribute] 
            public Guid? ServiceId { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(ServiceTemplateMetadata))]
    public partial class ServiceTemplate
    {

        internal sealed class ServiceTemplateMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private ServiceTemplateMetadata()
            {
            }

            [Include]
            public BusinessAccount OwnerServiceProvider { get; set; }

            [Include]
            public Client OwnerClient { get; set; }

            [Include]
            public EntityCollection<Field> Fields { get; set; }

            [Include]
            public Invoice Invoice { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(SubLocationMetadata))]
    public partial class SubLocation
    {
        internal class SubLocationMetadata
        {
            protected SubLocationMetadata()
            {
            }
        }
    }

    [MetadataTypeAttribute(typeof(UserAccountMetadata))]
    public partial class UserAccount
    {
        internal sealed class UserAccountMetadata : Party.PartyMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private UserAccountMetadata()
            {
            }

            [Exclude]
            public string PasswordHash { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(VehicleMetadata))]
    public partial class Vehicle
    {

        internal sealed class VehicleMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private VehicleMetadata()
            {
            }
        }
    }

    [MetadataTypeAttribute(typeof(VehicleMaintenanceLineItemMetadata))]
    public partial class VehicleMaintenanceLineItem
    {

        internal sealed class VehicleMaintenanceLineItemMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private VehicleMaintenanceLineItemMetadata()
            {
            }

            public decimal? Cost;

            [Include]
            public VehicleMaintenanceLogEntry VehicleMaintenanceLogEntry { get; set; }
        }
    }

    [MetadataTypeAttribute(typeof(VehicleMaintenanceLogEntryMetadata))]
    public partial class VehicleMaintenanceLogEntry
    {

        internal sealed class VehicleMaintenanceLogEntryMetadata
        {
            // Metadata classes are not meant to be instantiated.
            private VehicleMaintenanceLogEntryMetadata()
            {
            }

            [Include]
            public Vehicle Vehicle { get; set; }

            [Include]
            public EntityCollection<VehicleMaintenanceLineItem> LineItems { get; set; }
        }
    }
}
