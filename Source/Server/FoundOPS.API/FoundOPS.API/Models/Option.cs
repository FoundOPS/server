using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoundOPS.API.Models
{
    public class Option
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsChecked { get; set; }

        public Guid OptionsFieldId { get; set; }

        public int Index { get; set; }

        public string Tooltip { get; set; }

        public static Option ConvertOptionModel(FoundOps.Core.Models.CoreEntities.Option modelOption)
        {
            var option = new Option
                {
                    Id = modelOption.Id,
                    Name = modelOption.Name,
                    IsChecked = modelOption.IsChecked,
                    OptionsFieldId = modelOption.OptionsFieldId,
                    Index = modelOption.Index,
                    Tooltip = modelOption.Tooltip
                };

            return option;
        }
    }
}