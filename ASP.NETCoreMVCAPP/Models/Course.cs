using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreMVCAPP.Models;

[Table("COURSES")]
[Index("Name", Name = "UQ_COURSE_NAME", IsUnique = true)]
public partial class Course
{
    [Key]
    [Column("COURSE_ID")]
    public int CourseId { get; set; }

    [Column("NAME")]
    [StringLength(20)]
    [Display(Name = "Название курса")]
    [Required(ErrorMessage = "Вам нужно ввести название курса")]
    public string Name { get; set; } = null!;

    [Column("DESCRIPTION")]
    [StringLength(30)]
    [Display(Name = "Описание курса")]
    [Required(ErrorMessage = "Вам нужно ввести описание курса")]
    public string Description { get; set; } = null!;

    [InverseProperty("Course")]
    public virtual ICollection<Group> Groups { get; } = new List<Group>();
}
