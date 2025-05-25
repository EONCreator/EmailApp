using EmailTools.Models;
using EmailTools.Services;

var domains = new[] { "tbank.ru", "alfa.com", "vtb.ru" };

var exceptions = new Dictionary<string, IEnumerable<string>>
            {
                { "tbank.ru", new[] { "i.ivanov@tbank.ru" } },
                { "alfa.com", new[] { "s.sergeev@alfa.com", "a.andreev@alfa.com" } },
                { "vtb.ru", Array.Empty<string>() }
            };

var addressesForSubstitution = new Dictionary<string, IEnumerable<string>>
            {
                { "tbank.ru", new[] { "t.tbankovich@tbank.ru", "v.veronickovna@tbank.ru" } },
                { "alfa.com", new[] { "v.vladislavovich@alfa.com" } },
                { "vtb.ru", new[] { "a.aleksandrov@vtb.ru" } }
};

var email = new EmailMessage
{
    From = "sender@example.com",
    To = "q.qweshnikov@batut.com; w.petrov@alfa.com;",
    Copy = "f.patit@buisness.com;",
    Body = "Test message"
};

var emailService = new EmailService(domains, exceptions, addressesForSubstitution);

Console.WriteLine("До редактирования:");
Console.WriteLine($"To: {email.To}");
Console.WriteLine($"Copy: {email.Copy}");

emailService.ProcessEmail(email);

Console.WriteLine("\nПосле редактирования:");
Console.WriteLine($"To: {email.To}");
Console.WriteLine($"Copy: {email.Copy}");