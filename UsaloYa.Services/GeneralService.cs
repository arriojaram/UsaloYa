using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class GeneralService : IGeneralService
    {
        private readonly DBContext _dBContext;

        public GeneralService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<IEnumerable<LicenseDto>> GetLicenses()
        {
            var licenses = await _dBContext.PlanRentas.OrderBy(u => u.Name).ToListAsync();
            return licenses.Select(c => new LicenseDto
            {
                Id = c.Id,
                Name = c.Name,
                Notes = c.Notes,
                NumUsers = c.NumUsers,
                Price = c.Price,
                StatusId = c.StatusId
            });
        }
    }
}
