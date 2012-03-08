using System.Windows;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls;

namespace FoundOps.Core.Views.Converters
{
    public class ContactInfoDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            string type = ((ContactInfo) item).Type;

            switch (type)
            {
                case "Email Address":
                    return EmailAddressTemplate;
                case "Phone Number":
                    return PhoneNumberTemplate;
                case "Website":
                    return WebsiteTemplate;
                default:
                    return OtherTemplate;
            }
        }

        public DataTemplate EmailAddressTemplate { get; set; }
        public DataTemplate PhoneNumberTemplate { get; set; }
        public DataTemplate WebsiteTemplate { get; set; }
        public DataTemplate OtherTemplate { get; set; }
    }
}
