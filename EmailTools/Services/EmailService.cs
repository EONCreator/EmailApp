using EmailTools.Models;

namespace EmailTools.Services
{
    public class EmailService
    {
        private readonly HashSet<string> _domains;
        private readonly Dictionary<string, HashSet<string>> _exceptions;
        private readonly Dictionary<string, List<string>> _addressesForSubstitution;

        public EmailService(
            IEnumerable<string> domains,
            Dictionary<string, IEnumerable<string>> exceptions,
            Dictionary<string, IEnumerable<string>> addressesForSubstitution)
        {
            _domains = new HashSet<string>(domains);
            _exceptions = exceptions.ToDictionary(kvp => kvp.Key,
                                                    kvp => new HashSet<string>(kvp.Value));
            _addressesForSubstitution = addressesForSubstitution.ToDictionary(
                kvp => kvp.Key,
                kvp => new List<string>(kvp.Value));
        }

        public void ProcessEmail(EmailMessage email)
        {
            var fields = new[] { nameof(email.To), nameof(email.Copy) };

            foreach (var field in fields)
            {
                var currentAddresses = email.GetAddresses(field);
                var updatedAddresses = new List<string>(currentAddresses);

                for (int i = updatedAddresses.Count - 1; i >= 0; i--)
                {
                    var address = updatedAddresses[i];
                    var domain = GetDomain(address);

                    if (_domains.Contains(domain))
                    {
                        if (_exceptions.TryGetValue(domain, out var exceptionSet) && exceptionSet.Contains(address))
                        {
                            updatedAddresses.RemoveAt(i);
                            continue;
                        }

                        if (_addressesForSubstitution.TryGetValue(domain, out var substitutionAddresses))
                        {
                            foreach (var subAddr in substitutionAddresses)
                            {
                                if (!updatedAddresses.Contains(subAddr))
                                {
                                    updatedAddresses.Add(subAddr);
                                }
                            }
                        }
                    }
                }

                email.SetAddresses(field, updatedAddresses);
            }
        }



        private string GetDomain(string emailAddress)
        {
            var parts = emailAddress.Split('@');
            return parts.Length == 2 ? parts[1].ToLower() : "";
        }
    }
}
