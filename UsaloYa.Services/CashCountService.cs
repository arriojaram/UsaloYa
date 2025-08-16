using Microsoft.EntityFrameworkCore;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.Interfaces;

namespace UsaloYa.Services
{
    public class CashCountService : ICashCounter
    {
        private readonly DBContext _dBContext;
        
        public CashCountService(DBContext dBContext) 
        {
            _dBContext = dBContext;
        }

        public async Task<List<CashCountDto>> GetAllByCompany(int companyId)
        {
            var cashCounts = await _dBContext.CashCounts
                .Include(cc => cc.User)
                .ThenInclude(u => u.Company)
                .Where(cc => cc.User.CompanyId == companyId)
                .OrderByDescending(cc => cc.ReferenceDate)
                .Select(cc => new CashCountDto
                {
                    Id = (int)cc.CashCountId,
                    UserId = cc.UserId,
                    
                    StatusId = cc.StatusId,
                    ReferenceDate = cc.ReferenceDate,
                    InitialBalance = cc.InitialBalance,
                    Cash = cc.Cash,
                    CredictCard = cc.CredictCard,
                    Spei = cc.Spei,
                    CashOutputTotal = cc.CashOutputTotal,
                    Notes = cc.Notes,
                    FinalCash = cc.FinalCash,
                    UserName = $"{cc.User.FirstName} {cc.User.LastName}"
                })
                .ToListAsync();

            return cashCounts;
        }

        public async Task<CashCountDto?> Get(int id)
        {
            var cashCount = await _dBContext.CashCounts
                .Include(cc => cc.User)
                .Where(cc => cc.CashCountId == id)
                .Select(cc => new CashCountDto
                {
                    Id = (int)cc.CashCountId,
                    UserId = cc.UserId,
                    
                    StatusId = cc.StatusId,
                    ReferenceDate = cc.ReferenceDate,
                    InitialBalance = cc.InitialBalance,
                    Cash = cc.Cash,
                    CredictCard = cc.CredictCard,
                    Spei = cc.Spei,
                    CashOutputTotal = cc.CashOutputTotal,
                    Notes = cc.Notes,
                    FinalCash = cc.FinalCash,
                    UserName = $"{cc.User.FirstName} {cc.User.LastName}"
                })
                .FirstOrDefaultAsync();

            return cashCount;
        }

        public async Task<int> Save(CashCountDto cashCountDto)
        {
            CashCount entity;

            if (cashCountDto.Id == 0)
            {
                // Create new
                entity = new CashCount
                {
                    CashCountId = 0,
                    UserId = cashCountDto.UserId,
                    StatusId = cashCountDto.StatusId,
                    ReferenceDate = cashCountDto.ReferenceDate,
                    InitialBalance = cashCountDto.InitialBalance,
                    Cash = cashCountDto.Cash,
                    CredictCard = cashCountDto.CredictCard,
                    Spei = cashCountDto.Spei,
                    CashOutputTotal = cashCountDto.CashOutputTotal,
                    Notes = cashCountDto.Notes,
                    FinalCash = cashCountDto.FinalCash
                };
                _dBContext.CashCounts.Add(entity);
            }
            else
            {
                // Update existing
                entity = await _dBContext.CashCounts
                    .FirstOrDefaultAsync(cc => cc.CashCountId == cashCountDto.Id);
                

                if (entity == null)
                    throw new InvalidOperationException($"CashCount con ID {cashCountDto.Id} no encontrado");

                entity.UserId = cashCountDto.UserId;
                entity.StatusId = cashCountDto.StatusId;
                entity.ReferenceDate = cashCountDto.ReferenceDate;
                entity.InitialBalance = cashCountDto.InitialBalance;
                entity.Cash = cashCountDto.Cash;
                entity.CredictCard = cashCountDto.CredictCard;
                entity.Spei = cashCountDto.Spei;
                entity.CashOutputTotal = cashCountDto.CashOutputTotal;
                entity.Notes = cashCountDto.Notes;
                entity.FinalCash = cashCountDto.FinalCash;
            }

            await _dBContext.SaveChangesAsync();

            return (int)entity.CashCountId;
        }

    }
}
