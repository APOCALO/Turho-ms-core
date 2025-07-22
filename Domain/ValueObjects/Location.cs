namespace Domain.ValueObjects
{
    public partial record Location
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Location Create(double latitude, double longitude)
        {
            return new Location(latitude, longitude);
        }
    }
}
