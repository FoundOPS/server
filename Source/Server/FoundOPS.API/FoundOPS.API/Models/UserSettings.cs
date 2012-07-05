namespace FoundOPS.API.Models
{
    public class UserSettings
    {
        public string EmailAddress { get; set; }
        public string ImageUrl { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        /// <summary>
        /// Administrator, Mobile, Regular
        /// </summary>
        public string Role { get; set; }
    }
}