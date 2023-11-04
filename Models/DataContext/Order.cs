using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TugasAsinkron2.Models.DataContext
{
    [Table("Order")]
    [Index("CustomerId", Name = "IndexOrderCustomerId")]
    [Index("OrderDate", Name = "IndexOrderOrderDate")]
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }
        [StringLength(10)]
        public string? OrderNumber { get; set; }
        public int CustomerId { get; set; }
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? TotalAmount { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("Orders")]
        public virtual Customer Customer { get; set; } = null!;
        [InverseProperty("Order")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
