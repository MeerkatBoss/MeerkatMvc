using System.Net;
using MeerkatMvc.Models;

namespace MeerkatMvc.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _client;
    private const string SERVER_ERROR = "Something went wrong. Try again later.";
    private const string LOGIN_FAILED = "Wrong username or password.";
    private const string WRONG_PASSWORD = "Wrong password.";
    private const string NOT_FOUND = "User does not exist.";

    public UserApiService(HttpClient client)
    {
        _client = client;
    }

    public async Task<ProblemModel<UserModel>> SignUpAsync(ISession session, SignUpModel user)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("sign_up", user);
        var json = response.Content as JsonContent;
        if (response.IsSuccessStatusCode)
        {
            LoginResultModel? result = await json!.ReadFromJsonAsync<LoginResultModel>();
            return result!.User;
        }
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => await ParseValidationProblem(json!),
            _ => new (SERVER_ERROR)
        };
    }

    public async Task<ProblemModel<UserModel>> LogInAsync(ISession session, LoginModel credentials)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("log_in", credentials);
        var json = response.Content as JsonContent;
        LoginResultModel? result = await json!.ReadFromJsonAsync<LoginResultModel>();
        return result!.User;
    }

    public async Task<ProblemModel<UserModel>> GetUserAsync(ISession session)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "");
        request.Headers.Authorization = new("Bearer", String.Empty);
        HttpResponseMessage response = await _client.SendAsync(request);
        var json = response.Content as JsonContent;
        UserModel? result = await json!.ReadFromJsonAsync<UserModel>();
        return result!;
    }

    public async Task<ProblemModel<UserModel>> UpdateUserAsync(ISession session, UpdateModel model)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, "");
        request.Headers.Authorization = new("Bearer", String.Empty);
        request.Content = JsonContent.Create(model);
        HttpResponseMessage response = await _client.SendAsync(request);
        var json = response.Content as JsonContent;
        if (response.IsSuccessStatusCode)
        {
            UserModel? result = await json!.ReadFromJsonAsync<UserModel>();
            return result!;
        }
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => await ParseValidationProblem(json!),
            HttpStatusCode.Unauthorized => new (WRONG_PASSWORD),
            HttpStatusCode.NotFound => new (NOT_FOUND),
            _ => new (SERVER_ERROR)
        };

    }

    public async Task<ProblemModel> DeleteUserAsync(ISession session, DeleteModel model)
    {
        HttpResponseMessage response;
        var request = new HttpRequestMessage(HttpMethod.Delete, "");
        request.Headers.Authorization = new("Bearer", String.Empty);
        request.Content = JsonContent.Create(model);
        response = await _client.SendAsync(request);
        return new();
    }

    public async Task<ProblemModel> ParseValidationProblem(JsonContent json)
    {
        var problem = await json.ReadFromJsonAsync<ValidationProblemModel>();
        return new(problem!);
    }

}
