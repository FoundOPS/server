﻿using System;
using System.Runtime.Serialization;

namespace FoundOps.Core.Models.CoreEntities.ServiceEntites
{
    [DataContract]
    public class SimpleSignatureField : ISimpleField
    {
        public Guid Id { get; set; }

        public Guid ServiceTemplateId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// Returns the url if there is a value
        /// </summary>
        private string Url
        {
            get
            {
                if (String.IsNullOrEmpty(Value))
                    return "";

                return AppConstants.RootApiUrl + "/SignatureField/Image?Id=" + Id.ToString();
            }
        }

        /// <summary>
        /// Returns the url if there is a value
        /// </summary>
        public object ObjectValue
        {
            get { return Url; }
        }
    }
}
