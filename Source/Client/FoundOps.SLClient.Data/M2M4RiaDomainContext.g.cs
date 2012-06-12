

// M2M4RiaShared.ttinclude has been located and loaded.

#region http://riaservicescontrib.codeplex.com/
namespace RiaServicesContrib
{
	public interface IExtendedEntity
    {
        System.ServiceModel.DomainServices.Client.EntitySet EntitySet { get; }
    }
}
#endregion

#region Entities
namespace FoundOps.Core.Models.CoreEntities
{
    using System;
    using System.ServiceModel.DomainServices.Client;
	using RiaServicesContrib;
    using M2M4Ria;

    /// <summary>
    /// This class provides access to the entity's entity set and contains methods for attaching
	/// to entities to the link table in a single action.
    /// </summary>
    public partial class RoleBlock : IExtendedEntity
    {
        /// <summary>
        /// This method attaches Role and Block to the current join table entity, in such a way
        /// that both navigation properties are set before an INotifyCollectionChanged event is fired.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="role"></param>
        /// <param name="block"></param>
        [Obsolete("This property is only intended for use by the M2M4Ria solution.")]
        public static void AttachBlockToRole(RoleBlock r, Role role, Block block)
        {
            var dummy = r.Block; // this is to instantiate the EntityRef<Block>
            r._block.Entity = block;
            r._blockId = block.Id;

            r.Role = role;

            r._block.Entity = null;
            r._blockId = default(System.Guid);
            r.Block = block;
        }
        /// <summary>
        /// This method attaches Block and Role to the current join table entity, in such a way
        /// that both navigation properties are set before an INotifyCollectionChanged event is fired.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="block"></param>
        /// <param name="role"></param>
        [Obsolete("This property is only intended for use by the M2M4Ria solution.")]
        public static void AttachRoleToBlock(RoleBlock r, Block block, Role role)
        {
            var dummy = r.Role; // this is to instantiate the EntityRef<Role>
            r._role.Entity = role;
            r._roleId = role.Id;

            r.Block = block;

            r._role.Entity = null;
            r._roleId = default(System.Guid);
            r.Role = role;
        }
        /// <summary>
        /// Gets the EntitySet the link table entity is contained in.
        /// </summary>
        EntitySet IExtendedEntity.EntitySet
        {
            get
            {
                return EntitySet;
            }
        }
    }
    /// <summary>
    /// This class provides access to the entity's entity set and contains methods for attaching
	/// to entities to the link table in a single action.
    /// </summary>
    public partial class RouteVehicle : IExtendedEntity
    {
        /// <summary>
        /// This method attaches Route and Vehicle to the current join table entity, in such a way
        /// that both navigation properties are set before an INotifyCollectionChanged event is fired.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="route"></param>
        /// <param name="vehicle"></param>
        [Obsolete("This property is only intended for use by the M2M4Ria solution.")]
        public static void AttachVehicleToRoute(RouteVehicle r, Route route, Vehicle vehicle)
        {
            var dummy = r.Vehicle; // this is to instantiate the EntityRef<Vehicle>
            r._vehicle.Entity = vehicle;
            r._vehicleId = vehicle.Id;

            r.Route = route;

            r._vehicle.Entity = null;
            r._vehicleId = default(System.Guid);
            r.Vehicle = vehicle;
        }
        /// <summary>
        /// This method attaches Vehicle and Route to the current join table entity, in such a way
        /// that both navigation properties are set before an INotifyCollectionChanged event is fired.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="vehicle"></param>
        /// <param name="route"></param>
        [Obsolete("This property is only intended for use by the M2M4Ria solution.")]
        public static void AttachRouteToVehicle(RouteVehicle r, Vehicle vehicle, Route route)
        {
            var dummy = r.Route; // this is to instantiate the EntityRef<Route>
            r._route.Entity = route;
            r._routeId = route.Id;

            r.Vehicle = vehicle;

            r._route.Entity = null;
            r._routeId = default(System.Guid);
            r.Route = route;
        }
        /// <summary>
        /// Gets the EntitySet the link table entity is contained in.
        /// </summary>
        EntitySet IExtendedEntity.EntitySet
        {
            get
            {
                return EntitySet;
            }
        }
    }
    /// <summary>
    /// This class provides access to the entity's entity set and contains methods for attaching
	/// to entities to the link table in a single action.
    /// </summary>
    public partial class PartyRole : IExtendedEntity
    {
        /// <summary>
        /// This method attaches Party and Role to the current join table entity, in such a way
        /// that both navigation properties are set before an INotifyCollectionChanged event is fired.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="party"></param>
        /// <param name="role"></param>
        [Obsolete("This property is only intended for use by the M2M4Ria solution.")]
        public static void AttachRoleToParty(PartyRole r, Party party, Role role)
        {
            var dummy = r.Role; // this is to instantiate the EntityRef<Role>
            r._role.Entity = role;
            r._roleId = role.Id;

            r.Party = party;

            r._role.Entity = null;
            r._roleId = default(System.Guid);
            r.Role = role;
        }
        /// <summary>
        /// This method attaches Role and Party to the current join table entity, in such a way
        /// that both navigation properties are set before an INotifyCollectionChanged event is fired.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="role"></param>
        /// <param name="party"></param>
        [Obsolete("This property is only intended for use by the M2M4Ria solution.")]
        public static void AttachPartyToRole(PartyRole r, Role role, Party party)
        {
            var dummy = r.Party; // this is to instantiate the EntityRef<Party>
            r._party.Entity = party;
            r._partyId = party.Id;

            r.Role = role;

            r._party.Entity = null;
            r._partyId = default(System.Guid);
            r.Party = party;
        }
        /// <summary>
        /// Gets the EntitySet the link table entity is contained in.
        /// </summary>
        EntitySet IExtendedEntity.EntitySet
        {
            get
            {
                return EntitySet;
            }
        }
    }
    /// <summary>
    /// This class provides access to the entity's entity set and contains methods for attaching
	/// to entities to the link table in a single action.
    /// </summary>
    public partial class EmployeeRoute : IExtendedEntity
    {
        /// <summary>
        /// This method attaches Employee and Route to the current join table entity, in such a way
        /// that both navigation properties are set before an INotifyCollectionChanged event is fired.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="employee"></param>
        /// <param name="route"></param>
        [Obsolete("This property is only intended for use by the M2M4Ria solution.")]
        public static void AttachRouteToEmployee(EmployeeRoute r, Employee employee, Route route)
        {
            var dummy = r.Route; // this is to instantiate the EntityRef<Route>
            r._route.Entity = route;
            r._routeId = route.Id;

            r.Employee = employee;

            r._route.Entity = null;
            r._routeId = default(System.Guid);
            r.Route = route;
        }
        /// <summary>
        /// This method attaches Route and Employee to the current join table entity, in such a way
        /// that both navigation properties are set before an INotifyCollectionChanged event is fired.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="route"></param>
        /// <param name="employee"></param>
        [Obsolete("This property is only intended for use by the M2M4Ria solution.")]
        public static void AttachEmployeeToRoute(EmployeeRoute r, Route route, Employee employee)
        {
            var dummy = r.Employee; // this is to instantiate the EntityRef<Employee>
            r._employee.Entity = employee;
            r._employeeId = employee.Id;

            r.Route = route;

            r._employee.Entity = null;
            r._employeeId = default(System.Guid);
            r.Employee = employee;
        }
        /// <summary>
        /// Gets the EntitySet the link table entity is contained in.
        /// </summary>
        EntitySet IExtendedEntity.EntitySet
        {
            get
            {
                return EntitySet;
            }
        }
    }
    public partial class Block
    {
        //
        // Code relating to the managing of the 'RoleBlock' association from 'Block' to 'Role'
        //
        private IEntityCollection<Role> _Roles;

