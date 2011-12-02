using DevDefined.OAuth.Framework;

namespace FoundOps.Core.Models.QuickBooks
{
    public class QuickBooksSession
    {
        public string BaseUrl { get; set; }

        public TokenBase Token { get; set; }

        public string RealmId { get; set; }

        public string OAuthVerifier { get; set; }

    }
}