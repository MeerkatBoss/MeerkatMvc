namespace MeerkatMvc.Models;

public record LoginResultModel(
        string AccessToken,
        string RefreshToken,
        UserModel User
);
