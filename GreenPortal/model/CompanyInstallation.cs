namespace GreenPortal.model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("CompanyInstallation")]
public class CompanyInstallation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Type { get; set; }
    public double Output { get; set; }
    public int SettingUpTimePerUnit { get; set; }
    public double PricePerUnit { get; set; }
    public string CompanyCode { get; set; }
}
