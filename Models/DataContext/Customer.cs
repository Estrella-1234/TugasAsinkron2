using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TugasAsinkron2.Models.DataContext
{
    [Table("Customer")]
    [Index("LastName", "FirstName", Name = "IndexCustomerName")]
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(40)]
        public string FirstName { get; set; } = null!;
        [StringLength(40)]
        public string LastName { get; set; } = null!;
        [StringLength(40)]
        public string? City { get; set; }
        [StringLength(40)]
        public string? Country { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
