using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public partial record PhoneNumber
    {
        public string Value { get; init; }

        private PhoneNumber(string value) => Value = value;

        public static PhoneNumber? Create(string value, string countryCode)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(countryCode))
            {
                return null;
            }

            return ValidatePhoneNumberByCountry(value, countryCode) ? new PhoneNumber(value) : null;
        }

        public static PhoneNumber CreateWithoutCountryCode(string value)
        {
            return new PhoneNumber(value);
        }

        private static bool ValidatePhoneNumberByCountry(string value, string countryCode)
        {
            // Dictionary for patterns by country
            var phoneValidationPatterns = new Dictionary<string, (string pattern, int length)>
            {
                { "+57", (@"^3\d{9}$", 10) },  // Colombia - Accept any number starting with 3 and 10 digits long
                { "+1", (@"^\(?([2-9]{1}[0-9]{2})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$", 10) },  // United States and Canada
                { "+44", (@"^7\d{9}$", 10) },  // United Kingdom (mobile numbers only)
                { "+49", (@"^(1[5-7]\d{8,9})$", 11) },  // Germany (mobile numbers)
                { "+34", (@"^(6\d{8})$", 9) },  // Spain (mobile numbers)
                { "+33", (@"^(6|7)\d{8}$", 9) },  // France (mobile numbers)
                { "+55", (@"^(\d{2})?9\d{8}$", 11) },  // Brazil (mobile numbers)
                { "+52", (@"^1?\d{10}$", 10) },  // Mexico (mobile numbers with or without prefix 1)
                { "+91", (@"^[789]\d{9}$", 10) },  // India (mobile numbers)
                { "+61", (@"^4\d{8}$", 9) },  // Australia (mobile numbers)
                { "+81", (@"^[789]\d{9}$", 10) },  // Japan (mobile numbers)
                { "+7", (@"^9\d{9}$", 10) },  // Russia (mobile numbers)
                { "+39", (@"^\d{9,10}$", 10) },  // Italy (mobile numbers)
                { "+86", (@"^1[3-9]\d{9}$", 11) },  // China (mobile numbers)
                { "+82", (@"^1[016789]\d{7,8}$", 10) },  // South Korea (mobile numbers)
                { "+62", (@"^8\d{9,10}$", 10) },  // Indonesia (mobile numbers)
                { "+27", (@"^7[1-9]\d{7}$", 9) },  // South Africa (mobile numbers)
                { "+351", (@"^9[1236]\d{7}$", 9) },  // Portugal (mobile numbers)
                { "+31", (@"^6\d{8}$", 9) },  // Netherlands (mobile numbers)
                { "+63", (@"^9\d{9}$", 10) },  // Philippines (mobile numbers)
                // Add more countries as needed
            };

            // Validate if the country code exists in the dictionary
            if (!phoneValidationPatterns.TryGetValue(countryCode.ToUpper(), out var validationRule))
            {
                return false;
            }

            // Validate the number with the corresponding pattern
            return Regex.IsMatch(value, validationRule.pattern) && value.Length == validationRule.length;
        }
    }
}
