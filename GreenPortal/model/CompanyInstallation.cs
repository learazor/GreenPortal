namespace GreenPortal.model
{
    public class CompanyInstallation
    {
        // Composite Key
        public string type { get; set; }
        public string company_code { get; set; }
        
        public double output { get; set; }
        public int setting_up_time_per_unit { get; set; }
        public double price_per_unit { get; set; }
    }
}