public class GroupDto
{
    public int GroupId { get; set; }            // Opcional en creación
    public string Name { get; set; }
    public string Description { get; set; }
    public string Permissions { get; set; }     // Opcional si no se usa al registrar
    public int CompanyId { get; set; }          // Lo asignas en el backend
}
