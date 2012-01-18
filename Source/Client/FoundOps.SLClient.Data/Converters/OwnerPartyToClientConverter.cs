﻿using System;
using System.Globalization;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.Data.Converters
{
    /// <summary>
    /// Converts a Party to it's ClientOwner and vice versa
    /// </summary>
   public class OwnerPartyToClientConverter : IValueConverter
   {
       public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       {
           var ownerParty = value as Party;
           return ownerParty == null ? null : ownerParty.ClientOwner;
       }

       public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
       {
           var client = value as Client;
           return client == null ? null : client.OwnedParty;
       }
   }
}