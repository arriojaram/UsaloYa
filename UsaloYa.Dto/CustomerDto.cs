namespace UsaloYa.Dto
{
    public struct CustomerDto
    {
        public int CustomerId { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }

        public string FirstName { get; set; } 

        public string LastName1 { get; set; } 

        public string? LastName2 { get; set; }

        public string? WorkPhoneNumber { get; set; }

        public string? CellPhoneNumber { get; set; }

        public string? Email { get; set; }

        public string FullName {
            get 
            {
                return (FirstName?? "") + " " + (LastName1?? "") + " " + (LastName2?? "");
            }        
        }

        public int CompanyId { get; set; }

    }
}
