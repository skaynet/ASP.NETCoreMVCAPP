using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreMVCAPP.Models;

[Table("GROUPS")]
[Index("Name", Name = "UQ_GROUP_NAME", IsUnique = true)]
public partial class Group
{
    [Key]
    [Column("GROUP_ID")]
    public int GroupId { get; set; }

    [Column("COURSE_ID")]
    [Display(Name = "Название курса")]
    public int? CourseId { get; set; }

    [Column("NAME")]
    [StringLength(20)]
    [Display(Name = "Название группы")]
    [Required(ErrorMessage = "Вам нужно ввести название группы")]
    public string Name { get; set; } = null!;

    [ForeignKey("CourseId")]
    [InverseProperty("Groups")]
    [Display(Name = "Название курса")]
    public virtual Course? Course { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<Student> Students { get; } = new List<Student>();
}