        /// <summary>
        /// Gets the collection of associated <see cref="Role"/> entities.
        /// </summary>
        public IEntityCollection<Role> Roles
        {
            get
            {
                if(_Roles == null)
                {
                    _Roles = new EntityCollection<RoleBlock, Role>(
						this.RoleBlockToRoleSet,
						r => r.Role,
						RemoveRoleBlock,
						AddRoleBlock
				    );
                }
                return _Roles;
            }
        }

        // Instruct compiler not to warn about usage of obsolete members, because using them is intended.
        #pragma warning disable 618
        private void AddRoleBlock(Role role)
		{
            var newJoinType = new RoleBlock();
            RoleBlock.AttachRoleToBlock(newJoinType, this, role);
		}
		#pragma warning restore 618

        private void RemoveRoleBlock(RoleBlock r)
        {
            if(((IExtendedEntity)r).EntitySet == null)
            {
                this.RoleBlockToRoleSet.Remove(r);
            }
            else
            {
                ((IExtendedEntity)r).EntitySet.Remove(r);
            }
        }
    }
    public partial class Role
    {
        //
        // Code relating to the managing of the 'RoleBlock' association from 'Role' to 'Block'
        //
        private IEntityCollection<Block> _Blocks;

        /// <summary>
        /// Gets the collection of associated <see cref="Block"/> entities.
        /// </summary>
        public IEntityCollection<Block> Blocks
        {
            get
            {
                if(_Blocks == null)
                {
                    _Blocks = new EntityCollection<RoleBlock, Block>(
						this.RoleBlockToBlockSet,
						r => r.Block,
						RemoveRoleBlock,
						AddRoleBlock
				    );
                }
                return _Blocks;
            }
        }

