using MeerkatMvc.Models;

namespace MeerkatMvc.Services;

public interface IUserApiService
{
    Task<ProblemModel<UserModel>> SignUpAsync(string sessionId, SignUpModel user);

    Task<ProblemModel<UserModel>> LogInAsync(string sessionId, LoginModel credentials);

    Task<ProblemModel<UserModel>> GetUserAsync(string sessionId);

    Task<ProblemModel<UserModel>> UpdateUserAsync(string sessionId, UpdateModel model);

    Task<ProblemModel> DeleteUserAsync(string sessionId, DeleteModel model);

}
