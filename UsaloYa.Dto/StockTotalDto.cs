namespace UsaloYa.Dto
{
    public class StockTotalDto
    {
        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitsInStock { get; set; }
        public int Measure {  get; set; }
        public bool Discontinued { get; set; }
    }
}
