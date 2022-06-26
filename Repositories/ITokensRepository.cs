using MeerkatMvc.Models;

namespace MeerkatMvc.Repositories;

public interface ITokensRepository
{
    Task<UserTokensModel> AddTokensAsync(string sessionId, UserTokensModel model);

    Task<UserTokensModel> GetTokensAsync(string sessionId);

    Task<UserTokensModel> UpdateTokensAsync(string sessionId, UserTokensModel model);

    Task DeleteTokensAsync(string sessionId);
}
