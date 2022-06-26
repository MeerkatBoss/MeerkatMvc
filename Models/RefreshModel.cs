namespace MeerkatMvc.Models;

public record RefreshModel(
        string AccessToken,
        string RefreshToken
);