        // Instruct compiler not to warn about usage of obsolete members, because using them is intended.
        #pragma warning disable 618
        private void AddRoleBlock(Block block)
		{
            var newJoinType = new RoleBlock();
            RoleBlock.AttachBlockToRole(newJoinType, this, block);
		}
		#pragma warning restore 618

        private void RemoveRoleBlock(RoleBlock r)
        {
            if(((IExtendedEntity)r).EntitySet == null)
            {
                this.RoleBlockToBlockSet.Remove(r);
            }
            else
            {
                ((IExtendedEntity)r).EntitySet.Remove(r);
            }
        }
        //
        // Code relating to the managing of the 'PartyRole' association from 'Role' to 'Party'
        //
        private IEntityCollection<Party> _MemberParties;

        /// <summary>
        /// Gets the collection of associated <see cref="Party"/> entities.
        /// </summary>
        public IEntityCollection<Party> MemberParties
        {
            get
            {
                if(_MemberParties == null)
                {
                    _MemberParties = new EntityCollection<PartyRole, Party>(
						this.PartyRoleToPartySet,
						r => r.Party,
						RemovePartyRole,
						AddPartyRole
				    );
                }
                return _MemberParties;
            }
        }

        // Instruct compiler not to warn about usage of obsolete members, because using them is intended.
        #pragma warning disable 618
        private void AddPartyRole(Party party)
		{
            var newJoinType = new PartyRole();
            PartyRole.AttachPartyToRole(newJoinType, this, party);
		}
		#pragma warning restore 618

        private void RemovePartyRole(PartyRole r)
        {
            if(((IExtendedEntity)r).EntitySet == null)
            {
                this.PartyRoleToPartySet.Remove(r);
            }
            else
            {
                ((IExtendedEntity)r).EntitySet.Remove(r);
            }
        }
    }
    public partial class Party
    {
        //
        // Code relating to the managing of the 'PartyRole' association from 'Party' to 'Role'
        //
        private IEntityCollection<Role> _RoleMembership;

        /// <summary>
        /// Gets the collection of associated <see cref="Role"/> entities.
        /// </summary>
        public IEntityCollection<Role> RoleMembership
        {
            get
            {
                if(_RoleMembership == null)
                {
                    _RoleMembership = new EntityCollection<PartyRole, Role>(
						this.PartyRoleToRoleSet,
						r => r.Role,
						RemovePartyRole,
						AddPartyRole
				    );
                }
                return _RoleMembership;
            }
        }

        // Instruct compiler not to warn about usage of obsolete members, because using them is intended.
        #pragma warning disable 618
        private void AddPartyRole(Role role)
		{
            var newJoinType = new PartyRole();
            PartyRole.AttachRoleToParty(newJoinType, this, role);
		}
		#pragma warning restore 618

        private void RemovePartyRole(PartyRole r)
        {
            if(((IExtendedEntity)r).EntitySet == null)
            {
                this.PartyRoleToRoleSet.Remove(r);
            }
            else
            {
                ((IExtendedEntity)r).EntitySet.Remove(r);
            }
        }
    }
    public partial class Route
    {
        //
        // Code relating to the managing of the 'RouteVehicle' association from 'Route' to 'Vehicle'
        //
        private IEntityCollection<Vehicle> _Vehicles;

        /// <summary>
        /// Gets the collection of associated <see cref="Vehicle"/> entities.
        /// </summary>
        public IEntityCollection<Vehicle> Vehicles
        {
            get
            {
                if(_Vehicles == null)
                {
                    _Vehicles = new EntityCollection<RouteVehicle, Vehicle>(
						this.RouteVehicleToVehicleSet,
						r => r.Vehicle,
						RemoveRouteVehicle,
						AddRouteVehicle
				    );
                }
                return _Vehicles;
            }
        }

        // Instruct compiler not to warn about usage of obsolete members, because using them is intended.
        #pragma warning disable 618
        private void AddRouteVehicle(Vehicle vehicle)
		{
            var newJoinType = new RouteVehicle();
            RouteVehicle.AttachVehicleToRoute(newJoinType, this, vehicle);
		}
		#pragma warning restore 618

