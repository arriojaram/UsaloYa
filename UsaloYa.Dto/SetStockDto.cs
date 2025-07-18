namespace UsaloYa.Dto
{
    public class SetStockDto
    {
        public int ProductId { get; set; }
        public decimal UnitsInStock { get; set; }
        public bool IsHardReset { get; set; }
    }
}
