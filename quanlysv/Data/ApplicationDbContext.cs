using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Models;
using System.Linq;

namespace QuanLySinhVien.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 🟦 Khai báo DbSet cho tất cả model (tương ứng với bảng)
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🟨 Cấu hình bảng Student
            modelBuilder.Entity<Student>(entity =>
            {
                // Sử dụng tên số ít nếu tên bảng thực tế là student (khác với tên DbSet)
                entity.ToTable("students");
                entity.HasKey(e => e.StudentID);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Gender).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.NationalID).HasMaxLength(20);
            });

            // 🟨 Bảng Class
            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("classes");
                entity.HasKey(e => e.ClassID);
                entity.Property(e => e.ClassName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AcademicYear).HasMaxLength(50);
                entity.Property(e => e.HomeroomTeacher).HasMaxLength(100);
            });

            // 🟨 Bảng Faculty
            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.ToTable("faculties");
                entity.HasKey(e => e.FacultyID);
                entity.Property(e => e.FacultyName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.OfficeLocation).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(255);
            });

            // 🟨 Bảng Teacher
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("teachers");
                entity.HasKey(e => e.TeacherID);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Gender).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Position).HasMaxLength(50);
            });


            // ... (Đã sửa ToTable cho các bảng còn lại thành chữ thường, ví dụ "subjects", "semesters", etc.) ...
            modelBuilder.Entity<Subject>(entity => { entity.ToTable("subjects"); entity.HasKey(e => e.SubjectID); /*...*/ });
            modelBuilder.Entity<Semester>(entity => { entity.ToTable("semesters"); entity.HasKey(e => e.SemesterID); /*...*/ });
            modelBuilder.Entity<Enrollment>(entity => { entity.ToTable("enrollments"); entity.HasKey(e => e.EnrollmentID); /*...*/ });
            modelBuilder.Entity<Grade>(entity => { entity.ToTable("grades"); entity.HasKey(e => e.GradeID); /*...*/ });
            modelBuilder.Entity<Role>(entity => { entity.ToTable("roles"); entity.HasKey(e => e.RoleID); /*...*/ });
            modelBuilder.Entity<Account>(entity => { entity.ToTable("accounts"); entity.HasKey(e => e.AccountID); /*...*/ });

        }
    }
}