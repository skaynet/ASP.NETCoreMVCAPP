using System;
using System.Collections.Generic;
using ASP.NETCoreMVCAPP.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreMVCAPP.Data;

public partial class UniversityContext : DbContext
{
    public UniversityContext()
    {
    }

    public UniversityContext(DbContextOptions<UniversityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK_COURSE_ID");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK_GROUP_ID");

            entity.HasOne(d => d.Course).WithMany(p => p.Groups).HasConstraintName("FK_GROUPS_To_COURSES");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK_STUDENT_ID");

            entity.HasOne(d => d.Group).WithMany(p => p.Students).HasConstraintName("FK_STUDENTS_To_GROUPS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
