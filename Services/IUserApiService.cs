using MeerkatMvc.Models;

namespace MeerkatMvc.Services;

public interface IUserApiService
{
    Task<ProblemModel<UserModel>> SignUpAsync(ISession session, SignUpModel user);

    Task<ProblemModel<UserModel>> LogInAsync(ISession session, LoginModel credentials);

    Task<ProblemModel<UserModel>> GetUserAsync(ISession session);

    Task<ProblemModel<UserModel>> UpdateUserAsync(ISession session, UpdateModel model);

    Task<ProblemModel> DeleteUserAsync(ISession session, DeleteModel model);

}
