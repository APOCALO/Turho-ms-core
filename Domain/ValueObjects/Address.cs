namespace Domain.ValueObjects
{
    public partial record Address
    {
        public string Country { get; private set; }
        public string Department { get; private set; }
        public string City { get; private set; }
        public string StreetType { get; private set; }
        public string StreetNumber { get; private set; }
        public string CrossStreetNumber { get; private set; }
        public string PropertyNumber { get; private set; }
        public string? ZipCode { get; private set; }

        private static readonly List<string> StreetTypesCOL = new List<string> { "carrera", "calle", "diagonal", "avenida" };

        public Address(string country, string streetType, string department, string city, string streetNumber, string crossStreetNumber, string propertyNumber, string? zipCode)
        {
            this.Country = country;
            this.StreetType = streetType;
            this.Department = department;
            this.City = city;
            StreetNumber = streetNumber;
            CrossStreetNumber = crossStreetNumber;
            PropertyNumber = propertyNumber;
            ZipCode = zipCode;
        }

        public static Address? Create(string country, string department, string city, string streetType, string streetNumber, string crossStreetNumber, string propertyNumber, string? zipCode)
        {
            if (string.IsNullOrEmpty(country) 
                || string.IsNullOrEmpty(department) 
                || string.IsNullOrEmpty(city)
                || string.IsNullOrEmpty(streetType)
                || string.IsNullOrEmpty(streetNumber)
                || string.IsNullOrEmpty(crossStreetNumber)
                || string.IsNullOrEmpty(propertyNumber))
            {
                return null;
            }

            // Validate Colombia street type
            if (!ValidateColombiaStreetType(streetType))
            {
                return null;
            }

            return new Address(country, streetType, department, city, streetNumber, crossStreetNumber, propertyNumber, zipCode);
        }

        private static bool ValidateColombiaStreetType(string streetType)
        {
            return StreetTypesCOL.Contains(streetType.ToLower());
        }
    }
}
