﻿using System;
using System.Linq;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    //NOTE: Not shared because it is not editable on the client side yet
    public partial class UserAccount : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public UserAccount()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif
        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
            OnCreation();
        }
        #endregion

        /// <summary>
        /// Converts UTC to the user's time zone
        /// </summary>
        public DateTime UtcToUserTime(DateTime utc)
        {
            return utc.Add(UserTimeZoneOffset);
        }

        /// <summary>
        /// Converts the user's time to Utc
        /// </summary>
        public DateTime UserTimeToUtc(DateTime userTime)
        {
            return userTime.Subtract(UserTimeZoneOffset);
        }

        /// <summary>
        /// Now in the user's time zone
        /// </summary>
        /// <returns></returns>
        public DateTime Now()
        {
            return UtcToUserTime(DateTime.UtcNow);
        }

        public override string DisplayName
        {
            get { return String.Format("{0} {1}", this.FirstName, this.LastName); }
            set
            {
                //Try to guess the first name and last name
                if (!String.IsNullOrEmpty(value))
                {
                    var firstLastName = value.Split(' ');
                    if (firstLastName.Count() == 2)
                    {
                        this.FirstName = firstLastName.First();
                        this.LastName = firstLastName.Last();
                    }
                    else
                    {
                        this.FirstName = value;
                        this.LastName = "";
                    }
                }
                else
                {
                    FirstName = "";
                    LastName = "";
                }
            }
        }

        public Gender Gender
        {
            get
            {
                return (Gender)Convert.ToInt32(this.GenderInt);
            }
            set
            {
                this.GenderInt = Convert.ToInt16(value);
            }
        }

        partial void OnGenderIntChanged()
        {
#if SILVERLIGHT
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Gender"));      
#else
            this.OnPropertyChanged("Gender");
#endif
        }

        partial void OnFirstNameChanged()
        {
#if SILVERLIGHT
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("DisplayName"));      
#else
            this.OnPropertyChanged("DisplayName");
#endif
        }

        partial void OnLastNameChanged()
        {
#if SILVERLIGHT
            this.OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("DisplayName"));      
#else
            this.OnPropertyChanged("DisplayName");
#endif
        }
    }
}