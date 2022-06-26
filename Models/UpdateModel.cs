namespace MeerkatMvc.Models;

public record UpdateModel(
        string? Username = null,
        string? Password = null,
        string? Email = null,
        string? Phone = null
);
