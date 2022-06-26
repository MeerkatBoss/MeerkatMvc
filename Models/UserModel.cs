namespace MeerkatMvc.Models;

public record UserModel(
        int Id,
        string Username,
        string? Email = null,
        string? Phone = null
);
