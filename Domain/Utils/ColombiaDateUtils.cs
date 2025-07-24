namespace Domain.Utils
{
    public static class ColombiaDateUtils
    {
        private const string COLOMBIA_TIMEZONE = "SA Pacific Standard Time";

        /// <summary>
        /// Devuelve la fecha/hora convertida a hora local de Colombia.
        /// </summary>
        public static DateTime ToColombiaTime(this DateTime dateTime)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(COLOMBIA_TIMEZONE);
            return TimeZoneInfo.ConvertTime(dateTime, tz);
        }

        /// <summary>
        /// Permite inyectar una fecha/hora fija para pruebas.
        /// </summary>
        public static Func<DateTime> NowProvider { get; private set; } = () => DateTime.UtcNow;

        /// <summary>
        /// Obtiene la fecha/hora actual en la zona horaria de Colombia.
        /// </summary>
        public static DateTime NowInColombia() => NowProvider().ToColombiaTime();

        /// <summary>
        /// Establece una fecha/hora fija para pruebas.
        /// </summary>
        public static void SetFixedDateTime(DateTime fixedDateTimeUtc) =>
            NowProvider = () => fixedDateTimeUtc;

        /// <summary>
        /// Restaura el comportamiento normal de la hora actual (UTC).
        /// </summary>
        public static void ResetDateTime() =>
            NowProvider = () => DateTime.UtcNow;

        /// <summary>
        /// Obtiene el primer día del mes actual en hora local de Colombia.
        /// </summary>
        public static DateTime FirstDayOfMonth()
        {
            var now = NowInColombia();
            return new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Obtiene el último día del mes actual en hora local de Colombia.
        /// </summary>
        public static DateTime LastDayOfMonth()
        {
            return FirstDayOfMonth().AddMonths(1).AddTicks(-1);
        }

        /// <summary>
        /// Obtiene el primer día de la semana actual (lunes) en hora local de Colombia.
        /// </summary>
        public static DateTime FirstDayOfWeek()
        {
            var now = NowInColombia();
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.Date.AddDays(-diff);
        }

        /// <summary>
        /// Obtiene el último día de la semana actual (domingo) en hora local de Colombia.
        /// </summary>
        public static DateTime LastDayOfWeek()
        {
            return FirstDayOfWeek().AddDays(6);
        }
    }

}
