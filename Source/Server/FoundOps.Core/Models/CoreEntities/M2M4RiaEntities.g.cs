

// M2M4RiaShared.ttinclude has been located and loaded.

// Instruct compiler not to warn about usage of obsolete members, because using them is intended.
#pragma warning disable 618

namespace FoundOps.Core.Models.CoreEntities
{
    #region Entities

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel.DomainServices.Server;
    using System.Xml.Serialization;

    //
    // Association Entity Types
    //
    [Obsolete("This class is only intended for use by the RIA M2M solution")]
    public partial class RoleBlock
    {
        // 'RoleBlockToBlockSet' associationSet from 'Block.Id' to 'RoleBlock.BlockId'
        private System.Guid _BlockId;

        [DataMember]
        [Key]
        public System.Guid BlockId
        {
            get
            {
                if(Block != null)
                {
		            if(_BlockId != Block.Id && _BlockId == Guid.Empty)
                        _BlockId = Block.Id;
                }
                return _BlockId;
            }
            set
            {
                _BlockId = value;
            }
        }

        [Include]
        [XmlIgnore]
        [Association("RoleBlockToBlockSet", "BlockId", "Id", IsForeignKey = true)]
        [DataMember]
        public Block Block { get; set; }

        // 'RoleBlockToRoleSet' associationSet from 'Role.Id' to 'RoleBlock.RoleId'
        private System.Guid _RoleId;

        [DataMember]
        [Key]
        public System.Guid RoleId
        {
            get
            {
                if(Role != null)
                {
		            if(_RoleId != Role.Id && _RoleId == Guid.Empty)
                        _RoleId = Role.Id;
                }
                return _RoleId;
            }
            set
            {
                _RoleId = value;
            }
        }

        [Include]
        [XmlIgnore]
        [Association("RoleBlockToRoleSet", "RoleId", "Id", IsForeignKey = true)]
        [DataMember]
        public Role Role { get; set; }
    }
    [Obsolete("This class is only intended for use by the RIA M2M solution")]
    public partial class RouteVehicle
    {
        // 'RouteVehicleToVehicleSet' associationSet from 'Vehicle.Id' to 'RouteVehicle.VehicleId'
        private System.Guid _VehicleId;

        [DataMember]
        [Key]
        public System.Guid VehicleId
        {
            get
            {
                if(Vehicle != null)
                {
		            if(_VehicleId != Vehicle.Id && _VehicleId == Guid.Empty)
                        _VehicleId = Vehicle.Id;
                }
                return _VehicleId;
            }
            set
            {
                _VehicleId = value;
            }
        }

        [Include]
        [XmlIgnore]
        [Association("RouteVehicleToVehicleSet", "VehicleId", "Id", IsForeignKey = true)]
        [DataMember]
        public Vehicle Vehicle { get; set; }

        // 'RouteVehicleToRouteSet' associationSet from 'Route.Id' to 'RouteVehicle.RouteId'
        private System.Guid _RouteId;

        [DataMember]
        [Key]
        public System.Guid RouteId
        {
            get
            {
                if(Route != null)
                {
		            if(_RouteId != Route.Id && _RouteId == Guid.Empty)
                        _RouteId = Route.Id;
                }
                return _RouteId;
            }
            set
            {
                _RouteId = value;
            }
        }

        [Include]
        [XmlIgnore]
        [Association("RouteVehicleToRouteSet", "RouteId", "Id", IsForeignKey = true)]
        [DataMember]
        public Route Route { get; set; }
    }
    [Obsolete("This class is only intended for use by the RIA M2M solution")]
    public partial class PartyRole
    {
        // 'PartyRoleToRoleSet' associationSet from 'Role.Id' to 'PartyRole.RoleId'
        private System.Guid _RoleId;

        [DataMember]
        [Key]
        public System.Guid RoleId
        {
            get
            {
                if(Role != null)
                {
		            if(_RoleId != Role.Id && _RoleId == Guid.Empty)
                        _RoleId = Role.Id;
                }
                return _RoleId;
            }
            set
            {
                _RoleId = value;
            }
        }

        [Include]
        [XmlIgnore]
        [Association("PartyRoleToRoleSet", "RoleId", "Id", IsForeignKey = true)]
        [DataMember]
        public Role Role { get; set; }

        // 'PartyRoleToPartySet' associationSet from 'Party.Id' to 'PartyRole.PartyId'
        private System.Guid _PartyId;

        [DataMember]
        [Key]
        public System.Guid PartyId
        {
            get
            {
                if(Party != null)
                {
		            if(_PartyId != Party.Id && _PartyId == Guid.Empty)
                        _PartyId = Party.Id;
                }
                return _PartyId;
            }
            set
            {
                _PartyId = value;
            }
        }

        [Include]
        [XmlIgnore]
        [Association("PartyRoleToPartySet", "PartyId", "Id", IsForeignKey = true)]
        [DataMember]
        public Party Party { get; set; }
    }
    [Obsolete("This class is only intended for use by the RIA M2M solution")]
    public partial class EmployeeRoute
    {
        // 'EmployeeRouteToRouteSet' associationSet from 'Route.Id' to 'EmployeeRoute.RouteId'
        private System.Guid _RouteId;

        [DataMember]
        [Key]
        public System.Guid RouteId
        {
            get
            {
                if(Route != null)
                {
		            if(_RouteId != Route.Id && _RouteId == Guid.Empty)
                        _RouteId = Route.Id;
                }
                return _RouteId;
            }
            set
            {
                _RouteId = value;
            }
        }

