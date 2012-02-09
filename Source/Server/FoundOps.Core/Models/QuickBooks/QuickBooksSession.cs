using FoundOps.Common.NET;
using System.ServiceModel.DomainServices.Server;

namespace FoundOps.Core.Models.QuickBooks
{
    public class QuickBooksSession
    {
        [Exclude]
        private const string Password = "foosball27";

        [Exclude] 
        private const string InitialVector = "1Q@z32mvFeqwP~0D";

        [Exclude] 
        private const string Salt = "goose83";

        //The Id of the QuickBooks Account that is being accessed
        public string RealmId { get; set; }

        //Kind of a password given to us at the time we are accessing QuickBooks, used when we exchange the VerificationToken for the other Tokens
        public string OAuthVerifier { get; set; }

        //Temp token given to us to exchange for the QuickBooks Tokens
        //public TokenBase OAuthVerifierToken { get; set; }

        //Replacing TokenBase, problems with serialization?
        public string OAuthConsumerKey { get; set; }
        public string OAuthRealm { get; set; }
        public string OAuthSessionHandle { get; set; }
        public string OAuthToken { get; set; }

        //Used for serialization
        public string OAuthTokenSecretEncrypted { get; set; }

        [Exclude]
        public string OAuthTokenSecret
        {
            get
            {
                return AESEncryption.Decrypt(OAuthTokenSecretEncrypted, Password, Salt, "SHA1", 2, InitialVector, 256);
            }
            set
            {
                OAuthTokenSecretEncrypted = AESEncryption.Encrypt(value, Password, Salt, "SHA1", 2, InitialVector, 256);
            }
        }

        //Token used by OAuth and QuickBooks to verify login
        public string QBToken { get; set; }

        //Used along side the OAuthVerifier to check for a verified login
        public string QBTokenSecret { get; set; }
    }
}