using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaOrderApp.Models
{
    public class User
    {
        [Required(ErrorMessage = "Ім'я є обов'язковим.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Дата народження є обов'язковою.")]
        public DateTime? BirthDate { get; set; }

        public int Age => BirthDate.HasValue ? DateTime.Now.Year - BirthDate.Value.Year : 0;
    }
}
