namespace MeerkatMvc.Models;

public record SignUpModel(
        string Username,
        string Password,
        string? Email = null,
        string? Phone = null
);