        [Include]
        [XmlIgnore]
        [Association("EmployeeRouteToRouteSet", "RouteId", "Id", IsForeignKey = true)]
        [DataMember]
        public Route Route { get; set; }

        // 'EmployeeRouteToEmployeeSet' associationSet from 'Employee.Id' to 'EmployeeRoute.EmployeeId'
        private System.Guid _EmployeeId;

        [DataMember]
        [Key]
        public System.Guid EmployeeId
        {
            get
            {
                if(Employee != null)
                {
		            if(_EmployeeId != Employee.Id && _EmployeeId == Guid.Empty)
                        _EmployeeId = Employee.Id;
                }
                return _EmployeeId;
            }
            set
            {
                _EmployeeId = value;
            }
        }

        [Include]
        [XmlIgnore]
        [Association("EmployeeRouteToEmployeeSet", "EmployeeId", "Id", IsForeignKey = true)]
        [DataMember]
        public Employee Employee { get; set; }
    }
    //
    // Regular Entity Types
    //
    public partial class Block
    {
        [Obsolete("This property is only intended for use by the RIA M2M solution")]
        [DataMember]
        [Include]
        [Association("RoleBlockToBlockSet", "Id", "BlockId", IsForeignKey = false)]
        public IList<RoleBlock> RoleBlockToRoleSet
        {
            get
            {
                Func<Role, RoleBlock> makeJoinType = 
                    e => new RoleBlock { Block = this, Role = e };
                return Roles.Select(makeJoinType).ToList();
            }
        }
    }
    public partial class Role
    {
        [Obsolete("This property is only intended for use by the RIA M2M solution")]
        [DataMember]
        [Include]
        [Association("RoleBlockToRoleSet", "Id", "RoleId", IsForeignKey = false)]
        public IList<RoleBlock> RoleBlockToBlockSet
        {
            get
            {
                Func<Block, RoleBlock> makeJoinType = 
                    e => new RoleBlock { Role = this, Block = e };
                return Blocks.Select(makeJoinType).ToList();
            }
        }
        [Obsolete("This property is only intended for use by the RIA M2M solution")]
        [DataMember]
        [Include]
        [Association("PartyRoleToRoleSet", "Id", "RoleId", IsForeignKey = false)]
        public IList<PartyRole> PartyRoleToPartySet
        {
            get
            {
                Func<Party, PartyRole> makeJoinType = 
                    e => new PartyRole { Role = this, Party = e };
                return MemberParties.Select(makeJoinType).ToList();
            }
        }
    }
    public partial class Party
    {
        [Obsolete("This property is only intended for use by the RIA M2M solution")]
        [DataMember]
        [Include]
        [Association("PartyRoleToPartySet", "Id", "PartyId", IsForeignKey = false)]
        public IList<PartyRole> PartyRoleToRoleSet
        {
            get
            {
                Func<Role, PartyRole> makeJoinType = 
                    e => new PartyRole { Party = this, Role = e };
                return RoleMembership.Select(makeJoinType).ToList();
            }
        }
    }
    public partial class Route
    {
        [Obsolete("This property is only intended for use by the RIA M2M solution")]
        [DataMember]
        [Include]
        [Association("RouteVehicleToRouteSet", "Id", "RouteId", IsForeignKey = false)]
        public IList<RouteVehicle> RouteVehicleToVehicleSet
        {
            get
            {
                Func<Vehicle, RouteVehicle> makeJoinType = 
                    e => new RouteVehicle { Route = this, Vehicle = e };
                return Vehicles.Select(makeJoinType).ToList();
            }
        }
        [Obsolete("This property is only intended for use by the RIA M2M solution")]
        [DataMember]
        [Include]
        [Association("EmployeeRouteToRouteSet", "Id", "RouteId", IsForeignKey = false)]
        public IList<EmployeeRoute> EmployeeRouteToEmployeeSet
        {
            get
            {
                Func<Employee, EmployeeRoute> makeJoinType = 
                    e => new EmployeeRoute { Route = this, Employee = e };
                return Employees.Select(makeJoinType).ToList();
            }
        }
    }
    public partial class Vehicle
    {
        [Obsolete("This property is only intended for use by the RIA M2M solution")]
        [DataMember]
        [Include]
        [Association("RouteVehicleToVehicleSet", "Id", "VehicleId", IsForeignKey = false)]
        public IList<RouteVehicle> RouteVehicleToRouteSet
        {
            get
            {
                Func<Route, RouteVehicle> makeJoinType = 
                    e => new RouteVehicle { Vehicle = this, Route = e };
                return Routes.Select(makeJoinType).ToList();
            }
        }
    }
    public partial class Employee
    {
        [Obsolete("This property is only intended for use by the RIA M2M solution")]
        [DataMember]
        [Include]
        [Association("EmployeeRouteToEmployeeSet", "Id", "EmployeeId", IsForeignKey = false)]
        public IList<EmployeeRoute> EmployeeRouteToRouteSet
        {
            get
            {
                Func<Route, EmployeeRoute> makeJoinType = 
                    e => new EmployeeRoute { Employee = this, Route = e };
                return Routes.Select(makeJoinType).ToList();
            }
        }
    }
    #endregion
}

// Restore compiler warnings when using obsolete methods
#pragma warning restore 618



