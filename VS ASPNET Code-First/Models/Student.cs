using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }
        [Display(Name = "Имя и фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Электронная почта")]
        public string FirstMidName { get; set; }

        [Display(Name = "Дата")]
        public DateTime EnrollmentDate { get; set; }
        
        [Display(Name = "Куасы ")]
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
