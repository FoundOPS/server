using System;

namespace FoundOps.Core.Models.CoreEntities.ServiceEntites
{
    public interface ISimpleField
    {
        Guid Id { get; }

        Guid ServiceTemplateId { get; }

        string Name { get; }

        Object ObjectValue { get; }
    }
}
