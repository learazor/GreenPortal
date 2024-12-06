using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GreenPortal.model;

namespace GreenPortal.repository
{
    public class CompanyRepository
    {
        private readonly GreenPortalContext _context;

        public CompanyRepository(GreenPortalContext context)
        {
            _context = context;
        }

        // Method to get installations by type
        public async Task<List<CompanyInstallation>> GetInstallationsByType(string type)
        {
            return await _context.companyinstallation
                .Where(ci => ci.type == type)
                .ToListAsync();
        }

        // Method to get installation by composite key
        public async Task<CompanyInstallation> GetInstallationByTypeAndCode(string type, string companyCode)
        {
            return await _context.companyinstallation
                .FirstOrDefaultAsync(ci => ci.type == type && ci.company_code == companyCode) ?? throw new InvalidOperationException();
        }
        
        public async Task<List<CompanyInfo>> GetAllCompanyInfo()
        {
            return await _context.companyinfo.ToListAsync();
        }
        
        public async Task<CompanyInfo> GetCompanyInfo(string companyCode)
        {
            return await _context.companyinfo.FirstOrDefaultAsync(ci => ci.company_code == companyCode) ?? throw new InvalidOperationException();
        }
    }
}