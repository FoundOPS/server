using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities
{
    public enum Gender
    {
        Male = 0,
        Female = 1
    }

    public partial class Person
    {
        public override string DisplayName
        {
            get { return String.Format("{0} {1}", this.FirstName, this.LastName); }
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