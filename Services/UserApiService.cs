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

    public Task<ProblemModel<LoginResultModel>> SignUp(SignUpModel user)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<LoginResultModel>> LogIn(LoginModel credentials)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<UserModel>> GetUser(string id)
    {
        throw new NotImplementedException();
    }

    public Task<ProblemModel<UserModel>> UpdateUser(string id, UpdateModel model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(string id, DeleteModel model)
    {
        throw new NotImplementedException();
    }


}
