namespace UsaloYa.Dto
{
    public struct UserDto
    {
        public int UserId { get; set; }
        public bool IsEnabled { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CompanyId { get; set; }
        public int GroupId { get; set; }
        public int CreatedBy { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime? LastAccess { get; set; }
        public DateTime? CreationDate { get; set; }
        public int CompanyStatusId { get; set; }
        public int? RoleId { get; set; }
        public string? CodeVerification { get; set; }
        public bool? IsVerifiedCode { get; set; }
        public string? Email { get; set; }
    }
}
