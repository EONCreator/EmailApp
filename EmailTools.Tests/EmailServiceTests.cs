using EmailTools.Models;
using EmailTools.Services;

namespace EmailProcessing.Tests
{
    [TestClass]
    public class EmailServiceTests
    {
        private readonly string[] _domains = { "tbank.ru", "alfa.com", "vtb.ru" };
        private readonly Dictionary<string, IEnumerable<string>> _exceptions = new Dictionary<string, IEnumerable<string>>
        {
            { "tbank.ru", new[] { "i.ivanov@tbank.ru" } },
            { "alfa.com", new[] { "s.sergeev@alfa.com", "a.andreev@alfa.com" } },
            { "vtb.ru", Array.Empty<string>() }
        };
        private readonly Dictionary<string, IEnumerable<string>> _addressesForSubstitution = new Dictionary<string, IEnumerable<string>>
        {
            { "tbank.ru", new[] { "t.tbankovich@tbank.ru", "v.veronickovna@tbank.ru" } },
            { "alfa.com", new[] { "v.vladislavovich@alfa.com" } },
            { "vtb.ru", new[] { "a.aleksandrov@vtb.ru" } }
        };

        private EmailService CreateService()
        {
            return new EmailService(_domains, _exceptions, _addressesForSubstitution);
        }

        [TestMethod]
        public void ProcessEmail_Should_Add_Substitution_Addresses()
        {
            var email = new EmailMessage
            {
                From = "sender@example.com",
                To = "user1@tbank.ru; user2@alfa.com;",
                Copy = ""
            };
            var service = CreateService();

            service.ProcessEmail(email);

            var toAddresses = email.GetAddresses(nameof(email.To));
            var copyAddresses = email.GetAddresses(nameof(email.Copy));

            CollectionAssert.Contains(toAddresses, "t.tbankovich@tbank.ru");
            CollectionAssert.Contains(toAddresses, "v.veronickovna@tbank.ru");
            CollectionAssert.Contains(toAddresses, "v.vladislavovich@alfa.com");
        }

        [TestMethod]
        public void ProcessEmail_Should_Remove_Exception_Address()
        {
            var email = new EmailMessage
            {
                From = "sender@example.com",
                To = "i.ivanov@tbank.ru; user2@alfa.com;",
                Copy = ""
            };
            var service = CreateService();

            service.ProcessEmail(email);

            var toAddresses = email.GetAddresses(nameof(email.To));
            CollectionAssert.DoesNotContain(toAddresses, "i.ivanov@tbank.ru");
        }

        [TestMethod]
        public void ProcessEmail_Should_Not_Duplicate_Substitution_Addresses()
        {
            var email = new EmailMessage
            {
                From = "sender@example.com",
                To = "v.veronickovna@tbank.ru; user1@alfa.com;",
                Copy = ""
            };

            var service = CreateService();

            service.ProcessEmail(email);

            var toAddresses = email.GetAddresses(nameof(email.To));
            int count = 0;
            foreach (var addr in toAddresses)
            {
                if (addr == "v.veronickovna@tbank.ru")
                    count++;
            }
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void ProcessEmail_Should_Handle_Empty_Fields()
        {
            var email = new EmailMessage
            {
                From = "sender@example.com",
                To = "",
                Copy = ""
            };

            var service = CreateService();

            service.ProcessEmail(email);

            Assert.AreEqual(0, email.GetAddresses(nameof(email.To)).Count);
            Assert.AreEqual(0, email.GetAddresses(nameof(email.Copy)).Count);
        }

        [TestMethod]
        public void ProcessEmail_Should_Work_With_Multiple_Recipients()
        {
            var email = new EmailMessage
            {
                From = "sender@example.com",
                To = "user1@tbank.ru; user2@alfa.com; user3@vtb.ru",
                Copy = "somerandom@mail.com"
            };

            var service = CreateService();

            service.ProcessEmail(email);

            var toAddrs = email.GetAddresses(nameof(email.To));
            var copyAddrs = email.GetAddresses(nameof(email.Copy));

            CollectionAssert.Contains(toAddrs, "t.tbankovich@tbank.ru");
            CollectionAssert.Contains(toAddrs, "v.veronickovna@tbank.ru");
            CollectionAssert.Contains(toAddrs, "v.vladislavovich@alfa.com");
            CollectionAssert.DoesNotContain(toAddrs, "i.ivanov@tbank.ru");

            CollectionAssert.AreEqual(new[] { "somerandom@mail.com" }, copyAddrs);
        }
    }
}