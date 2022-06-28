using System.Net;
using MeerkatMvc.Models;
using MeerkatMvc.Repositories;

namespace MeerkatMvc.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _client;
    private readonly ITokensRepository _database;
    private const string SERVER_ERROR = "Something went wrong. Try again later.";
    private const string LOGIN_FAILED = "Wrong username or password.";
    private const string WRONG_PASSWORD = "Wrong password.";
    private const string NOT_FOUND = "User does not exist.";

    public UserApiService(HttpClient client, ITokensRepository database)
    {
        _client = client;
        _database = database;
    }

    public async Task<ProblemModel<UserModel>> SignUpAsync(string sessionId, SignUpModel user)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("sign_up", user);
        var json = response.Content as JsonContent;
        if (response.IsSuccessStatusCode)
        {
            LoginResultModel? result = await json!.ReadFromJsonAsync<LoginResultModel>();
            await _database.AddTokensAsync(
                    sessionId,
                    new UserTokensModel(result!.AccessToken, result!.RefreshToken));
            return result!.User;
        }
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => await ParseValidationProblem(json!),
            _ => new (SERVER_ERROR)
        };
    }

    public async Task<ProblemModel<UserModel>> LogInAsync(string sessionId, LoginModel credentials)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("log_in", credentials);
        var json = response.Content as JsonContent;
        LoginResultModel? result = await json!.ReadFromJsonAsync<LoginResultModel>();
        await _database.AddTokensAsync(
                sessionId,
                new UserTokensModel(result!.AccessToken, result!.RefreshToken));
        return result!.User;
    }

    public async Task<ProblemModel<UserModel>> GetUserAsync(string sessionId)
    {
        (string accessToken, string refreshToken) = await _database.GetTokensAsync(sessionId);
        var request = new HttpRequestMessage(HttpMethod.Get, "");
        request.Headers.Authorization = new("Bearer", accessToken);
        HttpResponseMessage response = await _client.SendAsync(request);
        var json = response.Content as JsonContent;
        UserModel? result = await json!.ReadFromJsonAsync<UserModel>();
        return result!;
    }

    public async Task<ProblemModel<UserModel>> UpdateUserAsync(string sessionId, UpdateModel model)
    {
        (string accessToken, string refreshToken) = await _database.GetTokensAsync(sessionId);
        var request = new HttpRequestMessage(HttpMethod.Put, "");
        request.Headers.Authorization = new("Bearer", accessToken);
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

    public async Task<ProblemModel> DeleteUserAsync(string sessionId, DeleteModel model)
    {
        (string accessToken, string refreshToken) = await _database.GetTokensAsync(sessionId);
        HttpResponseMessage response;
        var request = new HttpRequestMessage(HttpMethod.Delete, "");
        request.Headers.Authorization = new("Bearer", accessToken);
        request.Content = JsonContent.Create(model);
        response = await _client.SendAsync(request);
        await _database.DeleteTokensAsync(sessionId);
        return new();
    }

    public async Task<ProblemModel> ParseValidationProblem(JsonContent json)
    {
        var problem = await json.ReadFromJsonAsync<ValidationProblemModel>();
        return new(problem!);
    }

}
