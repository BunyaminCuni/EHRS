using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHRS.Model
{
    public class AppointmentDbContext : DbContext
    {
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Pet> Pets { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Hospital - City ilişkisi
            modelBuilder.Entity<Hospital>()
                .HasOne(h => h.City)
                .WithMany(c => c.Hospitals)
                .HasForeignKey(h => h.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor - Hospital ilişkisi
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Hospital)
                .WithMany(h => h.Doctors)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointment - Doctor ilişkisi
            modelBuilder.Entity<Appointment>()
                .HasOne<Doctor>()
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Appointment - Pet ilişkisi
            modelBuilder.Entity<Appointment>()
                .HasOne<Pet>()
                .WithMany()
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            //  Pet - User ilişkisi
            modelBuilder.Entity<Pet>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - City ilişkisi
            modelBuilder.Entity<User>()
                .HasOne(u => u.City)
                .WithMany()
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // User tablosunda Phone unique olmalı 
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique();

        }
    }

    [Table("cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("cityId")]
        public int CityId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("cityName")]
        public string CityName { get; set; } = string.Empty;

        // İlişki: Bir şehirde birden fazla hastane olabilir
        public ICollection<Hospital> Hospitals { get; set; } = new List<Hospital>();
    }

    [Table("hospitals")]
    public class Hospital
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("hospitalId")]
        public int HospitalId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("hospitalName")]
        public string HospitalName { get; set; } = string.Empty;

        [Required]
        [Column("cityId")]
        public int CityId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("address")]
        public string Address { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("districtName")]
        public string DistrictName { get; set; } = string.Empty;

        // Nöbet bilgisi
        [Column("isOnDuty")]
        public bool IsOnDuty { get; set; } = false;

        // Foreign Key ilişkisi
        [ForeignKey("CityId")]
        public City City { get; set; }

        // İlişki: Bir hastanede birden fazla doktor olabilir
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }

    [Table("doctors")]
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("doctorId")]
        public int DoctorId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("doctorName")]
        public string DoctorName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Column("hospitalId")]
        public int HospitalId { get; set; }

        // Foreign Key ilişkisi
        [ForeignKey("HospitalId")]
        public Hospital Hospital { get; set; }
    }

    // DEĞİŞTİ: Patient → User
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("userId")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("userName")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Column("cityId")]
        public int CityId { get; set; }

        [MaxLength(1000)]
        [Column("address")]
        public string Address { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey("CityId")]
        public City? City { get; set; }

        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        // YENİ EKLENEN
        [Required]
        [MaxLength(255)]
        [Column("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    [Table("pets")]
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("petId")]
        public int PetId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("petName")]
        public string PetName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("petType")]
        public string PetType { get; set; } = string.Empty;

        [Column("age")]
        public int? Age { get; set; }

        [MaxLength(10)]
        [Column("gender")]
        public string? Gender { get; set; }

        [MaxLength(100)]
        [Column("breed")]
        public string? Breed { get; set; }

        [MaxLength(500)]
        [Column("notes")]
        public string? Notes { get; set; }

        [Required]
        [Column("userId")]
        public int UserId { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    [Table("appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("appointmentId")]
        public int AppointmentId { get; set; }

        [Required]
        [Column("petId")]
        public int PetId { get; set; }

        [Required]
        [Column("appointmentDate")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Column("isDone")]
        public bool IsDone { get; set; } = false;

        [Required]
        [Column("hospitalId")]
        public int HospitalId { get; set; }

        [Column("doctorId")]
        public int? DoctorId { get; set; }
    }

    public class NewAppointmentRequest
    {
        [Required(ErrorMessage = "Evcil hayvan zorunludur")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Şehir zorunludur")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "İlçe zorunludur")]
        [MaxLength(100)]
        public string DistrictName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hastane zorunludur")]
        public int HospitalId { get; set; }

        [Required(ErrorMessage = "Doktor zorunludur")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Randevu tarihi zorunludur")]
        public DateTime AppointmentDate { get; set; }
    }

    public class AppointmentResponseDto
    {
        public string PetName { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsDone { get; set; }
        public string? DoctorName { get; set; }
    }

    public class CreateDoctorRequest
    {
        [Required(ErrorMessage = "Doktor adı zorunludur")]
        [MaxLength(100)]
        public string DoctorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hastane zorunludur")]
        public int HospitalId { get; set; }
    }

    public class CreatePetRequest
    {
        [Required(ErrorMessage = "Evcil hayvan adı zorunludur")]
        [MaxLength(50)]
        public string PetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hayvan türü zorunludur")]
        [MaxLength(50)]
        public string PetType { get; set; } = string.Empty;

        public int? Age { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(100)]
        public string? Breed { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

     
        [Required(ErrorMessage = "Kullanıcı ID'si zorunludur")]
        public int UserId { get; set; }
    }

    // YENİ: Kullanıcı kayıt/giriş için DTO'lar
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [MaxLength(10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Geçerli 10 haneli telefon numarası giriniz")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir zorunludur")]
        public int CityId { get; set; }

        [MaxLength(1000)]
        public string? Address { get; set; } = string.Empty;
    }

    public class LoginUserRequest
    {
        [Required(ErrorMessage = "Telefon zorunludur")]
        [MaxLength(10)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        public string Password { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string? CityName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}