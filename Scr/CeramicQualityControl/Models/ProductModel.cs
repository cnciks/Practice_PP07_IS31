using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CeramicQualityControl.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [ForeignKey("Party")]
        public int PartyID { get; set; }
        public Party Party { get; set; } = null!;

        public int ProductNumberInParty { get; set; }

        [StringLength(200)]
        public string Defect { get; set; } = string.Empty;
    }
}
