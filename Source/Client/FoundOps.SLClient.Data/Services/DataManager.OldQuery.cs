﻿using FoundOps.Common.NET;
using FoundOps.Core.Models.CoreEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.DomainServices.Client;

namespace FoundOps.SLClient.Data.Services
{
    /// <summary>
    /// The old query system.
    /// </summary>
    public partial class DataManager
    {
        #region Query Helpers

        /// <summary>
        /// Loads a single entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="callback">The callback.</param>
        public void LoadSingle<TEntity>(EntityQuery<TEntity> query, Action<TEntity> callback) where TEntity : Entity
        {
            DomainContext.Load(query, loadOperation =>
            {
                if (loadOperation.HasError) return; //TODO Setup error callback
                callback(loadOperation.Entities.FirstOrDefault());
            }, null);
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Gets the current businessAccount.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="getCurrentPartyCallback">The get current businessAccount callback.</param>
        public void GetCurrentBusinessAccount(Guid roleId, Action<BusinessAccount> getCurrentBusinessAccountCallback)
        {
            LoadSingle(DomainContext.BusinessAccountForRoleQuery(roleId), getCurrentBusinessAccountCallback);
        }

        /// <summary>
        /// Gets the current user account.
        /// </summary>
        /// <param name="getCurrentUserAccountCallback">The get current user account callback.</param>
        public void GetCurrentUserAccount(Action<UserAccount> getCurrentUserAccountCallback)
        {
            LoadSingle(DomainContext.CurrentUserAccountQuery(), getCurrentUserAccountCallback);
        }

        /// <summary>
        /// Tries to geocode search text.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="geocoderResultsCallback">The geocoder results callback.</param>
        /// <param name="userState">State of the user.</param>
        public void TryGeocode(string searchText, Action<IEnumerable<GeocoderResult>, object> geocoderResultsCallback, object userState = null)
        {
            DomainContext.TryGeocode(searchText, callback => geocoderResultsCallback(callback.Value, userState), userState);
        }

        /// <summary>
        /// Tries to geocode an address.
        /// </summary>
        /// <param name="addressLineOne"></param>
        /// <param name="city"></param>
        /// <param name="state"></param>
        /// <param name="zipCode"></param>
        /// <param name="geocoderResultsCallback">The geocoder results callback.</param>
        /// <param name="userState">State of the user.</param>
        public void TryGeocodeAddress(string addressLineOne, string city, string state, string zipCode, Action<IEnumerable<GeocoderResult>, object> geocoderResultsCallback, object userState = null)
        {
            DomainContext.TryGeocodeAddress(addressLineOne, city, state, zipCode, callback => geocoderResultsCallback(callback.Value, userState), userState);
        }

        #endregion
    }
}
