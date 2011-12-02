using System;
using System.Globalization;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.Data.Converters
{
    public class OwnerPartyToClientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ownerParty = (Party)value;
            return ownerParty == null ? null : ownerParty.ClientOwner;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var client = (Client)value;
            return client == null ? null : client.OwnedParty;
        }
    }
    
}
