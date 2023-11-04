using TugasAsinkron2.Models.DataContext;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TugasAsinkron2.Models
{
        public class SupplierViewModel : Supplier
        {
        [StringLength(50)]
        public string ProductName { get; set; } = null!;
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? UnitPrice { get; set; }
    }
    
}
