using System;

namespace FoundOps.Common.Silverlight.Tools
{
    public class PasswordTools
    {
        //Creates an 8 character random password
        public static string GeneratePassword()
        {
            var password = "";
            var random = new Random();
            var length = 8;
            for (var i = 0; i < length; i++)
            {
                if (random.Next(0, 3) == 0) //if random.Next() == 0 then we generate a random character
                {
                    password += ((char)random.Next(65, 91)).ToString();
                }
                else //if random.Next() == 0 then we generate a random digit
                {
                    password += random.Next(0, 9);
                }
            }

            return password;
        }
    }
}
