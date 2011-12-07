using System;
using System.Windows.Markup;
using System.Reflection;
using System.Xaml;

namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
{
    /// <summary>
    ///  Class to make a MarkupExtension XAML to retrieve the values ​​of static fields or properties.
    /// </summary>
    public class Static : IMarkupExtension<Object>
    {
        #region Properties
        private String _member;
        /// <summary>
        /// Gets or sets the static member to solve eg local:MyEnum.MyEnumValue if not otherwise MemberType MyEnumValue
        /// </summary>
        public string Member
        {
            get
            {
                return _member;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Member value");
                }

                _member = value;
            }
        }

        private Type _memberType;
        /// <summary>
        /// Gets or sets the type of the member to resolve
        /// </summary>
        public Type MemberType
        {
            get
            {
                return _memberType;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("MemberType value");
                }

                _memberType = value;
            }
        }
        #endregion

        #region MarkupExtension implementation

        /// <summary>
        /// Gets an object provides the type parameter. Retrieves a field or property
        /// </summary>
        /// <param name="serviceProvider">Service to retrieve the service used for MarkupExtension.</param>
        /// <returns>
        /// An object corresponding to the string provided in parameter
        /// </returns>
        public Object ProvideValue(IServiceProvider serviceProvider)
        {
            Object ret = null;
            Boolean typeResolveFailed = true;
            Type type = MemberType;
            String fieldMemberName = null;
            String fullFieldMemberName = null;

            if (Member != null)
            {
                if (MemberType != null)//we have the type and the member
                {
                    fieldMemberName = Member;
                    fullFieldMemberName = String.Format("{0}.{1}", type.FullName, Member);
                }
                else //was not the type, we see if the string is formatted correctly eg local:MyEnum.MyEnumeValue and try to resolve the type
                {
                    Int32 index = Member.IndexOf('.');

                    if (index >= 0)
                    {
                        string typeName = Member.Substring(0, index);

                        if (!String.IsNullOrEmpty(typeName))
                        {
                            var xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;

                            if (xamlTypeResolver != null)
                            {
                                type = xamlTypeResolver.Resolve(typeName);
                                fieldMemberName = Member.Substring(index + 1); //, Member.Length - index - 1
                                typeResolveFailed = String.IsNullOrEmpty(fieldMemberName);
                            }
                        }
                    }
                }

                if (typeResolveFailed)
                {
                    throw new InvalidOperationException("Member");
                }
                else
                {
                    if (type.IsEnum) //If an enum so we try to solve the Member
                    {
                        ret = Enum.Parse(type, fieldMemberName, true);
                    }
                    else // this is not an enum: probably a field or property
                    {
                        Boolean fail = true;

                        FieldInfo field = type.GetField(fieldMemberName, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static);

                        if (field != null)
                        {
                            fail = false;
                            ret = field.GetValue(null);
                        }
                        else// this is not a field, if you look at a property
                        {
                            //See if it is a property
                            PropertyInfo property = type.GetProperty(fieldMemberName, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static);

                            if (property != null)// this is a property
                            {
                                fail = false;
                                ret = property.GetValue(null, null);
                            }
                        }

                        if (fail)
                        {
                            throw new ArgumentException(fullFieldMemberName);
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            return ret;
        }

        #endregion
    }
}
