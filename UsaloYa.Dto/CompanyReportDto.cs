namespace UsaloYa.Dto
{
    public class CompanyReportDto
    {
        public int NumberOfUsers {  get; set; }
        public string CompanyName { get; set; }
        public DateTime? LastAccess { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public int NumberOfProducts { get; set; }
        public int NumberOfSales { get; set; }
    }
}
