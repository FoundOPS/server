namespace FoundOps.Api.Models
{
    public class Option
    {
        public string Name { get; set; }

        public bool IsChecked { get; set; }

        /// <summary>
        /// Converts from the FoundOPS model to the API model
        /// </summary>
        /// <param name="modelOption">The FoundOPS model of a Option to be converted</param>
        /// <returns>An Option that has been converted to it's API model</returns>
        public static Option ConvertModel(Core.Models.CoreEntities.Option modelOption)
        {
            var option = new Option
            {
                Name = modelOption.Name,
                IsChecked = modelOption.IsChecked
            };

            return option;
        }

        /// <summary>
        /// Converts the API model back to the FoundOPS model
        /// </summary>
        public static Core.Models.CoreEntities.Option ConvertBack(Option modelOption)
        {
            var option = new Core.Models.CoreEntities.Option
            {
                Name = modelOption.Name,
                IsChecked = modelOption.IsChecked
            };

            return option;
        }
    }
}