namespace Domain.ValueObjects
{
    public sealed record WorkSchedule
    {
        public IReadOnlyList<DayOfWeek> WorkingDays { get; }
        public TimeSpan OpeningHour { get; }
        public TimeSpan ClosingHour { get; }
        public TimeSpan? LunchStart { get; }
        public TimeSpan? LunchEnd { get; }
        public bool AllowAppointmentsDuringLunch { get; }
        public int AppointmentDurationMinutes { get; }
        public int MaxAppointmentsPerDay { get; }

        // Constructor requerido por EF Core
        private WorkSchedule() { }

        private WorkSchedule(
            IEnumerable<DayOfWeek> workingDays,
            TimeSpan openingHour,
            TimeSpan closingHour,
            TimeSpan? lunchStart,
            TimeSpan? lunchEnd,
            bool allowAppointmentsDuringLunch,
            int appointmentDurationMinutes,
            int maxAppointmentsPerDay)
        {
            var days = workingDays?.Distinct().ToList() ?? throw new ArgumentNullException(nameof(workingDays));
            if (!days.Any())
                throw new ArgumentException("At least one working day must be specified.", nameof(workingDays));

            if (openingHour >= closingHour)
                throw new ArgumentException("Opening hour must be before closing hour.");

            if (appointmentDurationMinutes <= 0)
                throw new ArgumentException("Appointment duration must be greater than zero.");

            if (maxAppointmentsPerDay <= 0)
                throw new ArgumentException("Max appointments per day must be greater than zero.");

            // Validación coherente de almuerzo
            if (lunchStart.HasValue && lunchEnd.HasValue)
            {
                if (lunchStart >= lunchEnd)
                    throw new ArgumentException("Lunch start must be before lunch end.");

                if (lunchStart < openingHour || lunchStart > closingHour)
                    throw new ArgumentException("Lunch start must be within working hours.");
                if (lunchEnd < openingHour || lunchEnd > closingHour)
                    throw new ArgumentException("Lunch end must be within working hours.");
            }

            WorkingDays = days.AsReadOnly();
            OpeningHour = openingHour;
            ClosingHour = closingHour;
            LunchStart = lunchStart;
            LunchEnd = lunchEnd;
            AllowAppointmentsDuringLunch = allowAppointmentsDuringLunch;
            AppointmentDurationMinutes = appointmentDurationMinutes;
            MaxAppointmentsPerDay = maxAppointmentsPerDay;
        }

        /// <summary>
        /// Método de factoría estático para crear un horario de trabajo
        /// </summary>
        public static WorkSchedule Create(
            IEnumerable<DayOfWeek> workingDays,
            TimeSpan openingHour,
            TimeSpan closingHour,
            TimeSpan? lunchStart,
            TimeSpan? lunchEnd,
            bool allowAppointmentsDuringLunch,
            int appointmentDurationMinutes,
            int maxAppointmentsPerDay)
        {
            // Validar nulls mínimos
            if (workingDays == null || !workingDays.Any())
                throw new ArgumentException("You must specify at least one business day.", nameof(workingDays));

            // El resto de validaciones se delegan al constructor
            return new WorkSchedule(
                workingDays,
                openingHour,
                closingHour,
                lunchStart,
                lunchEnd,
                allowAppointmentsDuringLunch,
                appointmentDurationMinutes,
                maxAppointmentsPerDay
            );
        }

        /// <summary>
        /// Cantidad de citas posibles en un día según la duración
        /// </summary>
        public int TotalPossibleAppointmentsPerDay =>
            (int)((ClosingHour - OpeningHour).TotalMinutes / AppointmentDurationMinutes);

        /// <summary>
        /// Verifica si una hora está dentro del horario laboral permitido
        /// </summary>
        public bool IsWithinWorkingHours(DayOfWeek day, TimeSpan time)
        {
            if (!WorkingDays.Contains(day)) return false;

            if (time < OpeningHour || time > ClosingHour) return false;

            if (!AllowAppointmentsDuringLunch && LunchStart.HasValue && LunchEnd.HasValue)
            {
                if (time >= LunchStart && time < LunchEnd)
                    return false;
            }

            return true;
        }
    }
}
