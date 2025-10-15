using System.ComponentModel.DataAnnotations;

namespace CeramicQualityControl.Models
{
    public class Defect
    {
        [Key]
        public int DefectID { get; set; }

        [Required]
        [StringLength(100)]
        public string NameOfDefect { get; set; } = string.Empty;

        [StringLength(500)]
        public string DescriptionOfDefect { get; set; } = string.Empty;

        [StringLength(50)]
        public string Criticality { get; set; } = string.Empty;
    }
}
