namespace MeerkatMvc.Models;

public record UserTokensModel(
        string AccessToken,
        string RefreshToken
);
