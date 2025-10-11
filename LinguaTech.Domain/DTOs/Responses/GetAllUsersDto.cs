namespace LinguaTech.Domain.DTOs.Responses;

public class GetAllUsersDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}