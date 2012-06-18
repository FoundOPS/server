

// M2M4RiaShared.ttinclude has been located and loaded.


// Instruct compiler not to warn about usage of obsolete members, because using them is intended.
#pragma warning disable 618

#region Domain Service

namespace FoundOps.Server.Services.CoreDomainService
{
    using System;
    using System.Data;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
	using System.Linq;
    using FoundOps.Core.Models.CoreEntities;
	
    /// <summary>
    /// This class defines Create and Delete operations for the following many-2-many relation(s):
    ///
    ///   Role <--> Block
    ///   Route <--> Vehicle
    ///   Party <--> Role
    ///   Route <--> Employee
    ///
    /// We use stub entities to represent entities for which only the foreign key property is available in join type objects.
    ///
    /// Note: If an entity type is abstract, we use one of its derived entities to act as the concrete type for the stub entity,
    /// because we can't instantiate the abstract type. The derived entity type that we use is not important, since all derived
    /// entities types will posses the same many to many relationship from the base entity.
    /// </summary>
    public partial class CoreDomainService
    {
        [Obsolete("This method is only intended for use by the RIA M2M solution")]
        public void InsertRoleBlock(RoleBlock roleBlock)
        {
            Role role = roleBlock.Role;
            if(role == null)
            {
			   role = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Role>()
				  .SingleOrDefault(e => e.Id == roleBlock.RoleId );
			}
            if(role == null)
            {
                Role roleStubEntity = new Role { Id = roleBlock.RoleId };
                role = GetEntityByKey<Role>(ObjectContext, "Roles", roleStubEntity);
            }
            Block block = roleBlock.Block;
            if(block == null)
            {
			   block = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Block>()
				  .SingleOrDefault(e => e.Id == roleBlock.BlockId );
			}
            if(block == null)
            {
                Block blockStubEntity = new Block { Id = roleBlock.BlockId };
                block = GetEntityByKey<Block>(ObjectContext, "Blocks", blockStubEntity);
            }
            role.Blocks.Add(block);
        }
        [Obsolete("This method is only intended for use by the RIA M2M solution")]
        public void DeleteRoleBlock(RoleBlock roleBlock)
        {
            Role role = roleBlock.Role;
            if(role == null)
            {
			   role = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Role>()
				  .SingleOrDefault(e => e.Id == roleBlock.RoleId );
			}
            if(role == null)
            {
                Role roleStubEntity = new Role { Id = roleBlock.RoleId };
                role = GetEntityByKey<Role>(ObjectContext, "Roles", roleStubEntity);
            }
            Block block = roleBlock.Block;
            if(block == null)
            {
			   block = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Block>()
				  .SingleOrDefault(e => e.Id == roleBlock.BlockId );
			}
            if(block == null)
            {
                Block blockStubEntity = new Block { Id = roleBlock.BlockId };
                block = GetEntityByKey<Block>(ObjectContext, "Blocks", blockStubEntity);
            }
            if(role.Blocks.IsLoaded == false)
            {
			    // We can't attach block if role is deleted. In that case we
				// temporarily reset the entity state of role, then attach block
				// and set the entity state of role back to EntityState.Deleted.
                ObjectStateEntry stateEntry = ObjectContext.ObjectStateManager.GetObjectStateEntry(role);
                EntityState state = stateEntry.State;

                if(state == EntityState.Deleted)
                {
                    stateEntry.ChangeState(EntityState.Unchanged);
                }
                role.Blocks.Attach(block);
                if(stateEntry.State != state)
                {
                    stateEntry.ChangeState(state);
                }
            }
            role.Blocks.Remove(block);
        }
        [Obsolete("This method is only intended for use by the RIA M2M solution")]
        public void InsertRouteVehicle(RouteVehicle routeVehicle)
        {
            Route route = routeVehicle.Route;
            if(route == null)
            {
			   route = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Route>()
				  .SingleOrDefault(e => e.Id == routeVehicle.RouteId );
			}
            if(route == null)
            {
                Route routeStubEntity = new Route { Id = routeVehicle.RouteId };
                route = GetEntityByKey<Route>(ObjectContext, "Routes", routeStubEntity);
            }
            Vehicle vehicle = routeVehicle.Vehicle;
            if(vehicle == null)
            {
			   vehicle = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Vehicle>()
				  .SingleOrDefault(e => e.Id == routeVehicle.VehicleId );
			}
            if(vehicle == null)
            {
                Vehicle vehicleStubEntity = new Vehicle { Id = routeVehicle.VehicleId };
                vehicle = GetEntityByKey<Vehicle>(ObjectContext, "Vehicles", vehicleStubEntity);
            }
            route.Vehicles.Add(vehicle);
        }
        [Obsolete("This method is only intended for use by the RIA M2M solution")]
        public void DeleteRouteVehicle(RouteVehicle routeVehicle)
        {
            Route route = routeVehicle.Route;
            if(route == null)
            {
			   route = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Route>()
				  .SingleOrDefault(e => e.Id == routeVehicle.RouteId );
			}
            if(route == null)
            {
                Route routeStubEntity = new Route { Id = routeVehicle.RouteId };
                route = GetEntityByKey<Route>(ObjectContext, "Routes", routeStubEntity);
            }
            Vehicle vehicle = routeVehicle.Vehicle;
            if(vehicle == null)
            {
			   vehicle = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Vehicle>()
				  .SingleOrDefault(e => e.Id == routeVehicle.VehicleId );
			}
            if(vehicle == null)
            {
                Vehicle vehicleStubEntity = new Vehicle { Id = routeVehicle.VehicleId };
                vehicle = GetEntityByKey<Vehicle>(ObjectContext, "Vehicles", vehicleStubEntity);
            }
            if(route.Vehicles.IsLoaded == false)
            {
			    // We can't attach vehicle if route is deleted. In that case we
				// temporarily reset the entity state of route, then attach vehicle
				// and set the entity state of route back to EntityState.Deleted.
                ObjectStateEntry stateEntry = ObjectContext.ObjectStateManager.GetObjectStateEntry(route);
                EntityState state = stateEntry.State;

                if(state == EntityState.Deleted)
                {
                    stateEntry.ChangeState(EntityState.Unchanged);
                }
                route.Vehicles.Attach(vehicle);
                if(stateEntry.State != state)
                {
                    stateEntry.ChangeState(state);
                }
            }
            route.Vehicles.Remove(vehicle);
        }
        [Obsolete("This method is only intended for use by the RIA M2M solution")]
        public void InsertPartyRole(PartyRole partyRole)
        {
            Party party = partyRole.Party;
            if(party == null)
            {
			   party = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Party>()
				  .SingleOrDefault(e => e.Id == partyRole.PartyId );
			}
            if(party == null)
            {
                Party partyStubEntity = new Party { Id = partyRole.PartyId };
                party = GetEntityByKey<Party>(ObjectContext, "Parties", partyStubEntity);
            }
            Role role = partyRole.Role;
            if(role == null)
            {
			   role = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Role>()
				  .SingleOrDefault(e => e.Id == partyRole.RoleId );
			}
            if(role == null)
            {
                Role roleStubEntity = new Role { Id = partyRole.RoleId };
                role = GetEntityByKey<Role>(ObjectContext, "Roles", roleStubEntity);
            }
            party.RoleMembership.Add(role);
        }
        [Obsolete("This method is only intended for use by the RIA M2M solution")]
        public void DeletePartyRole(PartyRole partyRole)
        {
            Party party = partyRole.Party;
            if(party == null)
            {
			   party = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Party>()
				  .SingleOrDefault(e => e.Id == partyRole.PartyId );
			}
            if(party == null)
            {
                Party partyStubEntity = new Party { Id = partyRole.PartyId };
                party = GetEntityByKey<Party>(ObjectContext, "Parties", partyStubEntity);
            }
            Role role = partyRole.Role;
            if(role == null)
            {
			   role = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Role>()
				  .SingleOrDefault(e => e.Id == partyRole.RoleId );
			}
            if(role == null)
            {
                Role roleStubEntity = new Role { Id = partyRole.RoleId };
                role = GetEntityByKey<Role>(ObjectContext, "Roles", roleStubEntity);
            }
            if(party.RoleMembership.IsLoaded == false)
            {
			    // We can't attach role if party is deleted. In that case we
				// temporarily reset the entity state of party, then attach role
				// and set the entity state of party back to EntityState.Deleted.
                ObjectStateEntry stateEntry = ObjectContext.ObjectStateManager.GetObjectStateEntry(party);
                EntityState state = stateEntry.State;

                if(state == EntityState.Deleted)
                {
                    stateEntry.ChangeState(EntityState.Unchanged);
                }
                party.RoleMembership.Attach(role);
                if(stateEntry.State != state)
                {
                    stateEntry.ChangeState(state);
                }
            }
            party.RoleMembership.Remove(role);
        }
        [Obsolete("This method is only intended for use by the RIA M2M solution")]
        public void InsertRouteEmployee(RouteEmployee routeEmployee)
        {
            Route route = routeEmployee.Route;
            if(route == null)
            {
			   route = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Route>()
				  .SingleOrDefault(e => e.Id == routeEmployee.RouteId );
			}
            if(route == null)
            {
                Route routeStubEntity = new Route { Id = routeEmployee.RouteId };
                route = GetEntityByKey<Route>(ObjectContext, "Routes", routeStubEntity);
            }
            Employee employee = routeEmployee.Employee;
            if(employee == null)
            {
			   employee = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Employee>()
				  .SingleOrDefault(e => e.Id == routeEmployee.EmployeeId );
			}
            if(employee == null)
            {
                Employee employeeStubEntity = new Employee { Id = routeEmployee.EmployeeId };
                employee = GetEntityByKey<Employee>(ObjectContext, "Employees", employeeStubEntity);
            }
            route.Employees.Add(employee);
        }
        [Obsolete("This method is only intended for use by the RIA M2M solution")]
        public void DeleteRouteEmployee(RouteEmployee routeEmployee)
        {
            Route route = routeEmployee.Route;
            if(route == null)
            {
			   route = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Route>()
				  .SingleOrDefault(e => e.Id == routeEmployee.RouteId );
			}
            if(route == null)
            {
                Route routeStubEntity = new Route { Id = routeEmployee.RouteId };
                route = GetEntityByKey<Route>(ObjectContext, "Routes", routeStubEntity);
            }
            Employee employee = routeEmployee.Employee;
            if(employee == null)
            {
			   employee = ChangeSet.ChangeSetEntries.Select(cse => cse.Entity)
			      .OfType<Employee>()
				  .SingleOrDefault(e => e.Id == routeEmployee.EmployeeId );
			}
            if(employee == null)
            {
                Employee employeeStubEntity = new Employee { Id = routeEmployee.EmployeeId };
                employee = GetEntityByKey<Employee>(ObjectContext, "Employees", employeeStubEntity);
            }
            if(route.Employees.IsLoaded == false)
            {
			    // We can't attach employee if route is deleted. In that case we
				// temporarily reset the entity state of route, then attach employee
				// and set the entity state of route back to EntityState.Deleted.
                ObjectStateEntry stateEntry = ObjectContext.ObjectStateManager.GetObjectStateEntry(route);
                EntityState state = stateEntry.State;

                if(state == EntityState.Deleted)
                {
                    stateEntry.ChangeState(EntityState.Unchanged);
                }
                route.Employees.Attach(employee);
                if(stateEntry.State != state)
                {
                    stateEntry.ChangeState(state);
                }
            }
            route.Employees.Remove(employee);
        }

#region GetEntityByKey
        /// <summary>
        /// http://blogs.msdn.com/b/alexj/archive/2009/06/19/tip-26-how-to-avoid-database-queries-using-stub-entities.aspx
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="qualifiedEntitySetName"></param>
        /// <param name="stubEntity"></param>
        /// <returns></returns>
        private static T GetEntityByKey<T>(ObjectContext ctx, string qualifiedEntitySetName, T stubEntity)
        {
            ObjectStateEntry state;
            EntityKey key = ctx.CreateEntityKey(qualifiedEntitySetName, stubEntity);
            if (ctx.ObjectStateManager.TryGetObjectStateEntry(key, out state) == false)
            {
                ctx.AttachTo(qualifiedEntitySetName, stubEntity);
                return stubEntity;
            }
            else
            {
                return (T)state.Entity;
            }
        }
#endregion
    }
}

#endregion

// Restore compiler warnings when using obsolete methods
#pragma warning restore 618


