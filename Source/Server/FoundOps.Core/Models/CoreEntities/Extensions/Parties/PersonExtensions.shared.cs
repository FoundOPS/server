using System;
using System.Linq;
using FoundOps.Common.Composite.Entities;

//This is a partial class, must be in the same namespace so disable warning
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
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