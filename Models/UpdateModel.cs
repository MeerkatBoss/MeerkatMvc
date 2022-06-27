namespace MeerkatMvc.Models;

public record UpdateModel(
        string OldPassword,
        string? Username = null,
        string? Password = null,
        string? Email = null,
        string? Phone = null
);
