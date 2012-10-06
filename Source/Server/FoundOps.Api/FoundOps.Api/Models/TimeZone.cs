namespace FoundOps.Api.Models
{
    public class TimeZone
    {
        public string Id { get; private set; }
        public string DisplayName { get; private set; }

        public TimeZone(string id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        public static TimeZone ConvertModel(System.TimeZoneInfo timeZoneIfo)
        {
            return new TimeZone(timeZoneIfo.Id, timeZoneIfo.DisplayName);
        }
    }
}