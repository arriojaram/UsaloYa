using System.Text.Json.Serialization;
using UsaloYa.Dto.Enums;

namespace UsaloYa.Dto
{
    public struct UpdateSaleDto
    {
        public int CompanyId { get; set; }
        public int SaleId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SaleStatus Status { get; set; }
    }
}
