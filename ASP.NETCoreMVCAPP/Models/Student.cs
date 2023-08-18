using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreMVCAPP.Models;

[Table("STUDENTS")]
public partial class Student
{
    [Key]
    [Column("STUDENT_ID")]
    public int StudentId { get; set; }

    [Column("GROUP_ID")]
    [Display(Name = "Название группы")]
    public int? GroupId { get; set; }

    [Column("FIRST_NAME")]
    [StringLength(20)]
    [Display(Name = "Имя студента")]
    [Required(ErrorMessage = "Вам нужно ввести имя студента")]
    public string FirstName { get; set; } = null!;

    [Column("LAST_NAME")]
    [StringLength(20)]
    [Display(Name = "Фамилия студента")]
    [Required(ErrorMessage = "Вам нужно ввести фамилию студента")]
    public string LastName { get; set; } = null!;

    [ForeignKey("GroupId")]
    [InverseProperty("Students")]
    [Display(Name = "Название группы")]
    public virtual Group? Group { get; set; }
}
