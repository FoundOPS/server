﻿using System;
using System.Globalization;
using System.Windows.Data;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.Data.Converters
{
    /// <summary>
    /// Converts a Party to it's ClientOwner and vice versa
    /// </summary>
    public class OwnerPartyToClientConverter : IMultiValueConverter
    {
        public object[] _lastValues;

        #region Implementation of IMultiValueConverter

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            _lastValues = values;
            var ownerParty = values[0] as Party;
            return ownerParty == null ? null : ownerParty.ClientOwner;
        }

        /// <summary>
        /// Converts back the value to values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetTypes">The target types.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var client = value as Client;

            object party = null;
            if (client != null)
                party = client.OwnedParty;

            return new[] { party, client, _lastValues[2] };
        }

        #endregion
    }
}