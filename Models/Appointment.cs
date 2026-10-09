namespace HospitalSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }

        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
