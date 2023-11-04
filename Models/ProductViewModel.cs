using TugasAsinkron2.Models.DataContext;
using System.ComponentModel.DataAnnotations;

namespace TugasAsinkron2.Models
{
    public class ProductViewModel : Product
    {
        [StringLength(40)]
        public string CompanyName { get; set; }   
    }
}
