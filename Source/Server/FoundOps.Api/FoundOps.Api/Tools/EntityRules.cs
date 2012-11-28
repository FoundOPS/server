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
                .Setup(ba => ba.Id).MustBeOfType(typeof(Guid))
                .Setup(ba => ba.CreatedDate).MustBeOfType(typeof(DateTime))
                .Setup(ba => ba.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                .Setup(ba => ba.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupClientRules(Engine engine = null)
        {
            if(engine == null)
                engine = new Engine();

            engine.For<Client>()
                .Setup(c => c.Id).MustBeOfType(typeof(Guid))
                .Setup(c => c.ContactInfoSet).CallValidateForEachElement()
                .Setup(c => c.CreatedDate).MustBeOfType(typeof(DateTime))
                .Setup(c => c.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                .Setup(c => c.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupColumnRules(Engine engine = null)
        {
            if(engine == null)
                engine = new Engine();

            engine.For<Column>()
                  .Setup(c => c.Hidden).MustBeOfType(typeof (bool))
                  .Setup(c => c.Order).MustBeOfType(typeof (int));

            return engine;
        }

        public static Engine SetupColumnConfigurationRules()
        {
            var engine = new Engine();

            SetupColumnRules(engine);

            engine.For<ColumnConfiguration>()
                  .Setup(cc => cc.RoleId).MustBeOfType(typeof (Guid))
                  .Setup(cc => cc.Columns).CallValidateForEachElement();

            return engine;
        }

        public static Engine SetupContactInfoRules(Engine engine = null)
        {
            if(engine == null)
                engine = new Engine();

            engine.For<ContactInfo>()
                .Setup(ci => ci.Id).MustBeOfType(typeof(Guid))
                .Setup(ci => ci.LocationId).MustBeOfType(typeof(Guid?))
                .Setup(ci => ci.ClientId).MustBeOfType(typeof(Guid?))
                .Setup(ci => ci.CreatedDate).MustBeOfType(typeof(DateTime))
                .Setup(ci => ci.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                .Setup(ci => ci.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupEmployeeRules()
        {
            var engine = new Engine();
            engine.For<Employee>()
                .Setup(e => e.Id).MustBeOfType(typeof(Guid))
                .Setup(e => e.LinkedUserAccountId).MustBeOfType(typeof(Guid?))
                .Setup(e => e.CreatedDate).MustBeOfType(typeof(DateTime))
                .Setup(e => e.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                .Setup(e => e.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        #region Fields

        public static Engine SetupFieldRules(bool setupAllFieldTypes = false)
        {
            var engine = new Engine();
            engine.For<Field>()
                  .Setup(f => f.Id).MustBeOfType(typeof(Guid))
                  .Setup(f => f.Required).MustBeOfType(typeof(bool))
                  .Setup(f => f.ParentFieldId).MustBeOfType(typeof(Guid?))
                  .Setup(f => f.ServiceTemplateId).MustBeOfType(typeof(Guid?))
                  .Setup(f => f.CreatedDate).MustBeOfType(typeof(DateTime))
                  .Setup(f => f.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                  .Setup(f => f.LastModifyingUserId).MustBeOfType(typeof(Guid?));

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
                  .Setup(lf => lf.LocationId).MustBeOfType(typeof(Guid?))
                  .Setup(lf => lf.LocationFieldTypeInt).MustBeOfType(typeof(short));

            return engine;
        }

        public static Engine SetupNumericFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine.For<NumericField>()
                  .Setup(nf => nf.DecimalPlaces).MustBeOfType(typeof(int))
                  .Setup(nf => nf.Minimum).MustBeOfType(typeof(decimal))
                  .Setup(nf => nf.Maximum).MustBeOfType(typeof(decimal))
                  .Setup(nf => nf.Value).MustBeOfType(typeof(decimal?));

            return engine;
        }

        public static Engine SetupOptionsFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine = SetupOptionRules(engine);
            engine.For<OptionsField>()
                  .Setup(of => of.AllowMultipleSelection).MustBeOfType(typeof(bool))
                  .Setup(of => of.TypeInt).MustBeOfType(typeof(short))
                  .Setup(of => of.Options).MustNotBeNull().CallValidateForEachElement();

            return engine;
        }

        public static Engine SetupOptionRules(Engine engine = null)
        {
            if (engine == null)
                engine = new Engine();

            engine.For<Option>()
                  .Setup(o => o.IsChecked).MustBeOfType(typeof(bool));

            return engine;
        }

        public static Engine SetupSignatureFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine.For<SignatureField>()
                  .Setup(sf => sf.Signed).MustBeOfType(typeof(DateTime?));

            return engine;
        }

        public static Engine SetupTextBoxFieldRules(Engine engine = null)
        {
            if (engine == null)
                engine = SetupFieldRules();

            engine.For<TextBoxField>()
                  .Setup(tbf => tbf.IsMultiLine).MustBeOfType(typeof(bool));

            return engine;
        }

        #endregion //Fields

        public static Engine SetupLocationRules(Engine engine = null)
        {
            if(engine == null)
                engine = new Engine();

            SetupContactInfoRules(engine);

            engine.For<Location>()
                  .Setup(l => l.Id).MustBeOfType(typeof (Guid))
                  .Setup(l => l.ContactInfoSet).CallValidateForEachElement()
                  .Setup(l => l.CreatedDate).MustBeOfType(typeof(DateTime))
                  .Setup(l => l.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                  .Setup(l => l.LastModifyingUserId).MustBeOfType(typeof(Guid?));


            return engine;
        }

        public static Engine SetupResourceWithLastPointRules()
        {
            var engine = new Engine();
            SetupTrackPointRules(engine);
            engine.For<ResourceWithLastPoint>()
                  .Setup(resource => resource.EmployeeId).MustBeOfType(typeof (Guid?))
                  .Setup(resource => resource.VehicleId).MustBeOfType(typeof (Guid?));

            return engine;
        }

        public static Engine SetupRouteRules()
        {
            var engine = new Engine();
            SetupRouteDestinationRules(engine);
            engine.For<Route>()
                  .Setup(r => r.Id).MustBeOfType(typeof (Guid))
                  .Setup(r => r.BusinessAccountId).MustBeOfType(typeof (Guid))
                  .Setup(r => r.RouteDestinations).CallValidateForEachElement()
                  .Setup(r => r.CreatedDate).MustBeOfType(typeof(DateTime))
                  .Setup(r => r.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                  .Setup(r => r.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupRouteDestinationRules(Engine engine = null)
        {
            if(engine == null)
                engine = new Engine();
            
            SetupClientRules(engine);
            SetupLocationRules(engine);
            SetupRouteTaskRules(engine);

            engine.For<RouteDestination>()
                  .Setup(rd => rd.Id).MustBeOfType(typeof (Guid))
                  .Setup(rd => rd.OrderInRoute).MustBeOfType(typeof (int))
                  .Setup(rd => rd.Client).CallValidate()
                  .Setup(rd => rd.Location).CallValidate()
                  .Setup(rd => rd.RouteTasks).CallValidateForEachElement()
                  .Setup(rd => rd.CreatedDate).MustBeOfType(typeof(DateTime))
                  .Setup(rd => rd.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                  .Setup(rd => rd.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupRouteTaskRules(Engine engine = null)
        {
            if(engine == null)
                engine = new Engine();
            
            engine.For<RouteTask>()
                .Setup(rt => rt.Id).MustBeOfType(typeof(Guid))
                .Setup(rt => rt.Date).MustBeOfType(typeof(DateTime))
                .Setup(rt => rt.RecurringServiceId).MustBeOfType(typeof(Guid?))
                .Setup(rt => rt.ServiceId).MustBeOfType(typeof(Guid?))
                .Setup(rt => rt.TaskStatusId).MustBeOfType(typeof(Guid?))
                .Setup(rt => rt.CreatedDate).MustBeOfType(typeof(DateTime))
                .Setup(rt => rt.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                .Setup(rt => rt.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupServiceRules()
        {
            var engine = SetupFieldRules(true);
            engine = SetupClientRules(engine);
            engine.For<Service>()
                .Setup(s => s.Id).MustBeOfType(typeof(Guid))
                .Setup(s => s.ServiceDate).MustBeOfType(typeof(DateTime))
                .Setup(s => s.ClientId).MustBeOfType(typeof(Guid))
                .Setup(s => s.Client).CallValidate()
                .Setup(s => s.Fields).CallValidateForEachElement()
                .Setup(s => s.RecurringServiceId).MustBeOfType(typeof(Guid?))
                .Setup(s => s.ServiceProviderId).MustBeOfType(typeof(Guid))
                .Setup(s => s.CreatedDate).MustBeOfType(typeof(DateTime))
                .Setup(s => s.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                .Setup(s => s.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupServiceTypeRules()
        {
            var engine = new Engine();
            engine.For<ServiceTemplate>()
                  .Setup(st => st.Id).MustBeOfType(typeof(Guid));

            return engine;
        }

        public static Engine SetupTaskStatusRules()
        {
            var engine = new Engine();
            engine.For<TaskStatus>()
                  .Setup(ts => ts.Id).MustBeOfType(typeof(Guid))
                  .Setup(ts => ts.BusinessAccountId).MustBeOfType(typeof(Guid?))
                  .Setup(ts => ts.DefaultTypeInt).MustBeOfType(typeof(int?))
                  .Setup(ts => ts.RemoveFromRoute).MustBeOfType(typeof(bool))
                  .Setup(ts => ts.CreatedDate).MustBeOfType(typeof(DateTime))
                  .Setup(ts => ts.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                  .Setup(ts => ts.LastModifyingUserId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupTimeZoneRules(Engine engine = null)
        {
            if(engine == null)
                engine = new Engine();

            engine.For<TimeZone>()
                .Setup(tz => tz.DisplayName).MustNotBeNull()
                            .MustBeOneOf("(UTC-10:00) Hawaii", "(UTC-09:00) Alaska", "(UTC-08:00) Pacific Time (US & Canada)", "(UTC-07:00) Mountain Time (US & Canada)", "(UTC-06:00) Central Time (US & Canada)", "(UTC-05:00) Eastern Time (US & Canada)")
                .Setup(tz => tz.Id).MustNotBeNull()
                            .MustBeOneOf("Hawaiian Standard Time", "Alaskan Standard Time", "Pacific Standard Time", "Mountain Standard Time", "Central Standard Time", "Eastern Standard Time");

            return engine;
        }

        public static Engine SetupTrackPointRules(Engine engine = null)
        {
            if(engine == null)
                engine = new Engine();
            
            engine.For<TrackPoint>()
                  .Setup(tp => tp.Accuracy).MustBeOfType(typeof(int))
                  .Setup(tp => tp.CollectedTimeStamp).MustBeOfType(typeof(DateTime))
                  .Setup(tp => tp.Heading).MustBeOfType(typeof(int?))
                  .Setup(tp => tp.Latitude).MustBeOfType(typeof(decimal?))
                  .Setup(tp => tp.Longitude).MustBeOfType(typeof(decimal?))
                  .Setup(tp => tp.Speed).MustBeOfType(typeof(decimal?))
                  .Setup(tp => tp.RouteId).MustBeOfType(typeof(Guid?));

            return engine;
        }

        public static Engine SetupUserAccountRules()
        {
            var engine = new Engine();
            SetupTimeZoneRules(engine);
            engine.For<UserAccount>()
                  .Setup(user => user.Id).MustBeOfType(typeof(Guid))
                  .Setup(user => user.EmployeeId).MustBeOfType(typeof(Guid))
                  .Setup(user => user.TimeZone).CallValidate()
                  .Setup(user => user.CreatedDate).MustBeOfType(typeof(DateTime))
                  .Setup(user => user.LastModifiedDate).MustBeOfType(typeof(DateTime?))
                  .Setup(user => user.LastModifyingUserId).MustBeOfType(typeof(Guid));

            return engine;
        }
    }
}