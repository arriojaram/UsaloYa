using UsaloYa.Dto;

namespace UsaloYa.Services.Interfaces
{
    public interface ICashCounter
    {
        public Task<List<CashCountDto>> GetAllByCompany(int companyId);
        
        public Task<CashCountDto> Get(int id);

        public Task<int> Save(CashCountDto item);


    }
}
