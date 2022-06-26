using MeerkatMvc.Models;

namespace MeerkatMvc.Repositories;

public interface ITokensRepository
{
    Task<UserTokensModel> AddTokensAsync(UserTokensModel model);

    Task<UserTokensModel> GetTokensAsync(string id);

    Task<UserTokensModel> UpdateTokensAsync(UserTokensModel model);

    Task DeleteTokensAsync(string id);
}
