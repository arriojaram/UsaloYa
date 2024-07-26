namespace UsaloYa.API.DTO
{
    public struct UserResponseDto
    {
        public int UserId { get; set; }
        public bool IsEnabled { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public int StatusId { get; set; }
        public DateTime? LastAccess { get; set; }
    }
}
