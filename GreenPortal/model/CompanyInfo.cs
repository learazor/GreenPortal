namespace GreenPortal.model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("CompanyInfo")]
public class CompanyInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
    public double PricePerDistanceUnit { get; set; }
    public string CompanyCode { get; set; }
}
