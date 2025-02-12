using System.Text.Json.Serialization;
using static UsaloYa.API.Enumerations;

namespace UsaloYa.API.DTO
{
    public struct UpdateSaleDto
    {
        public int CompanyId { get; set; }
        public int SaleId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SaleStatus Status { get; set; }
    }
}
