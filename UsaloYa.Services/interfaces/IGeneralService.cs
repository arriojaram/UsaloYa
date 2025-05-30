using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface IGeneralService
    {
        Task<IEnumerable<LicenseDto>> GetLicenses();

    }
}
