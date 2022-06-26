using MeerkatMvc.Models;

namespace MeerkatMvc.Services;

public interface IUserApiService
{
    Task<ProblemModel<LoginResultModel>> SignUp(SignUpModel user);

    Task<ProblemModel<LoginResultModel>> LogIn(LoginModel credentials);

    Task<ProblemModel<UserModel>> GetUser(string id);

    Task<ProblemModel<UserModel>> UpdateUser(string id, UpdateModel model);

    Task DeleteUser(string id);

}
