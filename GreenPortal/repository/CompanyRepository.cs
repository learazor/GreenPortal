using GreenPortal.db;
using GreenPortal.model;

namespace GreenPortal.repository;

using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CompanyRepository
{
    private readonly CompanyDbContext _context;
    private readonly string _connectionString;

    public CompanyRepository(CompanyDbContext context)
    {
        _connectionString = "Host=abstractly-awake-ouzel.data-1.euc1.tembo.io;Database=postgres;Username=postgres;Password=AQm5ISoHY3fjnw67;Port=5432";
    }

    public async Task<List<CompanyInstallation>> GetInstallationsByType(string type)
    {
        var installations = new List<CompanyInstallation>();

        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            var query = "SELECT * FROM CompanyInstallation WHERE Type = @Type";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("Type", type);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        installations.Add(new CompanyInstallation
                        {
                            //Id = reader.GetInt32(0),
                            Type = reader.GetString(0),
                            Output = reader.GetDouble(1),
                            SettingUpTimePerUnit = reader.GetInt32(2),
                            PricePerUnit = reader.GetDouble(3),
                            CompanyCode = reader.GetString(4)
                        });
                    }
                }
            }
        }
        return installations;
    }
    
    public async Task<List<CompanyInfo>> GetAllCompanyInfo()
    {
        var companyInfoList = new List<CompanyInfo>();

        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            var query = "SELECT * FROM CompanyInfo";

            using (var cmd = new NpgsqlCommand(query, conn))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        companyInfoList.Add(new CompanyInfo
                        {
                            //Id = reader.GetInt32(0),
                            Name = reader.GetString(0),
                            Country = reader.GetString(1),
                            PostalCode = reader.GetString(2),
                            PricePerDistanceUnit = reader.GetDouble(3),
                            CompanyCode = reader.GetString(4)
                        });
                    }
                }
            }
        }

        return companyInfoList;
    }
    
    
}