        private void RemoveRouteVehicle(RouteVehicle r)
        {
            if(((IExtendedEntity)r).EntitySet == null)
            {
                this.RouteVehicleToVehicleSet.Remove(r);
            }
            else
            {
                ((IExtendedEntity)r).EntitySet.Remove(r);
            }
        }
        //
        // Code relating to the managing of the 'EmployeeRoute' association from 'Route' to 'Employee'
        //
        private IEntityCollection<Employee> _Employees;

        /// <summary>
        /// Gets the collection of associated <see cref="Employee"/> entities.
        /// </summary>
        public IEntityCollection<Employee> Employees
        {
            get
            {
                if(_Employees == null)
                {
                    _Employees = new EntityCollection<EmployeeRoute, Employee>(
						this.EmployeeRouteToEmployeeSet,
						r => r.Employee,
						RemoveEmployeeRoute,
						AddEmployeeRoute
				    );
                }
                return _Employees;
            }
        }

        // Instruct compiler not to warn about usage of obsolete members, because using them is intended.
        #pragma warning disable 618
        private void AddEmployeeRoute(Employee employee)
		{
            var newJoinType = new EmployeeRoute();
            EmployeeRoute.AttachEmployeeToRoute(newJoinType, this, employee);
		}
		#pragma warning restore 618

        private void RemoveEmployeeRoute(EmployeeRoute r)
        {
            if(((IExtendedEntity)r).EntitySet == null)
            {
                this.EmployeeRouteToEmployeeSet.Remove(r);
            }
            else
            {
                ((IExtendedEntity)r).EntitySet.Remove(r);
            }
        }
    }
    public partial class Vehicle
    {
        //
        // Code relating to the managing of the 'RouteVehicle' association from 'Vehicle' to 'Route'
        //
        private IEntityCollection<Route> _Routes;

        /// <summary>
        /// Gets the collection of associated <see cref="Route"/> entities.
        /// </summary>
        public IEntityCollection<Route> Routes
        {
            get
            {
                if(_Routes == null)
                {
                    _Routes = new EntityCollection<RouteVehicle, Route>(
						this.RouteVehicleToRouteSet,
						r => r.Route,
						RemoveRouteVehicle,
						AddRouteVehicle
				    );
                }
                return _Routes;
            }
        }

        // Instruct compiler not to warn about usage of obsolete members, because using them is intended.
        #pragma warning disable 618
        private void AddRouteVehicle(Route route)
		{
            var newJoinType = new RouteVehicle();
            RouteVehicle.AttachRouteToVehicle(newJoinType, this, route);
		}
		#pragma warning restore 618

        private void RemoveRouteVehicle(RouteVehicle r)
        {
            if(((IExtendedEntity)r).EntitySet == null)
            {
                this.RouteVehicleToRouteSet.Remove(r);
            }
            else
            {
                ((IExtendedEntity)r).EntitySet.Remove(r);
            }
        }
    }
    public partial class Employee
    {
        //
        // Code relating to the managing of the 'EmployeeRoute' association from 'Employee' to 'Route'
        //
        private IEntityCollection<Route> _Routes;

        /// <summary>
        /// Gets the collection of associated <see cref="Route"/> entities.
        /// </summary>
        public IEntityCollection<Route> Routes
        {
            get
            {
                if(_Routes == null)
                {
                    _Routes = new EntityCollection<EmployeeRoute, Route>(
						this.EmployeeRouteToRouteSet,
						r => r.Route,
						RemoveEmployeeRoute,
						AddEmployeeRoute
				    );
                }
                return _Routes;
            }
        }

        // Instruct compiler not to warn about usage of obsolete members, because using them is intended.
        #pragma warning disable 618
        private void AddEmployeeRoute(Route route)
		{
            var newJoinType = new EmployeeRoute();
            EmployeeRoute.AttachRouteToEmployee(newJoinType, this, route);
		}
		#pragma warning restore 618

        private void RemoveEmployeeRoute(EmployeeRoute r)
        {
            if(((IExtendedEntity)r).EntitySet == null)
            {
                this.EmployeeRouteToRouteSet.Remove(r);
            }
            else
            {
                ((IExtendedEntity)r).EntitySet.Remove(r);
            }
        }
    }
}
#endregion

