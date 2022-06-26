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

    public Task<UserTokensModel> AddTokensAsync(UserTokensModel model)
    {
        throw new NotImplementedException();
    }

    public Task<UserTokensModel> GetTokensAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<UserTokensModel> UpdateTokensAsync(UserTokensModel model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTokensAsync(string id)
    {
        throw new NotImplementedException();
    }

}
