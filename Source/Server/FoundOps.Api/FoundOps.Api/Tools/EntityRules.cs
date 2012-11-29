using System;
using FoundOps.Api.Models;
using RulesEngine;
using TimeZone = FoundOps.Api.Models.TimeZone;

namespace FoundOps.Api.Tools
{
    public class EntityRules
    {
        public static Engine SetupBusinessAccountRules()
        {
            var engine = new Engine();
            engine.For<BusinessAccount>()
                .Setup(ba => ba.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                .Setup(ba => ba.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                .Setup(ba => ba.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                .Setup(ba => ba.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static Engine SetupClientRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            engine.For<Client>()
                .Setup(c => c.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                .Setup(c => c.ContactInfoSet).CallValidateForEachElement()
                .Setup(c => c.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                .Setup(c => c.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                .Setup(c => c.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static Engine SetupColumnRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            engine.For<Column>()
                  .Setup(c => c.Hidden).MustBeOfType(typeof(bool)).WithMessage("Hidden Invalid")
                  .Setup(c => c.Order).MustBeOfType(typeof(int)).WithMessage("Invalid Order");

            return engine;
        }

        public static Engine SetupColumnConfigurationRules()
        {
            var engine = new Engine();

            SetupColumnRules(engine);

            engine.For<ColumnConfiguration>()
                  .Setup(cc => cc.RoleId).MustBeOfType(typeof(Guid)).WithMessage("Invalid RoleId")
                  .Setup(cc => cc.Columns).CallValidateForEachElement();

            return engine;
        }

        public static Engine SetupContactInfoRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            engine.For<ContactInfo>()
                .Setup(ci => ci.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                .Setup(ci => ci.LocationId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LocationId")
                .Setup(ci => ci.ClientId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid ClientId")
                .Setup(ci => ci.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                .Setup(ci => ci.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                .Setup(ci => ci.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static Engine SetupEmployeeRules()
        {
            var engine = new Engine();
            engine.For<Employee>()
                .Setup(e => e.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                .Setup(e => e.LinkedUserAccountId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LinkedUserAccountId")
                .Setup(e => e.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                .Setup(e => e.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                .Setup(e => e.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        #region Fields

        public static Engine SetupFieldRules(bool setupAllFieldTypes = false)
        {
            var engine = new Engine();
            engine.For<Field>()
                  .Setup(f => f.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                  .Setup(f => f.Required).MustBeOfType(typeof(bool)).WithMessage("Required Invalid")
                  .Setup(f => f.ParentFieldId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid ParentFieldId")
                  .Setup(f => f.ServiceTemplateId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid ServiceTemplateId")
                  .Setup(f => f.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                  .Setup(f => f.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                  .Setup(f => f.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            if (setupAllFieldTypes)
            {
                SetupLocationFieldRules(engine);
                SetupNumericFieldRules(engine);
                SetupOptionsFieldRules(engine);
                SetupSignatureFieldRules(engine);
                SetupTextBoxFieldRules(engine);
            }

            return engine;
        }

        public static Engine SetupLocationFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine.For<LocationField>()
                  .Setup(lf => lf.LocationId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid Locationid")
                  .Setup(lf => lf.LocationFieldTypeInt).MustBeOfType(typeof(short)).WithMessage("Invalid LocationFieldTypeInt");

            return engine;
        }

        public static Engine SetupNumericFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine.For<NumericField>()
                  .Setup(nf => nf.DecimalPlaces).MustBeOfType(typeof(int)).WithMessage("DecimalPlaces Invalid")
                  .Setup(nf => nf.Minimum).MustBeOfType(typeof(decimal)).WithMessage("Invalid Minimum")
                  .Setup(nf => nf.Maximum).MustBeOfType(typeof(decimal)).WithMessage("Invalid Maximum")
                  .Setup(nf => nf.Value).MustBeOfType(typeof(decimal?)).WithMessage("Invalid Valiue");

            return engine;
        }

        public static Engine SetupOptionsFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine = SetupOptionRules(engine);
            engine.For<OptionsField>()
                  .Setup(of => of.AllowMultipleSelection).MustBeOfType(typeof(bool)).WithMessage("AllowMultipleSelection Invalid")
                  .Setup(of => of.TypeInt).MustBeOfType(typeof(short)).WithMessage("Invalid TypeInt")
                  .Setup(of => of.Options).MustNotBeNull().CallValidateForEachElement();

            return engine;
        }

        public static Engine SetupOptionRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            engine.For<Option>()
                  .Setup(o => o.IsChecked).MustBeOfType(typeof(bool)).WithMessage("IsChecked Invalid");

            return engine;
        }

        public static Engine SetupSignatureFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine.For<SignatureField>()
                  .Setup(sf => sf.Signed).MustBeOfType(typeof(DateTime?)).WithMessage("Signed Invalid");

            return engine;
        }

        public static Engine SetupTextBoxFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine.For<TextBoxField>()
                  .Setup(tbf => tbf.IsMultiLine).MustBeOfType(typeof(bool)).WithMessage("IsMultiLine Invalid");

            return engine;
        }

        #endregion //Fields

        public static Engine SetupLocationRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            SetupContactInfoRules(engine);

            engine.For<Location>()
                  .Setup(l => l.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                  .Setup(l => l.ContactInfoSet).CallValidateForEachElement()
                  .Setup(l => l.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                  .Setup(l => l.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                  .Setup(l => l.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");


            return engine;
        }

        public static Engine SetupResourceWithLastPointRules()
        {
            var engine = new Engine();
            SetupTrackPointRules(engine);
            engine.For<ResourceWithLastPoint>()
                  .Setup(resource => resource.EmployeeId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid EmployeeId")
                  .Setup(resource => resource.VehicleId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid VehicleId");

            return engine;
        }

        public static Engine SetupRouteRules()
        {
            var engine = new Engine();
            SetupRouteDestinationRules(engine);
            engine.For<Route>()
                  .Setup(r => r.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                  .Setup(r => r.BusinessAccountId).MustBeOfType(typeof(Guid)).WithMessage("Invalid BusinessAccountId")
                  .Setup(r => r.RouteDestinations).CallValidateForEachElement()
                  .Setup(r => r.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                  .Setup(r => r.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                  .Setup(r => r.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static Engine SetupRouteDestinationRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            SetupClientRules(engine);
            SetupLocationRules(engine);
            SetupRouteTaskRules(engine);

            engine.For<RouteDestination>()
                  .Setup(rd => rd.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                  .Setup(rd => rd.OrderInRoute).MustBeOfType(typeof(int)).WithMessage("Invalid OrderInRoute")
                  .Setup(rd => rd.Client).CallValidate()
                  .Setup(rd => rd.Location).CallValidate()
                  .Setup(rd => rd.RouteTasks).CallValidateForEachElement()
                  .Setup(rd => rd.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                  .Setup(rd => rd.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                  .Setup(rd => rd.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static Engine SetupRouteTaskRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            engine.For<RouteTask>()
                .Setup(rt => rt.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                .Setup(rt => rt.Date).MustBeOfType(typeof(DateTime)).WithMessage("Invalid Date")
                .Setup(rt => rt.RecurringServiceId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid RecurringServiceId")
                .Setup(rt => rt.ServiceId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid ServiceId")
                .Setup(rt => rt.TaskStatusId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid TaskStatusId")
                .Setup(rt => rt.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                .Setup(rt => rt.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                .Setup(rt => rt.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static Engine SetupServiceRules()
        {
            var engine = SetupFieldRules(true);
            engine = SetupClientRules(engine);
            engine.For<Service>()
                .Setup(s => s.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                .Setup(s => s.ServiceDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid ServiceDate")
                .Setup(s => s.ClientId).MustBeOfType(typeof(Guid)).WithMessage("Invalid ClientId")
                .Setup(s => s.Client).CallValidate()
                .Setup(s => s.Fields).CallValidateForEachElement()
                .Setup(s => s.RecurringServiceId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid RecurringServiceId")
                .Setup(s => s.ServiceProviderId).MustBeOfType(typeof(Guid)).WithMessage("Invalid ServiceProviderId")
                .Setup(s => s.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                .Setup(s => s.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                .Setup(s => s.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static Engine SetupServiceTypeRules()
        {
            var engine = new Engine();
            engine.For<ServiceTemplate>()
                  .Setup(st => st.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id");

            return engine;
        }

        public static Engine SetupTaskStatusRules()
        {
            var engine = new Engine();
            engine.For<TaskStatus>()
                  .Setup(ts => ts.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                  .Setup(ts => ts.BusinessAccountId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid BusinessAccountId")
                  .Setup(ts => ts.DefaultTypeInt).MustBeOfType(typeof(int?)).WithMessage("DefaultTypeInt Invalid")
                  .Setup(ts => ts.RemoveFromRoute).MustBeOfType(typeof(bool)).WithMessage("RemoveFromRoute Invalid")
                  .Setup(ts => ts.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                  .Setup(ts => ts.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                  .Setup(ts => ts.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static Engine SetupTimeZoneRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            engine.For<TimeZone>()
                  .Setup(tz => tz.DisplayName).MustNotBeNull()
                  .MustBeOneOf("(UTC-10:00) Hawaii", "(UTC-09:00) Alaska", "(UTC-08:00) Pacific Time (US & Canada)",
                               "(UTC-07:00) Mountain Time (US & Canada)", "(UTC-06:00) Central Time (US & Canada)",
                               "(UTC-05:00) Eastern Time (US & Canada)")
                  .WithMessage("Invalid DisplayName")
                  .Setup(tz => tz.Id).MustNotBeNull()
                  .MustBeOneOf("Hawaiian Standard Time", "Alaskan Standard Time", "Pacific Standard Time",
                               "Mountain Standard Time", "Central Standard Time", "Eastern Standard Time")
                  .WithMessage("Invalid Id");


            return engine;
        }

        public static Engine SetupTrackPointRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            engine.For<TrackPoint>()
                  .Setup(tp => tp.Accuracy).MustBeOfType(typeof(int)).WithMessage("Invalid Accuracy")
                  .Setup(tp => tp.CollectedTimeStamp).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CollectedTimeStamp")
                  .Setup(tp => tp.Heading).MustBeOfType(typeof(int?)).WithMessage("Invalid Heading")
                  .Setup(tp => tp.Latitude).MustBeOfType(typeof(decimal?)).WithMessage("Invalid Latitude")
                  .Setup(tp => tp.Longitude).MustBeOfType(typeof(decimal?)).WithMessage("Invalid Longitude")
                  .Setup(tp => tp.Speed).MustBeOfType(typeof(decimal?)).WithMessage("Invalid Speed")
                  .Setup(tp => tp.RouteId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid RouteId");

            return engine;
        }

        public static Engine SetupUserAccountRules()
        {
            var engine = new Engine();
            SetupTimeZoneRules(engine);
            engine.For<UserAccount>()
                  .Setup(user => user.Id).MustBeOfType(typeof(Guid)).WithMessage("Invalid Id")
                  .Setup(user => user.EmployeeId).MustBeOfType(typeof(Guid)).WithMessage("Invalid EmployeeId")
                  .Setup(user => user.TimeZone).CallValidate()
                  .Setup(user => user.CreatedDate).MustBeOfType(typeof(DateTime)).WithMessage("Invalid CreatedDate")
                  .Setup(user => user.LastModified).MustBeOfType(typeof(DateTime?)).WithMessage("Invalid LastModified")
                  .Setup(user => user.LastModifyingUserId).MustBeOfType(typeof(Guid?)).WithMessage("Invalid LastModifyingUserId");

            return engine;
        }

        public static string ValidateAndReturnErrors(Engine engine, object entity)
        {
            var report = new ValidationReport(engine);
            var result = report.Validate(entity);

            if (result)
                return null;

            var errors = report.GetErrorMessages(entity);

            return string.Join(". ", errors);
        }
    }
}