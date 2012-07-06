using System;
using System.Net.Http;
using FoundOPS.API.Controllers;
using FoundOPS.API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers
{
    [TestClass]
    public class UserSettingsControllerTest
    {
        [TestMethod]
        public void GetAllUserAccounts()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

            var controller = new SettingsController { Request = request };

            var response = controller.GetAllUserSettings(new Guid("EF8B0F5E-44BC-4A88-B3F0-59D8A29F5066"));
        }

        [TestMethod]
        public void InsertUserSettings()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

            var controller = new SettingsController { Request = request };

            var user = new UserSettings
                {
                    Id = new Guid("EFE06596-4FA1-40CA-B51F-EC30E8004A4D"),
                    FirstName = "Test",
                    LastName = "User",
                    EmailAddress = "bright.zach@gmail.com",
                    Role = "Mobile"
                };

            var response = controller.InsertUserSettings(user, new Guid("EF8B0F5E-44BC-4A88-B3F0-59D8A29F5066"));
        }

        [TestMethod]
        public void GetPersonalSettings()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

            var controller = new SettingsController { Request = request };

            var response = controller.GetPersonalSettings();
        }

        [TestMethod]
        public void UpdatePersonalSettings()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

            var controller = new SettingsController { Request = request };

            var user = new UserSettings
            {
                Id = new Guid("14565D04-FE81-41AC-B169-489166F07CB9"),
                FirstName = "Jonathan",
                LastName = "Perl",
                EmailAddress = "jperl@foundops.com"
            };

            var response = controller.UpdatePersonalSettings(user);
        }

        [TestMethod]
        public void UpdateUserSettings()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost");

            var controller = new SettingsController { Request = request };

            var user = new UserSettings
            {
                Id = new Guid("7FF80FD2-8AEE-46DD-B0A5-CCBA272516B0"),
                FirstName = "This Is",
                LastName = "A Test",
                EmailAddress = "Test Email"
            };

            var response = controller.UpdateUserSettings(user, new Guid("EF8B0F5E-44BC-4A88-B3F0-59D8A29F5066"));
        }
    }
}
