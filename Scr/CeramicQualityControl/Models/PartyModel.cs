using System.ComponentModel.DataAnnotations;

namespace CeramicQualityControl.Models
{
    public class Party
    {
        [Key]
        public int PartyID { get; set; }

        public DateTime ProductionDate { get; set; }

        [StringLength(50)]
        public string ChangeNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string ProductType { get; set; } = string.Empty;

        [StringLength(50)]
        public string Size { get; set; } = string.Empty;

        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        public int NumberOfProducts { get; set; }
    }
}
