namespace EmailTools.Models
{
    public class EmailMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Copy { get; set; }
        public string BlindCopy { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public List<string> ParseAddresses(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
                return new List<string>();

            return field.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim())
                        .Where(a => !string.IsNullOrEmpty(a))
                        .ToList();
        }

        public string FormatAddresses(IEnumerable<string> addresses)
        {
            var list = addresses.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
            if (!list.Any())
                return "";
            return string.Join("; ", list) + ";";
        }

        public void SetAddresses(string fieldName, IEnumerable<string> addresses)
        {
            var formatted = FormatAddresses(addresses);
            switch (fieldName)
            {
                case nameof(To):
                    To = formatted;
                    break;
                case nameof(Copy):
                    Copy = formatted;
                    break;
            }
        }

        public List<string> GetAddresses(string fieldName)
        {
            var fieldValue = (string)this.GetType().GetProperty(fieldName)?.GetValue(this);

            return fieldValue?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(a => a.Trim())
                              .Where(a => !string.IsNullOrEmpty(a))
                              .ToList() ?? new List<string>();
        }
    }
}
