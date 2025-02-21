namespace UsaloYa.API.DTO
{

    public class CompanySettingsDto
    {
        public int CompanyId { get; set; }
        public List<PairSettingsDto> Settings { get; set; }
    }

    public class PairSettingsDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

}
