using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace FoundOPS.API.Tools
{
    public class EmailPasswordTools
    {
        //Creates an 8 character random password (capital letters and numbers, excluding 0's and O's)
        public static string GeneratePassword()
        {
            var password = "";
            var random = new Random();
            const int length = 8;
            for (var i = 0; i < length; i++)
            {
                var num = random.Next(0, 3);
                if (num == 0) //if random.Next() == 0 then we generate a random character
                {
                    password += ((char)random.Next(65, 78)).ToString(CultureInfo.InvariantCulture);
                }
                else if (num == 1) //if random.Next() == 0 then we generate a random digit
                {
                    password += ((char)random.Next(80, 90)).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    password += random.Next(1, 9).ToString(CultureInfo.InvariantCulture);
                }
            }

            return password;
        }

        public static void SendEmail(string to, string subject, string body)
        {
            var ss = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("info@foundops.com", "6Neoy1KRjSVQV6sCk6ax")
            };

            var mm = new MailMessage("info@foundops.com", to, subject, body)
            {
                BodyEncoding = Encoding.UTF8,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            
            ss.Send(mm);
        }
    }
}
