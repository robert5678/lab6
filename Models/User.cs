using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaOrderApp.Models
{
    public class User
    {
        [Required(ErrorMessage = "��'� � ����'�������.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "���� ���������� � ����'�������.")]
        public DateTime? BirthDate { get; set; }

        public int Age => BirthDate.HasValue ? DateTime.Now.Year - BirthDate.Value.Year : 0;
    }
}
