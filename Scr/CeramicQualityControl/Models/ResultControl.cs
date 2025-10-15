using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CeramicQualityControl.Models
{
    public class ResultControl
    {
        [Key]
        public int ResultID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;

        [ForeignKey("Defect")]
        public int DefectID { get; set; }
        public Defect Defect { get; set; } = null!;

        public DateTime MonitoringData { get; set; }

        [StringLength(100)]
        public string ControlOperator { get; set; } = string.Empty;

        [StringLength(200)]
        public string LocationOfDefect { get; set; } = string.Empty;

        public decimal DefectSize { get; set; }

        public byte[]? DefectImage { get; set; }
    }
}