#region EntityCollection
namespace FoundOps.Core.Models.CoreEntities.M2M4Ria
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.ServiceModel.DomainServices.Client;

    /// <summary>
    /// Defines methods for manipulation a generic EntityCollection
    /// </summary>
    /// <typeparam name="TEntity">The type of the elements in the collection</typeparam>
    public interface IEntityCollection<TEntity> : IEnumerable<TEntity>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the current count of entities in this collection
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Event raised whenever an System.ServiceModel.DomainServices.Client.Entity
        /// is added to this collection
        /// </summary>
        event EventHandler<EntityCollectionChangedEventArgs<TEntity>> EntityAdded;
        /// <summary>
        /// Event raised whenever an System.ServiceModel.DomainServices.Client.Entity
        /// is removed from this collection
        /// </summary>
        event EventHandler<EntityCollectionChangedEventArgs<TEntity>> EntityRemoved;
        /// <summary>
        /// Add the specified entity to this collection. If the entity is unattached,
        /// it will be added to its System.ServiceModel.DomainServices.Client.EntitySet
        /// automatically.
        /// </summary>
        /// <param name="entity"> The entity to add</param>
        void Add(TEntity entity);
        /// <summary>
        /// Remove the specified entity from this collection.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        void Remove(TEntity entity);
    }

    /// <summary>
    /// M2M-specific entity collection class. It vorms a view on the underlying jointable collection.
    /// </summary>
    /// <typeparam name="JoinType"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityCollection<JoinType, TEntity> : IEntityCollection<TEntity>
        where JoinType : Entity, new()
        where TEntity : Entity
    {
        EntityCollection<JoinType> collection;
        Func<JoinType, TEntity> getEntity;
        Action<JoinType> removeAction;
		Action<TEntity> addAction;
        /// <summary>
        ///
        /// </summary>
        /// <param name="collection">The collection of associations to which this collection is connected</param>
        /// <param name="getEntity">The function used to get the entity object out of a join type entity</param>
        /// <param name="setEntity">The function used to set the entity object in a join type entity</param>
        public EntityCollection(
			EntityCollection<JoinType> collection,
			Func<JoinType, TEntity> getEntity,
            Action<JoinType> removeAction,
			Action<TEntity> addAction)
        {
            this.collection = collection;
            this.getEntity = getEntity;
            this.removeAction = removeAction;
            this.addAction = addAction;

            collection.EntityAdded += (a, b) =>
            {
                JoinType jt = b.Entity as JoinType;
                if (EntityAdded != null)
                    EntityAdded(this, new EntityCollectionChangedEventArgs<TEntity>(getEntity(jt)));
            };
            collection.EntityRemoved += (a, b) =>
            {
                JoinType jt = b.Entity as JoinType;
                if (EntityRemoved != null)
                    EntityRemoved(this, new EntityCollectionChangedEventArgs<TEntity>(getEntity(jt)));
            };
            ((INotifyCollectionChanged)collection).CollectionChanged += (sender, e) =>
            {
                if (CollectionChanged != null)
                    CollectionChanged(this, MakeNotifyCollectionChangedEventArgs(e));
            };
            ((INotifyPropertyChanged)collection).PropertyChanged += (sender, e) =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, e);
            };
        }

        /// <summary>
        /// Replaces JoinType elements in NotifyCollectionChangedEventArgs by elements of type TEntity
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private NotifyCollectionChangedEventArgs MakeNotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        TEntity entity = getEntity((JoinType)e.NewItems[0]);
                        return new NotifyCollectionChangedEventArgs(e.Action, entity, indexOfChange);
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        TEntity entity = getEntity((JoinType)e.OldItems[0]);
                        return new NotifyCollectionChangedEventArgs(e.Action, entity, indexOfChange);
                    }
                case NotifyCollectionChangedAction.Reset:
                    return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            }
            throw new Exception(String.Format("NotifyCollectionChangedAction.{0} action not supported by M2M4Ria.EntityCollection", e.Action.ToString()));
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return collection.Select(getEntity).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return collection.Count;
            }
        }

        private int IndexOf(TEntity entity)
        {
            int index = 0;
            foreach(TEntity e in this){
                if(e == entity)
                    return index;
                index++;
            }
            return -1;
        }

        // Indicates the index where a change of the collection occurred.
        private int indexOfChange;

        public void Add(TEntity entity)
        {
			addAction(entity);
        }

        /// <summary>
        /// Removes an m2m relation with the given entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(TEntity entity)
        {
            indexOfChange = IndexOf(entity);
            JoinType joinTypeToRemove = collection.SingleOrDefault(jt => getEntity(jt) == entity);
            if (joinTypeToRemove != null)
                removeAction(joinTypeToRemove);
        }

        public event EventHandler<EntityCollectionChangedEventArgs<TEntity>> EntityAdded;
        public event EventHandler<EntityCollectionChangedEventArgs<TEntity>> EntityRemoved;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
#endregion

