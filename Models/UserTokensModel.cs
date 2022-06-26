namespace MeerkatMvc.Models;

public record UserTokensModel(
        string SessionId,
        string AccessToken,
        string RefreshToken
);
