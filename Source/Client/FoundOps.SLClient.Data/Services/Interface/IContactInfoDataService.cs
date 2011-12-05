using System;
using System.Collections.Generic;
using System.ComponentModel;
using FoundOps.Common.Silverlight.MVVM.Models;

namespace FoundOps.Core.Context.Services.Interface
{
    public interface IContactInfoDataService :INotifyPropertyChanged
    {
        IEnumerable<string> ContactInfoTypes { get;}
        IEnumerable<string> ContactInfoLabels { get; }
    }

    public class ContactInfoDataService : ThreadableNotifiableObject, IContactInfoDataService
    {
        public IEnumerable<string> ContactInfoTypes { get; private set; }
        public IEnumerable<string> ContactInfoLabels { get; private set; }

        public ContactInfoDataService(IEnumerable<string> contactInfoTypes, IEnumerable<string> contactInfoLabels)
        {
            ContactInfoTypes = contactInfoTypes;
            ContactInfoLabels = contactInfoLabels;
        }
    }
}
