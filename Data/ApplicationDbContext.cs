using Microsoft.EntityFrameworkCore;
using AcademicGradingSystem.Models;

namespace AcademicGradingSystem.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de entidades y relaciones (Fluent API para claridad)
            
            // AcademicPeriod: Uno-a-muchos con Courses
            modelBuilder.Entity<AcademicPeriod>(entity =>
            {
                entity.HasKey(e => e.PeriodId);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.HasMany(e => e.Courses)
                      .WithOne(c => c.AcademicPeriod)
                      .HasForeignKey(c => c.PeriodId)
                      .OnDelete(DeleteBehavior.Restrict);  // Evita borrado en cascada accidental
            });

            // Course: Relaciones con Subject, AcademicPeriod, User (Teacher), Enrollments, EvaluationPlans
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);
                entity.Property(e => e.CourseName).HasMaxLength(120).IsRequired();
                entity.HasOne(e => e.Subject)
                      .WithMany(s => s.Courses)
                      .HasForeignKey(e => e.SubjectId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.AcademicPeriod)
                      .WithMany(ap => ap.Courses)
                      .HasForeignKey(e => e.PeriodId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Teacher)
                      .WithMany(u => u.CoursesTaught)
                      .HasForeignKey(e => e.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.Enrollments)
                      .WithOne(en => en.Course)
                      .HasForeignKey(en => en.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);  // Borra inscripciones si se borra el curso
                entity.HasMany(e => e.EvaluationPlans)
                      .WithOne(ep => ep.Course)
                      .HasForeignKey(ep => ep.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Enrollment: Relaciones con User (Student) y Course
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.HasOne(e => e.Student)
                      .WithMany(u => u.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // EvaluationPlan: Uno-a-muchos con Grades
            modelBuilder.Entity<EvaluationPlan>(entity =>
            {
                entity.HasKey(e => e.PlanId);
                entity.Property(e => e.ActivityName).HasMaxLength(120).IsRequired();
                entity.Property(e => e.Weight).HasPrecision(3, 2).IsRequired();  // Para decimales como 25.50
                entity.HasOne(e => e.Course)
                      .WithMany(c => c.EvaluationPlans)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Grades)
                      .WithOne(g => g.EvaluationPlan)
                      .HasForeignKey(g => g.PlanId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Grade: Relaciones con EvaluationPlan y User (Student)
            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasKey(e => e.GradeId);
                entity.Property(e => e.Score).HasPrecision(3, 2).IsRequired();  // Para puntuaciones como 85.50
                entity.HasOne(e => e.EvaluationPlan)
                      .WithMany(ep => ep.Grades)
                      .HasForeignKey(e => e.PlanId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Student)
                      .WithMany()  // No hay colección en User para Grades (agrega si quieres: public ICollection<Grade> Grades { get; set; })
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Configuración para AcademicProgram
            modelBuilder.Entity<AcademicProgram>(entity =>
            {
                  entity.HasKey(e => e.ProgramId);
                  entity.Property(e => e.ProgramName).HasMaxLength(120).IsRequired();
                  entity.Property(e => e.Description).HasMaxLength(500);
                  entity.HasMany<Subject>(e => e.Subjects)
                        .WithOne(s => s.Program)
                        .HasForeignKey(s => s.ProgramId)
                        .OnDelete(DeleteBehavior.Cascade);
            });


            // Report: Relaciones con User (Student) y Course
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.FinalGrade).HasPrecision(3, 2);
                entity.HasOne(e => e.Student)
                      .WithMany()  // Agrega colección en User si necesitas
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Course)
                      .WithMany()  // Agrega colección en Course si necesitas
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Role: Uno-a-muchos con Users
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.HasMany(e => e.Users)
                      .WithOne(u => u.Role)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Subject: Relaciones con Program y Courses
            modelBuilder.Entity<Subject>(entity =>
            {
                  entity.HasKey(e => e.SubjectId);
                  entity.Property(e => e.SubjectName).HasMaxLength(120).IsRequired();
                  entity.Property(e => e.Code).HasMaxLength(20).IsRequired();
                  entity.HasOne(s => s.Program)
                        .WithMany(p => p.Subjects)
                        .HasForeignKey(s => s.ProgramId)
                        .OnDelete(DeleteBehavior.Cascade);
                  entity.HasMany(s => s.Courses)
                        .WithOne(c => c.Subject)
                        .HasForeignKey(c => c.SubjectId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            // User: Relaciones con Role, CoursesTaught, Enrollments (y AuditLogs si existe)
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();

                // 🔒 Índice único en Email
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.CoursesTaught)
                      .WithOne(c => c.Teacher)
                      .HasForeignKey(c => c.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Enrollments)
                      .WithOne(en => en.Student)
                      .HasForeignKey(en => en.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Si agregas AuditLog más adelante:
                // entity.HasMany(e => e.AuditLogs)
                //       .WithOne(al => al.User)
                //       .HasForeignKey(al => al.UserId);
            });


            // Si tienes AuditLog, agrégala aquí similar a las otras

            base.OnModelCreating(modelBuilder);  // Llama a la configuración base de EF
        }

        // DbSets: Uno por entidad (nombres plurales para tablas)
        public DbSet<AcademicPeriod> AcademicPeriods { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<EvaluationPlan> EvaluationPlans { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<AcademicProgram> AcademicProgram { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }
        // Si agregas AuditLog: public DbSet<AuditLog> AuditLogs { get; set; }
    }
}