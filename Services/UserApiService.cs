using MeerkatMvc.Models;
using MeerkatMvc.Repositories;

namespace MeerkatMvc.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _client;
    private readonly ITokensRepository _database;

    public UserApiService(HttpClient client, ITokensRepository database)
    {
        _client = client;
        _database = database;
    }

    public Task<ProblemModel<UserModel>> SignUpAsync(string sessionId, SignUpModel user)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<UserModel>> LogInAsync(string sessionId, LoginModel credentials)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<UserModel>> GetUserAsync(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<UserModel>> UpdateUserAsync(string sessionId, UpdateModel model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(string sessionId, DeleteModel model)
    {
        throw new NotImplementedException();
    }


}
