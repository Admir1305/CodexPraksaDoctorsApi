using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentApi.Models
{
    public class Patients
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "EMBG is required.")]
        public string EMBG { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number is required.")]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;
    }
}
