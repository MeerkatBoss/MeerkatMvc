using MeerkatMvc.Models;

namespace MeerkatMvc.Services;

public interface IUserApiService
{
    Task<ProblemModel<LoginResultModel>> SignUp(string sessionId, SignUpModel user);

    Task<ProblemModel<LoginResultModel>> LogIn(string sessionId, LoginModel credentials);

    Task<ProblemModel<UserModel>> GetUser(string sessionId);

    Task<ProblemModel<UserModel>> UpdateUser(string sessionId, UpdateModel model);

    Task DeleteUser(string sessionId, DeleteModel model);

}
