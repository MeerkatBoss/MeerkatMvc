using MeerkatMvc.Models;
using ServiceStack.Redis.Generic;

namespace MeerkatMvc.Repositories;

public class RedisTokensRepository : ITokensRepository
{
    private readonly IRedisTypedClientAsync<UserTokensModel> _database;

    public RedisTokensRepository(IRedisTypedClientAsync<UserTokensModel> client)
    {
        _database = client;
    }

    public async Task<UserTokensModel> AddTokensAsync(string sessionId, UserTokensModel model)
    {
        await _database.SetValueAsync(sessionId, model);
        return new(model.AccessToken, model.RefreshToken);
    }

    public Task<UserTokensModel> GetTokensAsync(string sessionId)
    {
        return _database.GetValueAsync(sessionId).AsTask();
    }

    public Task<UserTokensModel> UpdateTokensAsync(string sessionId, UserTokensModel model)
    {
        return AddTokensAsync(sessionId, model);
    }

    public Task DeleteTokensAsync(string sessionId)
    {
        return _database.RemoveEntryAsync(sessionId).AsTask();
    }

}
