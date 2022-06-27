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

    public Task<ProblemModel<LoginResultModel>> SignUp(string sessionId, SignUpModel user)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<LoginResultModel>> LogIn(string sessionId, LoginModel credentials)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<UserModel>> GetUser(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<UserModel>> UpdateUser(string sessionId, UpdateModel model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(string sessionId, DeleteModel model)
    {
        throw new NotImplementedException();
    }


}
